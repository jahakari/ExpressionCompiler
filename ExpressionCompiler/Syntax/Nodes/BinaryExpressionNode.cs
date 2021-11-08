using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class BinaryExpressionNode : Node
    {
        public BinaryExpressionNode(Node left, BinaryOperatorNode @operator, Node right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public override NodeType NodeType => NodeType.BinaryExpression;
        public override NodeValueType ValueType
        {
            get
            {
                if (Operator.OperatorCategory == OperatorType.Boolean) {
                    return NodeValueType.Boolean;
                }

                return Left.ValueType | Right.ValueType;
            }
        }

        public Node Left { get; }
        public BinaryOperatorNode Operator { get; }
        public Node Right { get; }

        public override Node Accept(NodeVisitor visitor) => visitor.VisitBinary(this);

        public override string ToString() => $"{Left} {Operator} {Right}";

        public BinaryExpressionNode Update(Node left, Node right)
        {
            if (left == Left && right == Right) {
                return this;
            }

            return new BinaryExpressionNode(left, Operator, right);
        }
    }
}
