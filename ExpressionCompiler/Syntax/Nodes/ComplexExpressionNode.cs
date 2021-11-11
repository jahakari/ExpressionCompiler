using ExpressionCompiler.Utility;
using ExpressionCompiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class ComplexExpressionNode : Node
    {
        public ComplexExpressionNode(IEnumerable<Node> nodes)
        {
            Nodes = nodes.ToList();

            if (nodes.Any(n => n is BinaryOperatorNode op && op.OperatorCategory == OperatorType.Boolean)) {
                ValueType = NodeValueType.Boolean;
            } else {
                ValueType = NodeUtils.GetCommonDataType(nodes.EveryNth(2));
            }
        }

        public override NodeType NodeType => NodeType.Complex;

        public override NodeValueType ValueType { get; }

        public List<Node> Nodes { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitComplex(this);

        public override string ToString() => string.Join(' ', Nodes);

        public ComplexExpressionNode Update(IEnumerable<Node> nodes)
        {
            if (nodes.SequenceEqual(Nodes)) {
                return this;
            }

            return new ComplexExpressionNode(nodes);
        }
    }
}
