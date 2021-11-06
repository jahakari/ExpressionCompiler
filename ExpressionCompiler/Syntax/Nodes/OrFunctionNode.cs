using ExpressionCompiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Nodes
{
    public class OrFunctionNode : Node
    {
        public OrFunctionNode(IEnumerable<Node> arguments)
        {
            Arguments = arguments.ToList();
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.Boolean;

        public List<Node> Arguments { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitOr(this);

        public override string ToString() => $"OR({string.Join(", ", Arguments)})";
    }
}
