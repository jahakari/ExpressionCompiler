using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Visitors
{
    public class NodeValidatingVisitor : NodeVisitor
    {
        public List<string> Errors { get; } = new();

        public void Reset() => Errors.Clear();

        public override Node VisitAbs(AbsFunctionNode node)
        {
            if (!NodeUtils.IsNumber(node.Argument)) {
                Error($"ABS function argument '{node.Argument}' is invalid; argument must be a numeric expression.");
            }

            return base.VisitAbs(node);
        }

        public override Node VisitAnd(AndFunctionNode node)
        {
            foreach (Node n in node.Arguments) {
                if (n.ValueType != NodeValueType.Boolean) {
                    Error($"AND function argument '{n}' is invalid; each argument must be a Boolean expression.");
                }
            }

            return base.VisitAnd(node);
        }

        public override Node VisitBinary(BinaryExpressionNode node)
        {
            Node left = node.Left;
            Node right = node.Right;

            if (NodeUtils.IsBooleanOperator(node.Operator)) {
                if (!NodeUtils.CanCompare(left, right, node.Operator)) {
                    Error($"Expressions '{left}' and '{right}' cannot be compared with operator '{node.Operator}'.");
                }
            } else {
                NodeValueType commonType = NodeUtils.GetCommonDataType(left, right);

                if (!NodeUtils.IsNumber(commonType)) {
                    Error($"Expression '{node}' is invalid; left and right operands must be numeric.");
                }
            }

            return base.VisitBinary(node);
        }

        public override Node VisitCInt(CIntFunctionNode node)
        {
            if (node.Argument.ValueType is not NodeValueType.Integer and not NodeValueType.Decimal and not NodeValueType.String) {
                Error($"Expression '{node.Argument}' cannot be converted using the CINT function.");
            }

            return base.VisitCInt(node);
        }

        public override Node VisitComplex(ComplexExpressionNode node)
        {
            if (node.Nodes.Count == 1) {
                goto exit;
            }

            //Parser ensures a valid operand-operator-operand sequence, no need to check

            List<BinaryOperatorNode> booleanOps = node.Nodes.OfType<BinaryOperatorNode>()
                .Where(NodeUtils.IsBooleanOperator)
                .ToList();

            if (booleanOps.Count > 1) {
                Error($"Expression '{node}' is invalid; multiple boolean operators in a complex expression are not supported.");
                goto exit;
            }

            if (booleanOps.Count > 0) {
                int index = node.Nodes.IndexOf(booleanOps[0]);
                IEnumerable<Node> left = node.Nodes.Take(index);
                IEnumerable<Node> right = node.Nodes.Skip(index + 1);

                var binaryNode = new BinaryExpressionNode
                (
                    new ComplexExpressionNode(left),
                    booleanOps[0],
                    new ComplexExpressionNode(right)
                );

                return VisitBinary(binaryNode);
            }

            NodeValueType commonOperandType = NodeUtils.GetCommonDataType(node.Nodes.EveryNth(2));

            if (!NodeUtils.IsNumber(commonOperandType)) {
                Error($"Expression '{node}' is invalid; all operands must be numeric.");
            }

            exit:
            return base.VisitComplex(node);
        }

        public override Node VisitDate(DateFunctionNode node)
        {
            Node[] args = { node.Year, node.Month, node.Day };

            foreach (Node n in args) {
                if (n.ValueType != NodeValueType.Integer) {
                    Error($"DATE function argument '{n}' is invalid; argument must be an integer expression.");
                }
            }

            return base.VisitDate(node);
        }

        public override Node VisitDay(DayFunctionNode node)
        {
            if (node.Date.ValueType != NodeValueType.Date) {
                Error($"DAY function argument '{node.Date}' is invalid; argument must be a date expression.");
            }

            return base.VisitDay(node);
        }

        public override Node VisitIf(IfFunctionNode node)
        {
            if (node.Condition.ValueType != NodeValueType.Boolean) {
                Error($"IF function argument '{node.Condition}' is invalid; argument must be a Boolean expression.");
            }

            NodeValueType returnType = NodeUtils.GetCommonDataType(node.IfTrue, node.IfFalse);

            if (returnType == NodeValueType.None || !Enum.IsDefined(returnType)) {
                Error($"IF function return arguments '{node.IfTrue}' and '{node.IfFalse}' are not supported, or do not have compatible data types.");
            }

            return base.VisitIf(node);
        }

        public override Node VisitLeft(LeftFunctionNode node)
        {
            if (node.Text.ValueType != NodeValueType.String) {
                Error($"LEFT function argument '{node.Text}' is invalid; argument must be a string expression.");
            }

            if (node.Count.ValueType != NodeValueType.Integer) {
                Error($"LEFT function argument '{node.Count}' is invalid; argument must be an integer expression.");
            }

            return base.VisitLeft(node);
        }

        public override Node VisitMax(MaxFunctionNode node)
        {
            foreach (Node n in node.Arguments) {
                if (!NodeUtils.IsNumber(n)) {
                    Error($"MAX function argument '{n}' is invalid; argument must be a numeric expression.");
                }
            }

            return base.VisitMax(node);
        }

        public override Node VisitMin(MinFunctionNode node)
        {
            foreach (Node n in node.Arguments) {
                if (!NodeUtils.IsNumber(n)) {
                    Error($"MIN function argument '{n}' is invalid; argument must be a numeric expression.");
                }
            }

            return base.VisitMin(node);
        }

        public override Node VisitMonth(MonthFunctionNode node)
        {
            if (node.Date.ValueType != NodeValueType.Date) {
                Error($"MONTH function argument '{node.Date}' is invalid; argument must be a date expression.");
            }

            return base.VisitMonth(node);
        }

        public override Node VisitNegation(NegationNode node)
        {
            if (!NodeUtils.IsNumber(node.Operand)) {
                Error($"Negation expression '{node}' is invalid; the operand must be a numeric expression.");
            }

            return base.VisitNegation(node);
        }

        public override Node VisitOr(OrFunctionNode node)
        {
            foreach (Node n in node.Arguments) {
                if (n.ValueType != NodeValueType.Boolean) {
                    Error($"OR function argument '{n}' is invalid; each argument must be a Boolean expression.");
                }
            }

            return base.VisitOr(node);
        }

        public override Node VisitRight(RightFunctionNode node)
        {
            if (node.Text.ValueType != NodeValueType.String) {
                Error($"RIGHT function argument '{node.Text}' is invalid; argument must be a string expression.");
            }

            if (node.Count.ValueType != NodeValueType.Integer) {
                Error($"RIGHT function argument '{node.Count}' is invalid; argument must be an integer expression.");
            }

            return base.VisitRight(node);
        }

        public override Node VisitYear(YearFunctionNode node)
        {
            if (node.Date.ValueType != NodeValueType.Date) {
                Error($"YEAR function argument '{node.Date}' is invalid; argument must be a date expression.");
            }

            return base.VisitYear(node);
        }

        private void Error(string error) => Errors.Add(error);
    }
}
