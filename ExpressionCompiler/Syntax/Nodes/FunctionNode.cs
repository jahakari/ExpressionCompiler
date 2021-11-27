using ExpressionCompiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class FunctionNode : Node
    {
        public FunctionNode(string name, IEnumerable<Node> arguments)
        {
            Name = name;
            Arguments = arguments.ToList();
        }

        public override NodeType NodeType => NodeType.Function;
        public override NodeValueType ValueType => NodeValueType.None;
        public override SemanticType SemanticType => SemanticType.Value;

        public string Name { get; }
        public List<Node> Arguments { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitFunction(this);

        public override string ToString()
            => $"{Name}({string.Join(", ", Arguments)})";

        public FunctionNode Update(IEnumerable<Node> arguments)
        {
            if (arguments.SequenceEqual(Arguments)) {
                return this;
            }

            return new FunctionNode(Name, arguments);
        }
    }
}
