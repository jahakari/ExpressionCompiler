using ExpressionCompiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class OrFunctionNode : Node, IFunctionNode
    {
        public OrFunctionNode(IEnumerable<Node> arguments)
        {
            Arguments = arguments.ToList();
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.Boolean;

        public List<Node> Arguments { get; }

        public string FunctionName => "OR";

        public override Node Accept(INodeVisitor visitor)
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
