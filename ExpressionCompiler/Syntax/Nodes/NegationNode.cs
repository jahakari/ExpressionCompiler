using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class NegationNode : Node
    {
        public NegationNode(Node operand)
        {
            Operand = operand;
        }

        public override NodeType NodeType => NodeType.Negation;

        public override NodeValueType ValueType => Operand.ValueType;

        public Node Operand { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitNegation(this);

        public override string ToString() => $"-{Operand}";

        public NegationNode Update(Node operand)
        {
            if (operand == Operand) {
                return this;
            }

            return new NegationNode(operand);
        }
    }
}
