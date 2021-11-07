using ExpressionCompiler.Syntax.Nodes;

namespace ExpressionCompiler.Syntax
{
    public class ParseResult
    {
        private ParseResult(Node node)
        {
            ParseSuccessful = true;
            ParsedNode = node;
        }

        private ParseResult(string error) => Error = error;

        public Node ParsedNode { get; }
        public string Error { get; }
        public bool ParseSuccessful { get; }

        public static ParseResult Success(Node node) => new(node);

        public static ParseResult Failure(string error) => new(error);
    }
}
