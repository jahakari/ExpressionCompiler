using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Visitors
{
    public class NodeRewritingVisitor : NodeVisitor
    {
        public override Node VisitBinary(BinaryExpressionNode node)
        {
            Node left = node.Left.Accept(this);
            Node right = node.Right.Accept(this);

            if (left is LiteralValueNode && right is LiteralValueNode) {
                OperatorType opType = node.Operator.OperatorType;

                return (left, right) switch
                {
                    (LiteralValueNode<int> i1, LiteralValueNode<int> i2)         => NodeUtils.CreateConstant(i1.Value, i2.Value, opType),
                    (LiteralValueNode<decimal> d1, LiteralValueNode<decimal> d2) => NodeUtils.CreateConstant(d1.Value, d2.Value, opType),
                    (LiteralValueNode<decimal> d, LiteralValueNode<int> i)       => NodeUtils.CreateConstant(d.Value, i.Value, opType),
                    (LiteralValueNode<int> i, LiteralValueNode<decimal> d)       => NodeUtils.CreateConstant(i.Value, d.Value, opType),
                    _                                                            => node
                };
            }

            return node.Update(left, right);
        }

        public override Node VisitComplex(ComplexExpressionNode node)
        {
            List<Node> nodes = node.Nodes.Select(n => n.Accept(this)).ToList();

            if (nodes.Count == 1) {
                return nodes[0];
            }

            if (nodes.Count == 3) {
                var newNode = new BinaryExpressionNode(nodes[0], (BinaryOperatorNode)nodes[1], nodes[2]);
                return VisitBinary(newNode);
            }

            int booleanOpIndex = nodes.FindIndex(n => n is BinaryOperatorNode b && NodeUtils.IsBooleanOperator(b.OperatorType));

            if (booleanOpIndex > 0) {
                IEnumerable<Node> leftNodes = nodes.Take(booleanOpIndex);
                IEnumerable<Node> rightNodes = nodes.Skip(booleanOpIndex + 1);
                BinaryOperatorNode op = (BinaryOperatorNode)nodes[booleanOpIndex];

                var newNode = new BinaryExpressionNode
                (
                    new ComplexExpressionNode(leftNodes),
                    op,
                    new ComplexExpressionNode(rightNodes)
                );

                return VisitBinary(newNode);
            }

            IEnumerable<Node> rewrittenNodes = RewriteOperationsOfMathSubType(nodes, OperatorType.Exponent);
            rewrittenNodes = RewriteOperationsOfMathSubType(rewrittenNodes, OperatorType.MultiplyOrDivide);
            Node[] finalNodes = RewriteOperationsOfMathSubType(rewrittenNodes, OperatorType.AddOrSubtract).ToArray();

            BinaryExpressionNode finalNode;

            if (finalNodes.Length == 1) {
                finalNode = (BinaryExpressionNode)finalNodes[0];
            } else {
                finalNode = new BinaryExpressionNode(finalNodes[0], (BinaryOperatorNode)finalNodes[1], finalNodes[2]);
            }

            return VisitBinary(finalNode);

            IEnumerable<Node> RewriteOperationsOfMathSubType(IEnumerable<Node> nodes, OperatorType subType)
            {
                var output = new Stack<Node>();
                var input = new Queue<Node>(nodes);

                while (input.TryDequeue(out Node node)) {
                    if (node is BinaryOperatorNode op && op.MathOperatorSubType == subType) {
                        Node left = output.Pop();
                        Node right = input.Dequeue();
                        output.Push(new BinaryExpressionNode(left, op, right));
                    } else {
                        output.Push(node);
                    }
                }

                return output.Reverse();
            }
        }
    }
}
