using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class CStringFunctionNode : Node
    {
        public CStringFunctionNode(Node argument)
        {
            Argument = argument;
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.String;

        public Node Argument { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitCString(this);

        public override string ToString() => $"CSTRING({Argument})";

        public CStringFunctionNode Update(Node argument)
        {
            if (argument == Argument) {
                return this;
            }

            return new CStringFunctionNode(argument);
        }
    }
}
