using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using ExpressionCompiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static System.Reflection.Emit.OpCodes;

namespace ExpressionCompiler.Compilation
{
    public class NodeCompiler : NodeVisitor
    {
        private static readonly ModuleBuilder moduleBuilder;
        private static int count = 0;

        static NodeCompiler()
        {
            var name = new AssemblyName("Dynamic_Expressions");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            moduleBuilder = assembly.DefineDynamicModule("Dynamic_Expressions_Module");
        }

        private readonly Node node;
        private readonly string methodName;
        private readonly TypeBuilder typeBuilder;
        private readonly MethodBuilder methodBuilder;
        private readonly ILGenerator il;

        public NodeCompiler(Node node)
        {
            this.node = node;

            typeBuilder = moduleBuilder.DefineType($"ExpressionCompiler.Dynamic_Type_{count++}", TypeAttributes.Public);
            Type returnType = NodeUtils.GetNodeType(node);
            methodName = $"Dynamic_Method_{count}";
            methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Static, returnType, new[] { typeof(IIdentifierDataContext)} );
            il = methodBuilder.GetILGenerator();
        }

        public MethodInfo Compile()
        {
            node.Accept(this);
            il.Emit(Ret);

            return typeBuilder.CreateType().GetMethod(methodName);
        }

        public override Node VisitAbs(AbsFunctionNode node)
        {
            Label endLabel = il.DefineLabel();
            node.Argument.Accept(this);

            il.Emit(Dup);

            if (node.Argument.ValueType == NodeValueType.Integer) {
                EmitInt32(0);
                EmitGreaterThanOrEqual();
                il.Emit(Brtrue, endLabel);

                il.Emit(Neg);
            } else {
                EmitDecimal(0);
                il.Emit(Call, TypeHelper.DecimalGreaterThanOrEqualMethod);
                il.Emit(Brtrue, endLabel);

                MethodInfo method = typeof(decimal).GetMethod("op_UnaryNegation");
                il.Emit(Call, method);
            }

            il.MarkLabel(endLabel);
            return node;
        }

        public override Node VisitAnd(AndFunctionNode node)
        {
            Label falseLabel = il.DefineLabel();
            Label endLabel = il.DefineLabel();

            foreach (Node n in node.Arguments) {
                n.Accept(this);
                il.Emit(Brfalse, falseLabel);
            }

            EmitTrue(); //If we're here in the IL instructions, none of the arguments evaluated to false
            il.Emit(Br, endLabel);

            il.MarkLabel(falseLabel);
            EmitFalse();

            il.MarkLabel(endLabel);

            return node;
        }

        public override Node VisitBinary(BinaryExpressionNode node)
        {
            if (node.Operator.OperatorType == OperatorType.Exponent) {
                CompileExponentBinaryExpression(node);
                return node;
            }

            if (NodeUtils.GetCommonDataType(node.Left, node.Right) is NodeValueType.Decimal or NodeValueType.Number) {
                CompileDecimalBinaryExpression(node);
                return node;
            }

            node.Left.Accept(this);
            node.Right.Accept(this);

            //if node.Right isn't also a date, then the validating visitor has failed us
            if (node.Left.ValueType == NodeValueType.Date) {
                MethodInfo opMethod = typeof(DateTime).GetMethod(TypeHelper.GetOperatorNameForOperatorType(node.Operator.OperatorType));
                il.Emit(Call, opMethod);

                return node;
            }

            OperatorType opType = node.Operator.OperatorType;

            switch (opType) {
                case OperatorType.NotEqual:
                    EmitNotEqual();
                    break;

                case OperatorType.LessThanOrEqual:
                    EmitLessThanOrEqual();
                    break;

                case OperatorType.GreaterThanOrEqual:
                    EmitGreaterThanOrEqual();
                    break;

                default:
                    OpCode opCode = GetOpCodeForOperatorType(opType);
                    il.Emit(opCode);

                    break;
            }

            return node;
        }

        private void CompileExponentBinaryExpression(BinaryExpressionNode node)
        {
            EmitAsDouble(node.Left);
            EmitAsDouble(node.Right);

            il.Emit(Call, TypeHelper.MathPowMethod);
            EmitConversionFromDouble(node);
        }

        private void CompileDecimalBinaryExpression(BinaryExpressionNode node)
        {
            EmitAsDecimal(node.Left);
            EmitAsDecimal(node.Right);

            MethodInfo mInfo = TypeHelper.GetDecimalMethodForOperator(node.Operator.OperatorType);
            il.Emit(Call, mInfo);
        }

