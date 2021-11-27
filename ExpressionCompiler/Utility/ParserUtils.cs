using ExpressionCompiler.Tokenizing;
using static ExpressionCompiler.Tokenizing.TokenKind;

namespace ExpressionCompiler.Utility
{
    public static class ParserUtils
    {
        public static bool IsOperator(Token t1, Token t2)
        {
            switch ((t1.Kind, t2.Kind)) {
                case (LAngle, Equal):
                case (RAngle, Equal):
                case (LAngle, RAngle):
                case (Exclamation, Equal):
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsOperator(Token token)
        {
            switch (token.Kind) {
                case LAngle:
                case RAngle:
                case Plus:
                case Hypen:
                case Star:
                case Slash:
                case Percent:
                case Equal:
                case Caret:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsExpressionTerminator(Token t) => t.Kind is TokenKind.Comma or TokenKind.RParen;
    }
}
