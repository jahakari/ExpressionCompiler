using ExpressionCompiler.Utility;
using ExpressionCompiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class MaxFunctionNode : Node
    {
        public MaxFunctionNode(IEnumerable<Node> arguments)
        {
            Arguments = arguments.ToList();
            ValueType = NodeUtils.GetCommonDataType(arguments);
        }

        public override NodeType NodeType => NodeType.Function;
        public override NodeValueType ValueType { get; }
        public override SemanticType SemanticType => SemanticType.Value;

        public List<Node> Arguments { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitMax(this);

        public override string ToString()
            => $"MAX({string.Join(", ", Arguments)})";

        public MaxFunctionNode Update(IEnumerable<Node> arguments)
        {
            if (arguments.SequenceEqual(Arguments)) {
                return this;
            }

            return new MaxFunctionNode(arguments);
        }
    }
}
