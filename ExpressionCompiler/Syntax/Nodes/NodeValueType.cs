namespace ExpressionCompiler.Nodes
{
    public enum NodeValueType
    {
        None    = 0b00000,
        Boolean = 0b00001,
        Integer = 0b00010,
        Decimal = 0b00100,
        Number  = 0b00110,
        Date    = 0b01000,
        String  = 0b10000
    }
}
