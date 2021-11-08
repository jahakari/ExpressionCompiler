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
            IEnumerable<Token> tokens = new Tokenizer(input, skipWhitespace: true).Tokenize();
            window = new Window<Token>(tokens);

            if (TryParseNode(parseComplex: true, out Node node, out string error)) {
                if (window.HasItem) {
                    return ParseResult.Failure($"Unexpected token '{window.Current.Value}' at end of expression");
                }

                return ParseResult.Success(node);
            }

            return ParseResult.Failure(error);
        }

        private bool TryParseNode(bool parseComplex, out Node node, out string error)
        {
            node = null;
            error = null;

            if (!window.HasItem) {
                error = "Unexpected end of expression was reached";
                return false;
            }

            Token t = window.Current;

            switch (t.Kind) {
                case TokenKind.LParen:
                    window.Advance();

                    if (!TryParseNode(parseComplex: true, out node, out error)) {
                        return false;
                    }

                    if (!window.HasItem) {
                        error = "Unexpected end of expression was reached while parsing group expression; expected ')'";
                        return false;
                    }

                    if (window.Current.Kind != TokenKind.RParen) {
                        error = $"Unexpected token '{t.Value}' at end of Group expression; expected ')'";
                        return false;
                    }

                    node = new GroupNode(node);

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
                    node = null;
                    error = $"Unexpected token '{t.Value}' was encountered by the parser.";
                    break;
            }

            window.Advance();

            if (parseComplex && ParserUtils.IsOperator(window.Current.Value)) {
                if (TryParseComplexExpression(node, out ComplexExpressionNode complex, out error)) {
                    node = complex;
                    return true;
                }

                return false;
            }

            return true;
        }

        private bool TryParseComplexExpression(Node firstNode, out ComplexExpressionNode complex, out string error)
        {
            complex = null;
            error = null;

            var nodes = new List<Node> { firstNode };

            while (window.HasItem && !ParserUtils.IsExpressionTerminator(window.Current)) {
                if (!TryParseOperator(out BinaryOperatorNode op, out error)) {
                    return false;
                }

                nodes.Add(op);

                if (!TryParseNode(parseComplex: false, out Node n, out error)) {
                    return false;
                }

                nodes.Add(n);
            }

            complex = new ComplexExpressionNode(nodes);
            return true;
        }

        private bool TryParseOperator(out BinaryOperatorNode op, out string error)
        {
            op = null;
            error = null;

            string opValue = window.Current.Value;

            if (!ParserUtils.IsOperator(opValue)) {
                error = $"Unexpected token '{opValue}' was encountered by the parser; expected operator.";
                return false;
            }

            window.Advance();

            if (ParserUtils.IsOperator(window.Current.Value)) {
                opValue += window.Current.Value;

                if (!ParserUtils.IsOperator(opValue)) {
                    error = $"Invalid compound operator '{opValue}' was encountered by the parser.";
                    return false;
                }

                window.Advance();
            }

            op = new BinaryOperatorNode(opValue);
            return true;
        }
    }
}