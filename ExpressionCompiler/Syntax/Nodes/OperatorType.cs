namespace ExpressionCompiler.Nodes
{
    public enum OperatorType
    {
        None               = 0b000000000000,
        Add                = 0b000000000001,
        Subtract           = 0b000000000010,
        AddOrSubtract      = 0b000000000011,
        Multiply           = 0b000000000100,
        Divide             = 0b000000001000,
        Modulo             = 0b000000010000,
        MultiplyOrDivide   = 0b000000011100,
        Exponent           = 0b000000100000,
        Math               = 0b000000111111,
        Equal              = 0b000001000000,
        NotEqual           = 0b000010000000,
        LessThan           = 0b000100000000,
        LessThanOrEqual    = 0b001000000000,
        GreaterThan        = 0b010000000000,
        GreaterThanOrEqual = 0b100000000000,
        Boolean            = 0b111111000000
    }
}
