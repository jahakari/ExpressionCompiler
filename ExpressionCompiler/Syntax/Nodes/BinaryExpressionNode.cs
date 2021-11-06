using ExpressionCompiler.Compilation;
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
                if (Operator.OperatorSubType == OperatorType.Boolean) {
                    return NodeValueType.Boolean;
                }

                return Left.ValueType | Right.ValueType;
            }
        }

        public Node Left { get; }
        public BinaryOperatorNode Operator { get; }
        public Node Right { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitBinary(this);

        public override string ToString() => $"{Left} {Operator} {Right}";
    }
}
