using ExpressionCompiler.Tokenizing;

namespace ExpressionCompiler.Utility
{
    public static class ParserUtils
    {
        public static bool IsOperator(string value)
        {
            switch (value) {
                case "=":
                case "<":
                case ">":
                case "<=":
                case ">=":
                case "<>":
                case "!=":
                case "+":
                case "-":
                case "*":
                case "/":
                case "%":
                case "^":
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsExpressionTerminator(Token t) => t.Kind is TokenKind.Comma or TokenKind.RParen;
    }
}