        private void EmitAsDecimal(Node node)
        {
            node.Accept(this);

            if (node.ValueType == NodeValueType.Integer) {
                il.Emit(Newobj, TypeHelper.DecimalConstructorFromInt);
            }
        }

        private void EmitAsDouble(Node node)
        {
            node.Accept(this);

            if (node.ValueType == NodeValueType.Integer) {
                il.Emit(Conv_R8);
            } else {
                il.Emit(Call, TypeHelper.DecimalToFloatExplicitCastMethod);
            }
        }

        private void EmitConversionFromDouble(Node node)
        {
            if (node.ValueType == NodeValueType.Integer) {
                il.Emit(Conv_Ovf_I4);
            }
            else {
                il.Emit(Newobj, TypeHelper.DecimalConstructorFromDouble);
            }
        }

        private OpCode GetOpCodeForOperatorType(OperatorType opType)
        {
            return opType switch
            {
                OperatorType.Add         => Add,
                OperatorType.Subtract    => Sub,
                OperatorType.Multiply    => Mul,
                OperatorType.Divide      => Div,
                OperatorType.Modulo      => Rem,
                OperatorType.Equal       => Ceq,
                OperatorType.LessThan    => Clt,
                OperatorType.GreaterThan => Cgt,
                _                        => throw new InvalidOperationException($"Unsupported OperatorType '{opType}'.")
            };
        }

        private void EmitNotEqual()
        {
            il.Emit(Ceq);
            EmitFalse();
            il.Emit(Ceq);
        }

        private void EmitLessThanOrEqual()
        {
            //"Less Than or Equal" == "Not Greater Than"
            il.Emit(Cgt);
            EmitFalse();
            il.Emit(Ceq);
        }

        private void EmitGreaterThanOrEqual()
        {
            //"Greater Than or Equal" == "Not Less Than"
            il.Emit(Clt);
            EmitFalse();
            il.Emit(Ceq);
        }

        public override Node VisitCInt(CIntFunctionNode node)
        {
            node.Argument.Accept(this);

            switch (node.Argument.ValueType) {
                case NodeValueType.Decimal:
                case NodeValueType.Number:
                    MethodInfo cast = TypeHelper.GetExplicitCastMethod(typeof(decimal), typeof(int));
                    il.Emit(Call, cast);
                    break;

                case NodeValueType.String:
                    MethodInfo parse = typeof(int).GetMethod(nameof(int.Parse), new[] { typeof(string) });
                    il.Emit(Call, parse);

                    break;
            }

            return node;
        }

        public override Node VisitCString(CStringFunctionNode node)
        {
            node.Argument.Accept(this);

            switch (node.Argument.ValueType) {
                case NodeValueType.String:
                    break;

                default:
                    Type type = NodeUtils.GetNodeType(node.Argument);
                    MethodInfo toString = type.GetMethod("ToString", Type.EmptyTypes);

                    LocalBuilder value = il.DeclareLocal(type);
                    il.Emit(Stloc, value);
                    il.Emit(Ldloca, value);

                    il.Emit(Call, toString);
                    break;
            }

            return node;
        }

        public override Node VisitDate(DateFunctionNode node)
        {
            node.Year.Accept(this);
            node.Month.Accept(this);
            node.Day.Accept(this);

            il.Emit(Newobj, TypeHelper.DateTimeConstructor);
            return node;
        }

        public override Node VisitDay(DayFunctionNode node)
        {
            CompileDateProperty(node.Date, TypeHelper.DateTimeDayGetter);
            return node;
        }

        public override Node VisitGroup(GroupNode node)
        {
            node.Inner.Accept(this);
            return node;
        }

        public override Node VisitIdentifier(IdentifierNode node)
        {
            MethodInfo method = node.ValueType switch
            {
                NodeValueType.Integer => TypeHelper.DataContextGetIntegerMethod,
                NodeValueType.Decimal => TypeHelper.DataContextGetDecimalMethod,
                NodeValueType.Date    => TypeHelper.DataContextGetDateMethod,
                NodeValueType.String  => TypeHelper.DataContextGetStringMethod,
                _                     => throw new InvalidOperationException($"Value type '{node.ValueType}' is not supported for identifiers")
            };

            il.Emit(Ldarg_0);
            il.Emit(Ldstr, node.Identifier);
            il.Emit(Callvirt, method);

            return node;
        }

        public override Node VisitIf(IfFunctionNode node)
        {
            Label falseLabel = il.DefineLabel();
            Label end = il.DefineLabel();

            node.Condition.Accept(this);
            il.Emit(Brfalse, falseLabel);

            node.IfTrue.Accept(this);
            il.Emit(Br, end);

            il.MarkLabel(falseLabel);
            node.IfFalse.Accept(this);
            il.MarkLabel(end);

            return node;
        }

