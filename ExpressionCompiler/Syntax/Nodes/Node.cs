using ExpressionCompiler.Compilation;
using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Nodes
{
    public abstract class Node
    {
        public abstract NodeType NodeType { get; }
        public abstract NodeValueType ValueType { get; }

        public abstract Node Accept(INodeVisitor visitor);
        public abstract override string ToString();
    }
}
