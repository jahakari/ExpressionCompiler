using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Nodes
{
    public class CIntFunctionNode : Node
    {
        public CIntFunctionNode(Node argument)
        {
            Argument = argument;
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.Integer;

        public Node Argument { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitCInt(this);

        public override string ToString() => $"CINT({Argument})";
    }
}
