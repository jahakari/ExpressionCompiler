using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class YearFunctionDefinition : IFunctionDefinition
    {
        public string Name => "YEAR";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            if (arguments.Count == 1) {
                node = new YearFunctionNode(arguments[0]);
                return true;
            }

            error = FunctionUtils.CreateFunctionError(Name, arguments.Count);
            return false;
        }
    }
}
