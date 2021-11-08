namespace ExpressionCompiler.Utility
{
    public static class FunctionUtils
    {
        public static string CreateFunctionError(string name, int argCount)
        {
            if (argCount == 0) {
                return $"No arguments were specified for '{name}' function";
            }

            return $"Incorrect number of arguments were specified for '{name}' function";
        }
    }
}
