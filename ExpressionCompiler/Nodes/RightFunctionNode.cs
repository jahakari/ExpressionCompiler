﻿using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Nodes
{
    public class RightFunctionNode : Node
    {
        public RightFunctionNode(Node text, Node count)
        {
            Text = text;
            Count = count;
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.String;

        public Node Text { get; }
        public Node Count { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitRight(this);

        public override string ToString() => $"RIGHT({Text}, {Count})";
    }
}
