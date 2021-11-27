using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class RelationalNode : Node
    {
        public RelationalNode(BinaryOperatorNode @operator, Node operand)
        {
            Operator = @operator;
            Operand = operand;
        }

        public override NodeType NodeType => NodeType.Relational;
        public override NodeValueType ValueType => Operand.ValueType;
        public override SemanticType SemanticType => SemanticType.NonValue;

        public BinaryOperatorNode Operator { get; }
        public Node Operand { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitRelational(this);

        public override string ToString() => $"{Operator} {Operand}";

        public RelationalNode Update(BinaryOperatorNode @operator, Node operand)
        {
            if (@operator == Operator && operand == Operand) {
                return this;
            }

            return new RelationalNode(@operator, operand);
        }
    }
}
