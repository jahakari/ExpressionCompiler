using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class IfFunctionNode : Node
    {
        public IfFunctionNode(Node condition, Node ifTrue, Node ifFalse)
        {
            Condition = condition;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => IfTrue.ValueType | IfFalse.ValueType;

        public Node Condition { get; }
        public Node IfTrue { get; }
        public Node IfFalse { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitIf(this);

        public override string ToString()
            => $"IF({Condition}, {IfTrue}, {IfFalse})";
    }
}
