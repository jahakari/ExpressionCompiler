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
            ValidateValueSemanticType(node.Argument);

            if (!NodeUtils.IsNumber(node.Argument)) {
                Error($"ABS function argument '{node.Argument}' is invalid; argument must be a numeric expression.");
            }

            return base.VisitAbs(node);
        }

        public override Node VisitAnd(AndFunctionNode node)
        {
            ValidateValueSemanticType(node.Arguments);

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

            ValidateValueSemanticType(left);
            ValidateValueSemanticType(right);

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

        public override Node VisitCase(CaseFunctionNode node)
        {
            ValidateValueSemanticType(node.SwitchArgument);
            ValidateValueSemanticType(node.DefaultArgument);
            ValidateValueSemanticType(node.ReturnArguments);

            foreach (Node n in node.CaseArguments) {
                if (n is not RelationalNode r) {
                    Error($"CASE function argument '{n}' is invalid; argument must be a relational expression.");
                    continue;
                }

                if (!NodeUtils.CanCompare(node.SwitchArgument, r.Operand, r.Operator)) {
                    Error($"CASE function argument '{n}' is invalid; switch argument '{node.SwitchArgument}' cannot be compared to operand '{r.Operand}' using operator '{r.Operator}'.");
                }
            }

            if (!Enum.IsDefined(node.ValueType)) {
                Error("One or more CASE function return arguments are invalid; all return arguments must share a common data type.");
            }

            return base.VisitCase(node);
        }

        public override Node VisitCInt(CIntFunctionNode node)
        {
            ValidateValueSemanticType(node.Argument);

            if (node.Argument.ValueType is not NodeValueType.Integer and not NodeValueType.Decimal and not NodeValueType.String) {
                Error($"Expression '{node.Argument}' cannot be converted using the CINT function.");
            }

            return base.VisitCInt(node);
        }

        public override Node VisitComplex(ComplexExpressionNode node)
        {
            if (node.Nodes.Count == 1) {
                ValidateValueSemanticType(node.Nodes[0]);
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

            if (!NodeUtils.IsNumber(node.ValueType)) {
                Error($"Expression '{node}' is invalid; all operands must be numeric.");
            }

            ValidateValueSemanticType(node.Nodes.EveryNth(2)); //operands

            exit:
                return base.VisitComplex(node);
        }

        public override Node VisitDate(DateFunctionNode node)
        {
            Node[] args = { node.Year, node.Month, node.Day };
            ValidateValueSemanticType(args);

            foreach (Node n in args) {
                if (n.ValueType != NodeValueType.Integer) {
                    Error($"DATE function argument '{n}' is invalid; argument must be an integer expression.");
                }
            }

            return base.VisitDate(node);
        }

        public override Node VisitDay(DayFunctionNode node)
        {
            ValidateValueSemanticType(node.Date);

            if (node.Date.ValueType != NodeValueType.Date) {
                Error($"DAY function argument '{node.Date}' is invalid; argument must be a date expression.");
            }

            return base.VisitDay(node);
        }

        public override Node VisitIf(IfFunctionNode node)
        {
            ValidateValueSemanticType(node.Condition);
            ValidateValueSemanticType(node.IfTrue);
            ValidateValueSemanticType(node.IfFalse);

            if (node.Condition.ValueType != NodeValueType.Boolean) {
                Error($"IF function argument '{node.Condition}' is invalid; argument must be a Boolean expression.");
            }

            if (node.ValueType == NodeValueType.None || !Enum.IsDefined(node.ValueType)) {
                Error($"IF function return arguments '{node.IfTrue}' and '{node.IfFalse}' are not supported, or do not have compatible data types.");
            }

            return base.VisitIf(node);
        }

        public override Node VisitLeft(LeftFunctionNode node)
        {
            ValidateValueSemanticType(node.Text);
            ValidateValueSemanticType(node.Count);

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
            ValidateValueSemanticType(node.Arguments);

            foreach (Node n in node.Arguments) {
                if (!NodeUtils.IsNumber(n)) {
                    Error($"MAX function argument '{n}' is invalid; argument must be a numeric expression.");
                }
            }

            return base.VisitMax(node);
        }

        public override Node VisitMin(MinFunctionNode node)
        {
            ValidateValueSemanticType(node.Arguments);

            foreach (Node n in node.Arguments) {
                if (!NodeUtils.IsNumber(n)) {
                    Error($"MIN function argument '{n}' is invalid; argument must be a numeric expression.");
                }
            }

            return base.VisitMin(node);
        }

        public override Node VisitMonth(MonthFunctionNode node)
        {
            ValidateValueSemanticType(node.Date);

            if (node.Date.ValueType != NodeValueType.Date) {
                Error($"MONTH function argument '{node.Date}' is invalid; argument must be a date expression.");
            }

            return base.VisitMonth(node);
        }

        public override Node VisitNegation(NegationNode node)
        {
            ValidateValueSemanticType(node.Operand);

            if (!NodeUtils.IsNumber(node.Operand)) {
                Error($"Negation expression '{node}' is invalid; the operand must be a numeric expression.");
            }

            return base.VisitNegation(node);
        }

        public override Node VisitOr(OrFunctionNode node)
        {
            ValidateValueSemanticType(node.Arguments);

            foreach (Node n in node.Arguments) {
                if (n.ValueType != NodeValueType.Boolean) {
                    Error($"OR function argument '{n}' is invalid; each argument must be a Boolean expression.");
                }
            }

            return base.VisitOr(node);
        }

        public override Node VisitRelational(RelationalNode node)
        {
            ValidateValueSemanticType(node.Operand);
            return base.VisitRelational(node);
        }

        public override Node VisitRight(RightFunctionNode node)
        {
            ValidateValueSemanticType(node.Text);
            ValidateValueSemanticType(node.Count);

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
            ValidateValueSemanticType(node.Date);

            if (node.Date.ValueType != NodeValueType.Date) {
                Error($"YEAR function argument '{node.Date}' is invalid; argument must be a date expression.");
            }

            return base.VisitYear(node);
        }

        private void Error(string error) => Errors.Add(error);

        private bool ValidateValueSemanticType(Node node)
        {
            if (node.SemanticType != SemanticType.Value) {
                Error($"Expression '{node}' is not valid in the current context.");
                return false;
            }

            return true;
        }

        private bool ValidateValueSemanticType(IEnumerable<Node> nodes)
        {
            bool valid = true;

            foreach (Node node in nodes) {
                valid &= ValidateValueSemanticType(node);
            }

            return valid;
        }
    }
}
