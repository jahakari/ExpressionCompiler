using ExpressionCompiler.Syntax.Nodes;
using System;

namespace ExpressionCompiler.Utility
{
    public class DefaultIdentifierContext : IIdentifierContext
    {
        public NodeValueType GetValueType(string value)
        {
            throw new NotImplementedException();
        }

        public bool IsIdentifier(string value) => false;
    }
}
