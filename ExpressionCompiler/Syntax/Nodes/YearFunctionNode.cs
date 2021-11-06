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

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitYear(this);

        public override string ToString() => $"YEAR({Date})";
    }
}
