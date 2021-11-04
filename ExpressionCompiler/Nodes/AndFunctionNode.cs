using ExpressionCompiler.Visitors;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Nodes
{
    public class AndFunctionNode : Node
    {
        public AndFunctionNode(IEnumerable<Node> arguments)
        {
            Arguments = arguments.ToList();
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.Boolean;

        public List<Node> Arguments { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitAnd(this);

        public override string ToString() => $"AND({string.Join(", ", Arguments)})";
    }
}