        public override Node VisitLeft(LeftFunctionNode node)
        {
            LocalBuilder text = il.DeclareLocal(typeof(string));

            node.Text.Accept(this);
            il.Emit(Dup);
            il.Emit(Stloc, text);
            il.Emit(Ldc_I4_0);

            node.Count.Accept(this);
            il.Emit(Ldloc, text);
            il.Emit(Call, TypeHelper.StringLengthGetter);

            EmitMin();
            il.Emit(Call, TypeHelper.StringSubstringWithLengthMethod);

            return node;
        }

        /// <summary>
        /// Given 2 int values on the stack, pops both and pushes the smallest value back onto the stack
        /// </summary>
        private void EmitMin()
        {
            LocalBuilder value1 = il.DeclareLocal(typeof(int));
            LocalBuilder value2 = il.DeclareLocal(typeof(int));

            Label first = il.DefineLabel();
            Label end = il.DefineLabel();

            il.Emit(Stloc, value2);
            il.Emit(Stloc, value1);

            il.Emit(Ldloc, value1);
            il.Emit(Ldloc, value2);

            il.Emit(Clt);
            il.Emit(Brtrue, first);

            il.Emit(Ldloc, value2);
            il.Emit(Br, end);

            il.MarkLabel(first);
            il.Emit(Ldloc, value1);

            il.MarkLabel(end);
        }

        public override Node VisitLiteral(LiteralValueNode node)
        {
            switch (node.ValueType) {
                case NodeValueType.Boolean:
                    var b = (LiteralValueNode<bool>)node;
                    EmitBoolean(b.Value);

                    break;

                case NodeValueType.Integer:
                    var i = (LiteralValueNode<int>)node;
                    EmitInt32(i.Value);

                    break;

                case NodeValueType.Decimal:
                    var d = (LiteralValueNode<decimal>)node;
                    EmitDecimal(d.Value);

                    break;

                case NodeValueType.Date:
                    var date = (LiteralValueNode<DateTime>)node;

                    EmitInt32(date.Value.Year);
                    EmitInt32(date.Value.Month);
                    EmitInt32(date.Value.Day);
                    il.Emit(Newobj, TypeHelper.DateTimeConstructor);

                    break;

                case NodeValueType.String:
                    var s = (LiteralValueNode<string>)node;
                    il.Emit(Ldstr, s.Value);

                    break;

                default:
                    break;
            }

            return node;
        }

        public override Node VisitMax(MaxFunctionNode node)
        {
            if (node.ValueType == NodeValueType.Integer) {
                EmitIntegerMinMaxFunction(node.Arguments, Bgt);
            } else {
                EmitDecimalMinMaxFunction(node.Arguments, TypeHelper.DecimalGreaterThanMethod);
            }

            return node;
        }

        public override Node VisitMin(MinFunctionNode node)
        {
            if (node.ValueType == NodeValueType.Integer) {
                EmitIntegerMinMaxFunction(node.Arguments, Blt);
            } else {
                EmitDecimalMinMaxFunction(node.Arguments, TypeHelper.DecimalLessThanMethod);
            }

            return node;
        }

        private void EmitIntegerMinMaxFunction(List<Node> arguments, OpCode comparer)
        {
            LocalBuilder final = il.DeclareLocal(typeof(int));
            LocalBuilder current = il.DeclareLocal(typeof(int));

            arguments[0].Accept(this);
            il.Emit(Stloc, final);
            il.Emit(Ldloc, final);

            foreach (Node n in arguments.Skip(1)) {
                Label label = il.DefineLabel();
                n.Accept(this);
                il.Emit(Stloc, current);
                il.Emit(Ldloc, current);
                il.Emit(comparer, label);

                il.Emit(Ldloc, current);
                il.Emit(Stloc, final);

                il.MarkLabel(label);
                il.Emit(Ldloc, final);
            }
        }

        private void EmitDecimalMinMaxFunction(List<Node> arguments, MethodInfo comparer)
        {
            LocalBuilder final = il.DeclareLocal(typeof(decimal));
            LocalBuilder current = il.DeclareLocal(typeof(decimal));

            EmitAsDecimal(arguments[0]);
            il.Emit(Stloc, final);
            il.Emit(Ldloc, final);

            foreach (Node n in arguments.Skip(1)) {
                Label label = il.DefineLabel();
                EmitAsDecimal(n);
                il.Emit(Stloc, current);
                il.Emit(Ldloc, current);

                il.Emit(Call, comparer);
                il.Emit(Brtrue, label);

                il.Emit(Ldloc, current);
                il.Emit(Stloc, final);

                il.MarkLabel(label);
                il.Emit(Ldloc, final);
            }
        }

