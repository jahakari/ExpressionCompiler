using ExpressionCompiler.Syntax.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Visitors
{
    public abstract class NodeVisitor
    {
        public virtual Node VisitAbs(AbsFunctionNode node)
            => node.Update(node.Argument.Accept(this));

        public virtual Node VisitAnd(AndFunctionNode node)
        {
            IEnumerable<Node> arguments = node.Arguments.Select(a => a.Accept(this));
            return node.Update(arguments);
        }

        public virtual Node VisitBinary(BinaryExpressionNode node)
        {
            Node left = node.Left.Accept(this);
            Node right = node.Right.Accept(this);

            return node.Update(left, right);
        }

        public virtual Node VisitCInt(CIntFunctionNode node)
            => node.Update(node.Argument.Accept(this));

        public virtual Node VisitComplex(ComplexExpressionNode node)
        {
            IEnumerable<Node> nodes = node.Nodes.Select(n => n.Accept(this));
            return node.Update(nodes);
        }

        public virtual Node VisitCString(CStringFunctionNode node)
            => node.Update(node.Argument.Accept(this));

        public virtual Node VisitDate(DateFunctionNode node)
        {
            Node year = node.Year.Accept(this);
            Node month = node.Month.Accept(this);
            Node day = node.Day.Accept(this);

            return node.Update(year, month, day);
        }

        public virtual Node VisitDay(DayFunctionNode node)
            => node.Update(node.Date.Accept(this));

        public virtual Node VisitGroup(GroupNode node)
            => node.Update(node.Inner.Accept(this));

        public virtual Node VisitIdentifier(IdentifierNode node)
            => node;

        public virtual Node VisitIf(IfFunctionNode node)
        {
            Node condition = node.Condition.Accept(this);
            Node ifTrue = node.IfTrue.Accept(this);
            Node ifFalse = node.IfFalse.Accept(this);

            return node.Update(condition, ifTrue, ifFalse);
        }

        public virtual Node VisitLeft(LeftFunctionNode node)
        {
            Node text = node.Text.Accept(this);
            Node count = node.Count.Accept(this);

            return node.Update(text, count);
        }

        public virtual Node VisitLiteral(LiteralValueNode node)
            => node;

        public virtual Node VisitMonth(MonthFunctionNode node)
            => node.Update(node.Date.Accept(this));

        public virtual Node VisitNegation(NegationNode node)
            => node.Update(node.Operand.Accept(this));

        public virtual Node VisitOr(OrFunctionNode node)
        {
            IEnumerable<Node> arguments = node.Arguments.Select(a => a.Accept(this));
            return node.Update(arguments);
        }

        public virtual Node VisitRight(RightFunctionNode node)
        {
            Node text = node.Text.Accept(this);
            Node count = node.Count.Accept(this);

            return node.Update(text, count);
        }

        public virtual Node VisitToday(TodayFunctionNode node)
            => node;

        public virtual Node VisitYear(YearFunctionNode node)
            => node.Update(node.Date.Accept(this));
    }
}
