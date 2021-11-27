using ExpressionCompiler.Visitors;
using System;

namespace ExpressionCompiler.Syntax.Nodes
{
    public sealed class LiteralValueNode<T> : LiteralValueNode
    {
        private readonly NodeValueType valueType;

        public LiteralValueNode(T value)
        {
            valueType = DetermineValueTypeOfT();
            Value = value;
        }

        public override NodeValueType ValueType => valueType;
        public override NodeType NodeType => NodeType.LiteralValue;
        public override SemanticType SemanticType => SemanticType.Value;

        public T Value { get; }

        private NodeValueType DetermineValueTypeOfT()
        {
            return typeof(T).FullName switch
            {
                "System.Boolean"  => NodeValueType.Boolean,
                "System.Int32"    => NodeValueType.Integer,
                "System.Decimal"  => NodeValueType.Decimal,
                "System.DateTime" => NodeValueType.Date,
                "System.String"   => NodeValueType.String,
                _                 => throw new InvalidOperationException($"Literal values of type '{typeof(T)}' are not supported.")
            };
        }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitLiteral(this);

        public override string ToString()
        {
            return Value switch
            {
                bool b     => b.ToString().ToUpper(),
                DateTime d => d.ToString("MM/dd/yyyy"),
                string s   => $"\"{s}\"",
                _          => Value.ToString()
            };
        }
    }
}
