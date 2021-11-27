using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public abstract class Node
    {
        public abstract NodeType NodeType { get; }
        public abstract NodeValueType ValueType { get; }
        public abstract SemanticType SemanticType { get; }

        public abstract Node Accept(NodeVisitor visitor);
        public abstract override string ToString();
    }
}
