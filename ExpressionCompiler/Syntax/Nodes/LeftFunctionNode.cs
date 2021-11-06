﻿using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class LeftFunctionNode : Node
    {
        public LeftFunctionNode(Node text, Node count)
        {
            Text = text;
            Count = count;
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.String;

        public Node Text { get; }
        public Node Count { get; }

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitLeft(this);

        public override string ToString() => $"LEFT({Text}, {Count})";
    }
}
