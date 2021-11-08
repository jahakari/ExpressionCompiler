using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class CStringFunctionDefinition : IFunctionDefinition
    {
        public string Name => "CSTRING";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            switch (arguments.Count) {
                case 1:
                    node = new CStringFunctionNode(arguments[0]);
                    return true;

                default:
                    error = FunctionUtils.CreateFunctionError(Name, arguments.Count);
                    return false;
            }
        }
    }
}