        public override Node VisitMonth(MonthFunctionNode node)
        {
            CompileDateProperty(node.Date, TypeHelper.DateTimeMonthGetter);
            return node;
        }

        public override Node VisitNegation(NegationNode node)
        {
            node.Operand.Accept(this);

            if (node.ValueType == NodeValueType.Integer) {
                il.Emit(Neg);
            } else {
                MethodInfo method = typeof(decimal).GetMethod("op_UnaryNegation");
                il.Emit(Call, method);
            }

            return node;
        }

        public override Node VisitOr(OrFunctionNode node)
        {
            Label trueLabel = il.DefineLabel();
            Label endLabel = il.DefineLabel();

            foreach (Node n in node.Arguments) {
                n.Accept(this);
                il.Emit(Brtrue, trueLabel);
            }

            EmitFalse(); //If we're here in the IL instructions, none of the arguments evaluated to true
            il.Emit(Br, endLabel);

            il.MarkLabel(trueLabel);
            EmitTrue();

            il.MarkLabel(endLabel);

            return node;
        }

        public override Node VisitRight(RightFunctionNode node)
        {
            LocalBuilder text = il.DeclareLocal(typeof(string));
            LocalBuilder length = il.DeclareLocal(typeof(int));

            node.Text.Accept(this);
            il.Emit(Dup);
            il.Emit(Stloc, text);

            il.Emit(Call, TypeHelper.StringLengthGetter);
            il.Emit(Stloc, length);

            // start index = stringLength - chars

            il.Emit(Ldloc, text);
            il.Emit(Ldloc, length);

            il.Emit(Dup);
            node.Count.Accept(this);
            EmitMin();

            il.Emit(Sub);
            il.Emit(Call, TypeHelper.StringSubstringMethod);

            return node;
        }

        public override Node VisitToday(TodayFunctionNode node)
        {
            il.Emit(Call, TypeHelper.DateTimeTodayGetter);
            return node;
        }

        public override Node VisitYear(YearFunctionNode node)
        {
            CompileDateProperty(node.Date, TypeHelper.DateTimeYearGetter);
            return node;
        }

        private void CompileDateProperty(Node dateNode, MethodInfo getter)
        {
            LocalBuilder date = il.DeclareLocal(typeof(DateTime));
            dateNode.Accept(this);

            il.Emit(Stloc, date);
            il.Emit(Ldloca, date);
            il.Emit(Call, getter);
        }

        private void EmitInt32(int value)
        {
            switch (value) {
                case -1:
                    il.Emit(Ldc_I4_M1);
                    break;

                case 0:
                    il.Emit(Ldc_I4_0);
                    break;
                    
                case 1:
                    il.Emit(Ldc_I4_1);
                    break;
                    
                case 2:
                    il.Emit(Ldc_I4_2);
                    break;
                    
                case 3:
                    il.Emit(Ldc_I4_3);
                    break;
                    
                case 4:
                    il.Emit(Ldc_I4_4);
                    break;
                    
                case 5:
                    il.Emit(Ldc_I4_5);
                    break;
                    
                case 6:
                    il.Emit(Ldc_I4_6);
                    break;
                    
                case 7:
                    il.Emit(Ldc_I4_7);
                    break;
                    
                case 8:
                    il.Emit(Ldc_I4_8);
                    break;

                default:
                    if (value is >= sbyte.MinValue and <= sbyte.MaxValue) {
                        il.Emit(Ldc_I4_S, (sbyte)value);
                    } else {
                        il.Emit(Ldc_I4, value);
                    }

                    break;
            }
        }

        private void EmitDecimal(decimal value)
        {
            //https://stackoverflow.com/questions/33570554/emit-il-code-to-load-a-decimal-value
            int[] bits = decimal.GetBits(value);

            bool sign = (bits[3] & 0x80000000) != 0;
            int scale = (byte)((bits[3] >> 16) & 0x7f);

            EmitInt32(bits[0]);
            EmitInt32(bits[1]);
            EmitInt32(bits[2]);
            EmitBoolean(sign);
            EmitInt32(scale);

            il.Emit(Newobj, TypeHelper.DecimalConstructorFromParts);
        }

        private void EmitBoolean(bool value)
        {
            if (value) {
                EmitTrue();
            } else {
                EmitFalse();
            }
        }

        private void EmitTrue() => il.Emit(Ldc_I4_1);

        private void EmitFalse() => il.Emit(Ldc_I4_0);
    }
}