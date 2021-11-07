using ExpressionCompiler.Visitors;
using System;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class GroupNode : Node
    {
        public GroupNode(Node inner)
        {
            Inner = inner;
        }

        public override NodeType NodeType => NodeType.Group;

        public override NodeValueType ValueType => Inner.ValueType;

        public Node Inner { get; }

        public override Node Accept(NodeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => $"({Inner})";

        public GroupNode Update(Node inner)
        {
            if (inner == Inner) {
                return this;
            }

            return new GroupNode(inner);
        }
    }
}
