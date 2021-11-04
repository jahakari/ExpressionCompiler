using ExpressionCompiler.Nodes;
using ExpressionCompiler.Utility;
using ExpressionCompiler.Visitors;
using System;
using System.Reflection;
using System.Reflection.Emit;
using static System.Reflection.Emit.OpCodes;

namespace ExpressionCompiler.Compilation
{
    public class NodeCompiler : INodeVisitor
    {
        private static ModuleBuilder module;
        private static int methodCount = 0;

        static NodeCompiler()
        {
            var name = new AssemblyName("Dynamic_Expressions");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            module = assembly.DefineDynamicModule("Dynamic_Expressions_Module");
        }

        private readonly Node node;
        private readonly string methodName;
        private readonly MethodBuilder methodBuilder;
        private readonly ILGenerator il;

        public NodeCompiler(Node node)
        {
            this.node = node;

            Type returnType = GetNodeType(node);
            methodName = $"Dynamic_Method_{++methodCount}";
            methodBuilder = module.DefineGlobalMethod(methodName, MethodAttributes.Public | MethodAttributes.Static, returnType, new[] { typeof(IIdentifierDataContext)} );
            il = methodBuilder.GetILGenerator();
        }

        public MethodInfo Compile()
        {
            node.Accept(this);
            il.Emit(Ret);

            module.CreateGlobalFunctions();
            return module.GetMethod(methodName);
        }

        public Node VisitAnd(AndFunctionNode node)
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

