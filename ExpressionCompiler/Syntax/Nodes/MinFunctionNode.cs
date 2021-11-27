using ExpressionCompiler.Utility;
using ExpressionCompiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class MinFunctionNode : Node
    {
        public MinFunctionNode(IEnumerable<Node> arguments)
        {
            Arguments = arguments.ToList();
            ValueType = NodeUtils.GetCommonDataType(arguments);
        }

        public override NodeType NodeType => NodeType.Function;
        public override NodeValueType ValueType { get; }
        public override SemanticType SemanticType => SemanticType.Value;

        public List<Node> Arguments { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitMin(this);

        public override string ToString()
            => $"MIN({string.Join(", ", Arguments)})";

        public MinFunctionNode Update(IEnumerable<Node> arguments)
        {
            if (arguments.SequenceEqual(Arguments)) {
                return this;
            }

            return new MinFunctionNode(arguments);
        }
    }
}
