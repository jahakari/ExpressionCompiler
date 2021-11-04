using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Nodes
{
    public class MonthFunctionNode : Node
    {
        public MonthFunctionNode(Node date)
        {
            Date = date;
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.Integer;

        public Node Date { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitMonth(this);

        public override string ToString() => $"MONTH({Date})";
    }
}
