using ExpressionCompiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class OrFunctionNode : Node
    {
        public OrFunctionNode(IEnumerable<Node> arguments)
        {
            Arguments = arguments.ToList();
        }

        public override NodeType NodeType => NodeType.Function;
        public override NodeValueType ValueType => NodeValueType.Boolean;
        public override SemanticType SemanticType => SemanticType.Value;

        public List<Node> Arguments { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitOr(this);

        public override string ToString() => $"OR({string.Join(", ", Arguments)})";

        public OrFunctionNode Update(IEnumerable<Node> arguments)
        {
            if (arguments.SequenceEqual(Arguments)) {
                return this;
            }

            return new OrFunctionNode(arguments);
        }
    }
}
