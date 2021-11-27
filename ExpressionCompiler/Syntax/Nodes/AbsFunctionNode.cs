using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class AbsFunctionNode : Node
    {
        public AbsFunctionNode(Node argument)
        {
            Argument = argument;
        }

        public override NodeType NodeType => NodeType.Function;
        public override NodeValueType ValueType => Argument.ValueType;
        public override SemanticType SemanticType => SemanticType.Value;

        public Node Argument { get; }

        public override Node Accept(NodeVisitor visitor)
             => visitor.VisitAbs(this);

        public override string ToString() => $"ABS({Argument})";

        public AbsFunctionNode Update(Node argument)
        {
            if (argument == Argument) {
                return this;
            }

            return new AbsFunctionNode(argument);
        }
    }
}
