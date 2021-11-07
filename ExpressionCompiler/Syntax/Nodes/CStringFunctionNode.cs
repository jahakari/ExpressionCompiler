using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class CStringFunctionNode : Node, IFunctionNode
    {
        public CStringFunctionNode(Node argument)
        {
            Argument = argument;
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.String;

        public Node Argument { get; }

        public string FunctionName => "CSTRING";

        public override Node Accept(INodeVisitor visitor)
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
