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
        private readonly IFunctionContext functionContext = FunctionContext.Instance;
        private readonly IIdentifierContext identifierContext;

        public SyntaxParser(IIdentifierContext identifierContext)
        {
            this.identifierContext = identifierContext;
        }

        public SyntaxParser() : this(new DefaultIdentifierContext()) { }

        public ParseResult Parse(string input)
        {
            IEnumerable<Token> tokens = new Tokenizer(input, skipWhitespace: true).Tokenize();
            window = new Window<Token>(tokens);

            if (ParseNode(parseComplex: true, out Node node, out string error)) {
                if (window.HasItem) {
                    return ParseResult.Failure($"Unexpected token '{window.Current.Value}' at end of expression");
                }

                return ParseResult.Success(node);
            }

            return ParseResult.Failure(error);
        }

        private bool ParseNode(bool parseComplex, out Node node, out string error)
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

                    if (!ParseNode(parseComplex: true, out node, out error)) {
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

                case TokenKind.Hypen:
                    window.Advance();

                    if (!ParseNode(parseComplex: false, out node, out error)) {
                        return false;
                    }

                    node = new NegationNode(node);
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
                            if (functionContext.IsFunction(t.Value)) {
                                window.Advance();

                                if (!ParseFunctionArguments(t.Value, out List<Node> arguments, out error)) {
                                    return false;
                                }

                                node = new FunctionNode(t.Value, arguments);
                                break;
                            }

                            if (identifierContext.IsIdentifier(t.Value)) {
                                NodeValueType type = identifierContext.GetValueType(t.Value);
                                node = new IdentifierNode(t.Value, type);
                            }

                            break;
                    }
                    break;

                case TokenKind.String:
                    if (DateTime.TryParse(t.Value, out DateTime date)) {
                        node = new LiteralValueNode<DateTime>(date);
                    }
                    else {
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
                    if (TryParseRelationalExpression(out node)) {
                        window.Advance();
                        return true;
                    }

                    error = $"Unexpected token '{t}' was encountered by the parser.";
                    return false;
            }

            window.Advance();

            if (parseComplex && TryParseOperator(out BinaryOperatorNode op)) {
                if (ParseComplexExpression(node, op, out node, out error)) {
                    return true;
                }

                return false;
            }

            return true;
        }

        private bool TryParseRelationalExpression(out Node node)
        {
            node = null;

            if (!TryParseOperator(out BinaryOperatorNode op)) {
                return false;
            }

            if (op.OperatorCategory != OperatorType.Boolean) {
                return false;
            }

            if (!ParseNode(parseComplex: true, out Node n, out _)) {
                return false;
            }

            node = new RelationalNode(op, n);
            return true;
        }

        private bool ParseComplexExpression(Node firstNode, BinaryOperatorNode firstOperator, out Node complex, out string error)
        {
            complex = null;
            var nodes = new List<Node> { firstNode, firstOperator };

            if (!ParseNode(parseComplex: false, out Node node, out error)) {
                return false;
            }

            nodes.Add(node);

            while (window.HasItem && !ParserUtils.IsExpressionTerminator(window.Current)) {
                if (!ParseOperator(out BinaryOperatorNode op, out error)) {
                    return false;
                }

                nodes.Add(op);

                if (!ParseNode(parseComplex: false, out Node n, out error)) {
                    return false;
                }

                nodes.Add(n);
            }

            complex = new ComplexExpressionNode(nodes);
            return true;
        }

        private bool TryParseOperator(out BinaryOperatorNode op)
        {
            op = null;
            Token t1 = window.Current;
            Token t2 = window.Peek();

            if (ParserUtils.IsOperator(t1, t2)) {
                op = new BinaryOperatorNode(t1 + t2);
                window.Advance(2);
            } else if (ParserUtils.IsOperator(t1)) {
                op = new BinaryOperatorNode(t1);
                window.Advance();
            }

            return op is not null;
        }

        private bool ParseOperator(out BinaryOperatorNode op, out string error)
        {
            if (!TryParseOperator(out op)) {
                error = $"Unexpected token '{window.Current}' was encountered by the parser; expected operator.";
                return false;
            }

            error = null;
            return true;
        }

        private bool ParseFunctionArguments(string functionName, out List<Node> arguments, out string error)
        {
            arguments = null;
            error = null;

            if (window.Current.Kind != TokenKind.LParen) {
                error = $"Error parsing '{functionName}' function; expected '(' before arguments";
                return false;
            }

            window.Advance();
            var args = new List<Node>();

            while (window.HasItem && window.Current.Kind != TokenKind.RParen) {
                if (!ParseNode(parseComplex: true, out Node node, out error)) {
                    return false;
                }

                args.Add(node);

                if (window.Current.Kind == TokenKind.Comma) {
                    window.Advance();
                }
            }

            if (window.Current.Kind != TokenKind.RParen) {
                error = $"Error parsing '{functionName}' function; expected ')' after arguments";
                return false;
            }

            arguments = args;
            return true;
        }
    }
}