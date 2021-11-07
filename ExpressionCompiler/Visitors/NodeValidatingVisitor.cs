using ExpressionCompiler.Syntax.Nodes;
using System;
using System.Collections.Generic;

namespace ExpressionCompiler.Visitors
{
    public class NodeValidatingVisitor : NodeVisitor
    {
        public List<string> Errors { get; } = new();

        public override Node VisitAbs(AbsFunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override Node VisitAnd(AndFunctionNode node)
        {
            if (node.Arguments.Count < 2) {
                Error($"Function '{node}' requires at least 2 arguments.");
            }

            foreach (Node n in node.Arguments) {
                n.Accept(this);

                if (n.ValueType != NodeValueType.Boolean) {
                    Error($"Argument '{n}' of AND function is invalid; each argument must be a Boolean expression.");
                }
            }

            return node;
        }

        public override Node VisitBinary(BinaryExpressionNode node)
        {
            return node;
        }

        public override Node VisitCInt(CIntFunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override Node VisitCString(CStringFunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override Node VisitDate(DateFunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override Node VisitDay(DayFunctionNode node)
        {
            node.Date.Accept(this);

            if (node.Date.ValueType != NodeValueType.Date) {
                Error($"Function {node} is invalid; argument must be a date expression.");
            }

            return node;
        }

        public override Node VisitGroup(GroupNode node)
        {
            throw new NotImplementedException();
        }

        public override Node VisitIf(IfFunctionNode node)
        {
            node.Condition.Accept(this);
            node.IfTrue.Accept(this);
            node.IfFalse.Accept(this);

            if (node.Condition.ValueType != NodeValueType.Boolean) {
                Error($"Argument '{node.Condition}' of IF function is invalid; argument must be a Boolean expression.");
            }

            NodeValueType returnType = node.IfTrue.ValueType | node.IfFalse.ValueType;

            if (returnType == NodeValueType.None || !Enum.IsDefined(returnType)) {
                Error($"IF function return arguments '{node.IfTrue}' and '{node.IfFalse}' are not supported, or do not have compatible data types.");
            }

            return node;
        }

        public override Node VisitLeft(LeftFunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override Node VisitMonth(MonthFunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override Node VisitNegation(NegationNode node)
        {
            throw new NotImplementedException();
        }

        public override Node VisitOr(OrFunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override Node VisitRight(RightFunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override Node VisitYear(YearFunctionNode node)
        {
            throw new NotImplementedException();
        }

        private void Error(string error) => Errors.Add(error);
    }
}
