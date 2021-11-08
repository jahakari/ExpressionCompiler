using ExpressionCompiler.Syntax.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Utility
{
    public static class NodeUtils
    {
        public static Node CreateConstant(int i1, int i2, OperatorType @operator)
        {
            if (IsMathOperator(@operator)) {
                int i = CreateIntConstantValue(i1, i2, @operator);
                return new LiteralValueNode<int>(i);
            }

            if (IsBooleanOperator(@operator)) {
                bool b = CreateBooleanConstantValue(i1, i2, @operator);
                return new LiteralValueNode<bool>(b);
            }

            throw new ArgumentException("Specified operator is not supported.", nameof(@operator));
        }

        public static Node CreateConstant(decimal d1, decimal d2, OperatorType @operator)
        {
            if (IsMathOperator(@operator)) {
                decimal d = CreateDecimalConstantValue(d1, d2, @operator);
                return new LiteralValueNode<decimal>(d);
            }

            if (IsBooleanOperator(@operator)) {
                bool b = CreateBooleanConstantValue(d1, d2, @operator);
                return new LiteralValueNode<bool>(b);
            }

            throw new ArgumentException("Specified operator is not supported.", nameof(@operator));
        }

        private static int CreateIntConstantValue(int i1, int i2, OperatorType @operator)
        {
            return @operator switch
            {
                OperatorType.Add      => i1 + i2,
                OperatorType.Subtract => i1 - i2,
                OperatorType.Multiply => i1 * i2,
                OperatorType.Divide   => i1 / i2,
                OperatorType.Modulo   => i1 % i2,
                OperatorType.Exponent => (int)Math.Pow(i1, i2),
                _                     => throw new ArgumentException("This method should not be called with a non-mathematical operator.", nameof(@operator))
            };
        }

        private static decimal CreateDecimalConstantValue(decimal d1, decimal d2, OperatorType @operator)
        {
            return @operator switch
            {
                OperatorType.Add      => d1 + d2,
                OperatorType.Subtract => d1 - d2,
                OperatorType.Multiply => d1 * d2,
                OperatorType.Divide   => d1 / d2,
                OperatorType.Modulo   => d1 % d2,
                OperatorType.Exponent => (decimal)Math.Pow((double)d1, (double)d2),
                _                     => throw new ArgumentException("This method should not be called with a non-mathematical operator.", nameof(@operator))
            };
        }

        private static bool CreateBooleanConstantValue(int i1, int i2, OperatorType @operator)
        {
            return @operator switch
            {
                OperatorType.Equal              => i1 == i2,
                OperatorType.NotEqual           => i1 != i2,
                OperatorType.LessThan           => i1 < i2,
                OperatorType.LessThanOrEqual    => i1 <= i2,
                OperatorType.GreaterThan        => i1 > i2,
                OperatorType.GreaterThanOrEqual => i1 >= i2,
                _                               => throw new ArgumentException("This method should not be called with a non-boolean operator.", nameof(@operator))
            };
        }

        private static bool CreateBooleanConstantValue(decimal d1, decimal d2, OperatorType @operator)
        {
            return @operator switch
            {
                OperatorType.Equal              => d1 == d2,
                OperatorType.NotEqual           => d1 != d2,
                OperatorType.LessThan           => d1 < d2,
                OperatorType.LessThanOrEqual    => d1 <= d2,
                OperatorType.GreaterThan        => d1 > d2,
                OperatorType.GreaterThanOrEqual => d1 >= d2,
                _                               => throw new ArgumentException("This method should not be called with a non-boolean operator.", nameof(@operator))
            };
        }

        public static bool IsMathOperator(OperatorType @operator)
        {
            if ((OperatorType.Math & @operator) != @operator) {
                return false;
            }

            /*
             We want to ensure that this operator represents a single type of operation (Add, Subtract, etc.), and not a category (AddOrSubtract, Math, etc.).
             
             OperatorType.Multiply     = 0100 (4)
             OperatorType.Multiply - 1 = 0011 (3)

             0100 & 0011 == 0000
             */
            return (@operator & (@operator - 1)) == 0;
        }

        public static bool IsBooleanOperator(OperatorType @operator)
        {
            if ((OperatorType.Boolean & @operator) != @operator) {
                return false;
            }

            /*
             We want to ensure that this operator represents a single type of operation (Equal, LessThan, etc.), and not a category (Boolean).
             
             OperatorType.Equal     = 01000000 (64)
             OperatorType.Equal - 1 = 00111111 (63)

             01000000 & 00111111 == 00000000
             */
            return (@operator & (@operator - 1)) == 0;
        }

        public static NodeValueType GetCommonDataType(params Node[] nodes) => GetCommonDataType(nodes.AsEnumerable());

        public static NodeValueType GetCommonDataType(IEnumerable<Node> nodes)
        {
            NodeValueType type = NodeValueType.None;

            foreach (Node node in nodes) {
                type |= node.ValueType;
            }

            return type;
        }
    }
}
