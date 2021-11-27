using ExpressionCompiler.Visitors;
using System;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class DateFunctionNode : Node
    {
        public DateFunctionNode(Node year, Node month, Node day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public override NodeType NodeType => NodeType.Function;
        public override NodeValueType ValueType => NodeValueType.Date;
        public override SemanticType SemanticType => SemanticType.Value;

        public Node Year { get; }
        public Node Month { get; }
        public Node Day { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitDate(this);

        public override string ToString() => $"DATE({Year}, {Month}, {Day})";

        public DateFunctionNode Update(Node year, Node month, Node day)
        {
            if (year == Year && month == Month && day == Day) {
                return this;
            }

            return new DateFunctionNode(year, month, day);
        }
    }
}
