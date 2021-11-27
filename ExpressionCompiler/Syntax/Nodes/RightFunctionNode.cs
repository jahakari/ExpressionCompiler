using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class RightFunctionNode : Node
    {
        public RightFunctionNode(Node text, Node count)
        {
            Text = text;
            Count = count;
        }

        public override NodeType NodeType => NodeType.Function;
        public override NodeValueType ValueType => NodeValueType.String;
        public override SemanticType SemanticType => SemanticType.Value;

        public Node Text { get; }
        public Node Count { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitRight(this);

        public override string ToString() => $"RIGHT({Text}, {Count})";

        public RightFunctionNode Update(Node text, Node count)
        {
            if (text == Text && count == Count) {
                return this;
            }

            return new RightFunctionNode(text, count);
        }
    }
}
