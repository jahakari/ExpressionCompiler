using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Nodes
{
    public class DayFunctionNode : Node
    {
        public DayFunctionNode(Node date)
        {
            Date = date;
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.Integer;

        public Node Date { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitDay(this);

        public override string ToString() => $"DAY({Date})";
    }
}
