using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;

namespace ExpressionCompiler.Visitors
{
    public class NodeRewritingVisitor : NodeVisitor
    {
        public override Node VisitBinary(BinaryExpressionNode node)
        {
            node = (BinaryExpressionNode)base.VisitBinary(node);

            if (node.Left is LiteralValueNode && node.Right is LiteralValueNode) {
                OperatorType opType = node.Operator.OperatorType;

                return (node.Left, node.Right) switch
                {
                    (LiteralValueNode<int> i1, LiteralValueNode<int> i2)         => NodeUtils.CreateConstant(i1.Value, i2.Value, opType),
                    (LiteralValueNode<decimal> d1, LiteralValueNode<decimal> d2) => NodeUtils.CreateConstant(d1.Value, d2.Value, opType),
                    (LiteralValueNode<decimal> d, LiteralValueNode<int> i)       => NodeUtils.CreateConstant(d.Value, i.Value, opType),
                    (LiteralValueNode<int> i, LiteralValueNode<decimal> d)       => NodeUtils.CreateConstant(d.Value, i.Value, opType),
                    _                                                            => node
                };
            }

            return node;
        }


    }
}
