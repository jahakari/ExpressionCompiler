using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class YearFunctionNode : Node
    {
        public YearFunctionNode(Node date)
        {
            Date = date;
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.Integer;

        public Node Date { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitYear(this);

        public override string ToString() => $"YEAR({Date})";

        public YearFunctionNode Update(Node date)
        {
            if (date == Date) {
                return this;
            }

            return new YearFunctionNode(date);
        }
    }
}
