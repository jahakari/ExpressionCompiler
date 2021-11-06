using ExpressionCompiler.Visitors;
using System;

namespace ExpressionCompiler.Nodes
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

        public Node Year { get; }
        public Node Month { get; }
        public Node Day { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitDate(this);

        public override string ToString() => $"DATE({Year}, {Month}, {Day})";
    }
}
