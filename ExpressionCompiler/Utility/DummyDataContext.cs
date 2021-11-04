using System;

namespace ExpressionCompiler.Utility
{
    public class DummyDataContext : IIdentifierDataContext
    {
        public DateTime GetDate(string identifier)
        {
            return new DateTime(1988, 1, 19);
        }

        public decimal GetDecimal(string identifier)
        {
            return 33.33m;
        }

        public int GetInteger(string identifier)
        {
            return 12;
        }

        public string GetString(string identifier)
        {
            return "Shibouya!";
        }
    }
}
