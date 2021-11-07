using ExpressionCompiler.Visitors;
using System;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class DateFunctionNode : Node, IFunctionNode
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

        public string FunctionName => "DATE";

        public override Node Accept(INodeVisitor visitor)
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
