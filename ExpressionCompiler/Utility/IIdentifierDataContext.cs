using System;

namespace ExpressionCompiler.Utility
{
    public interface IIdentifierDataContext
    {
        int GetInteger(string identifier);
        decimal GetDecimal(string identifier);
        DateTime GetDate(string identifier);
        string GetString(string identifier);
    }
}
