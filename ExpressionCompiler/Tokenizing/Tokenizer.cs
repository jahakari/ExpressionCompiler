using ExpressionCompiler.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ExpressionCompiler.Tokenizing.TokenKind;

namespace ExpressionCompiler.Tokenizing
{
    public class Tokenizer
    {
        private readonly Window<char> window;
        private StringBuilder stringBuilder = new();

        public Tokenizer(string input) => window = new Window<char>(input);

        public IEnumerable<Token> Tokenize()
        {
            while (window.HasItem) {
                yield return GetToken();
            }
        }

        private Token GetToken()
        {
            char c = window.Current;
            TokenKind kind;

            switch (c) {
                case '(':
                    kind = LParen;
                    break;
                    
                case ')':
                    kind = RParen;
                    break;
                    
                case '<':
                    kind = LAngle;
                    break;
                    
                case '>':
                    kind = RAngle;
                    break;
                    
                case '+':
                    kind = Plus;
                    break;
                    
                case '-':
                    if (char.IsNumber(window.Next)) {
                        window.Advance();
                        return ScanNumber();
                    }

                    kind = Hypen;
                    break;
                    
                case '*':
                    kind = Star;
                    break;
                    
                case '/':
                    kind = Slash;
                    break;
                    
                case '%':
                    kind = Percent;
                    break;
                    
                case '=':
                    kind = Equal;
                    break;
                    
                case '^':
                    kind = Caret;
                    break;
                    
                case '!':
                    kind = Exclamation;
                    break;
                    
                case ',':
                    kind = Comma;
                    break;

                case ' ':
                    kind = Space;
                    break;

                case '\r':
                    if (window.Peek() == '\n') {
                        window.Advance();
                    }

                    goto case '\n';
                    
                case '\n':
                    kind = Newline;
                    break;
                    
                case '\t':
                    kind = Tab;
                    break;
                    
                case '"':
                    window.Advance();
                    return ScanString();

                default:
                    if (char.IsNumber(c)) {
                        return ScanNumber();
                    }

                    if (IsWordCharacter(c, '_')) {
                        return ScanWord();
                    }

                    return new Token(Unknown, c);
            }

            window.Advance();
            return new Token(kind, c);
        }

        private Token ScanString()
        {
            stringBuilder.Clear();

            while (window.HasItem) {
                char c = window.Current;

                if (c == '"' && window.Previous != '\\') {
                    window.Advance();
                    break;
                }

                stringBuilder.Append(c);
                window.Advance();
            }

            return new Token(TokenKind.String, stringBuilder.ToString());
        }

        private Token ScanNumber()
        {
            stringBuilder.Clear();
            TokenKind kind = Integer;

            if (window.Previous == '-') {
                stringBuilder.Append('-');
            }

            while (window.HasItem) {
                char c = window.Current;

                if (c == '.') {
                    kind = TokenKind.Decimal;
                    stringBuilder.Append('.');
                    window.Advance();

                    continue;
                }

                if (!char.IsNumber(c)) {
                    break;
                }

                stringBuilder.Append(c);
                window.Advance();
            }

            return new Token(kind, stringBuilder.ToString());
        }

        private Token ScanWord()
        {
            stringBuilder.Clear();
            char[] additionalChars = { '_', '.' };

            while (window.HasItem) {
                char c = window.Current;

                if (!IsWordCharacter(c, additionalChars)) {
                    break;
                }

                stringBuilder.Append(c);
                window.Advance();
            }

            return new Token(Word, stringBuilder.ToString());
        }

        private bool IsWordCharacter(char c, params char[] additionalChars)
            => char.IsNumber(c) || char.IsLetter(c) || additionalChars.Contains(c);
    }
}