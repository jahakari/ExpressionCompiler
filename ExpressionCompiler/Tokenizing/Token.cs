namespace ExpressionCompiler.Tokenizing
{
    public struct Token
    {
        public Token(TokenKind kind, string value)
        {
            Kind = kind;
            Value = value;
        }

        public Token(TokenKind kind, char value) : this(kind, value.ToString()) { }

        public TokenKind Kind { get; set; }
        public string Value { get; set; }

        public override string ToString()
            => $"{Kind, -20}{Value}";
    }
}