        public Node VisitBinary(BinaryExpressionNode node)
        {
            if (TryCompileToConstant(node)) {
                return node;
            }

            if (node.Operator.OperatorType == OperatorType.Exponent) {
                CompileExponent(node);
                return node;
            }

            node.Left.Accept(this);
            node.Right.Accept(this);

            if (node.ValueType == NodeValueType.Decimal) {
                MethodInfo mInfo = TypeHelper.GetDecimalMethodForOperator(node.Operator.OperatorType);
                il.Emit(Call, mInfo);

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

        private void CompileExponent(BinaryExpressionNode node)
        {
            node.Left.Accept(this);
            EmitConversionToDouble(node.Left);

            node.Right.Accept(this);
            EmitConversionToDouble(node.Right);

            il.Emit(Call, TypeHelper.MathPowMethod);
            EmitConversionFromDouble(node);
        }

        private void EmitConversionToDouble(Node node)
        {
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

        public Node VisitDate(DateFunctionNode node)
        {
            node.Year.Accept(this);
            node.Month.Accept(this);
            node.Day.Accept(this);

            il.Emit(Newobj, TypeHelper.DateTimeConstructor);
            return node;
        }

        public Node VisitDay(DayFunctionNode node)
        {
            CompileDateProperty(node.Date, TypeHelper.DateTimeDayGetter);
            return node;
        }

        public Node VisitIdentifier(IdentifierNode node)
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

        public Node VisitIf(IfFunctionNode node)
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

        public Node VisitLeft(LeftFunctionNode node)
        {
            int textLoc = il.DeclareLocal(typeof(string)).LocalIndex;

            node.Text.Accept(this);
            il.Emit(Stloc, textLoc);
            il.Emit(Ldloc, textLoc);
            il.Emit(Ldc_I4_0);

            node.Count.Accept(this);
            il.Emit(Ldloc, textLoc);
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
            int firstLoc = il.DeclareLocal(typeof(int)).LocalIndex;
            int secondLoc = il.DeclareLocal(typeof(int)).LocalIndex;

            Label first = il.DefineLabel();
            Label end = il.DefineLabel();

            il.Emit(Stloc, secondLoc);
            il.Emit(Stloc, firstLoc);

            il.Emit(Ldloc, firstLoc);
            il.Emit(Ldloc, secondLoc);

            il.Emit(Clt);
            il.Emit(Brtrue, first);

            il.Emit(Ldloc, secondLoc);
            il.Emit(Br, end);

            il.MarkLabel(first);
            il.Emit(Ldloc, firstLoc);

            il.MarkLabel(end);
        }

        public Node VisitLiteral(LiteralValueNode node)
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

        public Node VisitMonth(MonthFunctionNode node)
        {
            CompileDateProperty(node.Date, TypeHelper.DateTimeMonthGetter);
            return node;
        }

        public Node VisitOr(OrFunctionNode node)
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

        public Node VisitRight(RightFunctionNode node)
        {
            int textLoc = il.DeclareLocal(typeof(string)).LocalIndex;
            int lengthLoc = il.DeclareLocal(typeof(int)).LocalIndex;

            node.Text.Accept(this);
            il.Emit(Stloc, textLoc);

            il.Emit(Ldloc, textLoc);
            il.Emit(Call, TypeHelper.StringLengthGetter);
            il.Emit(Stloc, lengthLoc);

            // start index = stringLength - chars

            il.Emit(Ldloc, textLoc);
            il.Emit(Ldloc, lengthLoc);

            il.Emit(Ldloc, lengthLoc);
            node.Count.Accept(this);
            EmitMin();

            il.Emit(Sub);
            il.Emit(Call, TypeHelper.StringSubstringMethod);

            return node;
        }

        public Node VisitToday(TodayFunctionNode node)
        {
            il.Emit(Call, TypeHelper.DateTimeTodayGetter);
            return node;
        }

        public Node VisitYear(YearFunctionNode node)
        {
            CompileDateProperty(node.Date, TypeHelper.DateTimeYearGetter);
            return node;
        }

        private void CompileDateProperty(Node dateNode, MethodInfo getter)
        {
            int dateLoc = il.DeclareLocal(typeof(DateTime)).LocalIndex;
            dateNode.Accept(this);

            il.Emit(Stloc, dateLoc);
            il.Emit(Ldloca, dateLoc);
            il.Emit(Call, getter);
        }

        private Type GetNodeType(Node node)
        {
            return node.ValueType switch
            {
                NodeValueType.Boolean => typeof(bool),
                NodeValueType.Integer => typeof(int),
                NodeValueType.Decimal => typeof(decimal),
                NodeValueType.Number  => typeof(decimal),
                NodeValueType.Date    => typeof(DateTime),
                NodeValueType.String  => typeof(string),
                _                     => throw new InvalidOperationException($"Node value type '{node.ValueType}' is not supported.")
            };
        }

        private bool TryCompileToConstant(BinaryExpressionNode node)
        {
            if (node.Left is not LiteralValueNode || node.Right is not LiteralValueNode) {
                return false;
            }

            if (node.Left is LiteralValueNode<int> left && node.Right is LiteralValueNode<int> right) {
                unchecked {
                    switch (node.Operator.OperatorType) {
                        case OperatorType.Add:
                            EmitInt32(left.Value + right.Value);
                            break;

                        case OperatorType.Subtract:
                            EmitInt32(left.Value - right.Value);
                            break;

                        case OperatorType.Multiply:
                            EmitInt32(left.Value * right.Value);
                            break;

                        case OperatorType.Divide:
                            EmitInt32(left.Value / right.Value);
                            break;

                        case OperatorType.Modulo:
                            EmitInt32(left.Value % right.Value);
                            break;

                        case OperatorType.Exponent:
                            EmitInt32((int)Math.Pow(left.Value, right.Value));
                            break;

                        case OperatorType.Equal:
                            EmitBoolean(left.Value == right.Value);
                            break;

                        case OperatorType.NotEqual:
                            EmitBoolean(left.Value != right.Value);
                            break;

                        case OperatorType.LessThan:
                            EmitBoolean(left.Value < right.Value);
                            break;

                        case OperatorType.LessThanOrEqual:
                            EmitBoolean(left.Value <= right.Value);
                            break;

                        case OperatorType.GreaterThan:
                            EmitBoolean(left.Value > right.Value);
                            break;

                        case OperatorType.GreaterThanOrEqual:
                            EmitBoolean(left.Value >= right.Value);
                            break;

                        default:
                            return false;
                    }

                    return true;
                }
            }

            return false;
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