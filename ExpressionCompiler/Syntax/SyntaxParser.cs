using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Tokenizing;
using ExpressionCompiler.Utility;
using System;
using System.Collections.Generic;

namespace ExpressionCompiler.Syntax
{
    public class SyntaxParser
    {
        private Window<Token> window;

        public ParseResult Parse(string input)
        {
            IEnumerable<Token> tokens = new Tokenizer(input).Tokenize();
            window = new Window<Token>(tokens);

            throw new NotImplementedException();
        }

        private bool TryParseNode(out Node node, out string error)
        {
            Token t = window.Current;

            switch (t.Kind) {
                case TokenKind.LParen:
                    window.Advance();

                    if (!TryParseNode(out node, out error)) {
                        return false;
                    }

                    if (window.Current.Kind != TokenKind.RParen) {
                        error = $"Unexpected token '{t.Value}' at end of Group expression; expected ')'";
                        return false;
                    }

                    node = new GroupNode(node);
                    window.Advance();

                    break;
                
                case TokenKind.Word:
                    switch (t.Value) {
                        case "TRUE":
                            node = new LiteralValueNode<bool>(true);
                            break;

                        case "FALSE":
                            node = new LiteralValueNode<bool>(false);
                            break;

                        default:
                            break;
                    }
                    break;
                case TokenKind.String:
                    if (DateTime.TryParse(t.Value, out DateTime date)) {
                        node = new LiteralValueNode<DateTime>(date);
                    } else {
                        node = new LiteralValueNode<string>(t.Value);
                    }

                    break;

                case TokenKind.Integer:
                    node = new LiteralValueNode<int>(int.Parse(t.Value));
                    break;

                case TokenKind.Decimal:
                    node = new LiteralValueNode<decimal>(decimal.Parse(t.Value));
                    break;

                default:
                    break;
            }

            throw new NotImplementedException();
        } 
    }
}
