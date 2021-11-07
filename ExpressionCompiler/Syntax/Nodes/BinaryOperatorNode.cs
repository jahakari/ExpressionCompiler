using ExpressionCompiler.Compilation;
using ExpressionCompiler.Visitors;
using System;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class BinaryOperatorNode : Node
    {
        public BinaryOperatorNode(string @operator)
        {
            Operator = @operator;
            OperatorType = GetOperatorType(@operator);
            OperatorSubType = GetOperatorSubType(OperatorType);

            if (OperatorSubType == OperatorType.Math) {
                MathOperatorSubType = GetMathOperatorSubType(OperatorType); 
            }
        }

        public override NodeType NodeType => NodeType.BinaryOperator;
        public override NodeValueType ValueType => NodeValueType.None;

        public string Operator { get; }
        public OperatorType OperatorType { get; }
        public OperatorType OperatorSubType { get; }
        public OperatorType MathOperatorSubType { get; } = OperatorType.None;

        private OperatorType GetOperatorType(string @operator)
        {
            return @operator switch
            {
                "+"  => OperatorType.Add,
                "-"  => OperatorType.Subtract,
                "*"  => OperatorType.Multiply,
                "/"  => OperatorType.Divide,
                "%"  => OperatorType.Modulo,
                "^"  => OperatorType.Exponent,
                "="  => OperatorType.Equal,
                "!=" => OperatorType.NotEqual,
                "<>" => OperatorType.NotEqual,
                "<"  => OperatorType.LessThan,
                "<=" => OperatorType.LessThanOrEqual,
                ">"  => OperatorType.GreaterThan,
                ">=" => OperatorType.GreaterThanOrEqual,
                _    => throw new InvalidOperationException($"Binary operator '{@operator}' is not supported.")
            };
        }

        private OperatorType GetOperatorSubType(OperatorType operatorType)
        {
            if ((OperatorType.Math & operatorType) == operatorType) {
                return OperatorType.Math;
            }

            return OperatorType.Boolean;
        }

        private OperatorType GetMathOperatorSubType(OperatorType operatorType)
        {
            if ((OperatorType.AddOrSubtract & operatorType) == operatorType) {
                return OperatorType.AddOrSubtract;
            }

            if ((OperatorType.MultiplyOrDivide & operatorType) == operatorType) {
                return OperatorType.MultiplyOrDivide;
            }

            return OperatorType;
        }

        public override Node Accept(NodeVisitor visitor) => this;

        public override string ToString() => Operator;
    }
}
