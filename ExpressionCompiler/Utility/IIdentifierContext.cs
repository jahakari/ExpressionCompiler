using ExpressionCompiler.Syntax.Nodes;

namespace ExpressionCompiler.Utility
{
    public interface IIdentifierContext
    {
        bool IsIdentifier(string value);
        NodeValueType GetValueType(string value);
    }
}
