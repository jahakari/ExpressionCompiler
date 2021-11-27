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

        public override string ToString() => Value;

        public static string operator +(Token t1, Token t2) => t1.Value + t2.Value; 
        public static implicit operator string(Token token) => token.Value;
    }
}
