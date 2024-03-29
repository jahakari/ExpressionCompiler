﻿using ExpressionCompiler.Visitors;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class AndFunctionNode : Node
    {
        public AndFunctionNode(IEnumerable<Node> arguments)
        {
            Arguments = arguments.ToList();
        }

        public override NodeType NodeType => NodeType.Function;
        public override NodeValueType ValueType => NodeValueType.Boolean;
        public override SemanticType SemanticType => SemanticType.Value;

        public List<Node> Arguments { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitAnd(this);

        public override string ToString() => $"AND({string.Join(", ", Arguments)})";

        public AndFunctionNode Update(IEnumerable<Node> arguments)
        {
            if (arguments.SequenceEqual(Arguments)) {
                return this;
            }

            return new AndFunctionNode(arguments);
        }
    }
}
