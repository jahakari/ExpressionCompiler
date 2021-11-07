using ExpressionCompiler.Visitors;
using System;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class IdentifierNode : Node
    {
        private readonly NodeValueType valueType;

        public IdentifierNode(string identifier, NodeValueType valueType)
        {
            Identifier = identifier;
            this.valueType = valueType;
        }

        public override NodeType NodeType => NodeType.Identifier;

        public override NodeValueType ValueType => valueType;

        public string Identifier { get; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitIdentifier(this);

        public override string ToString() => Identifier;
    }
}
