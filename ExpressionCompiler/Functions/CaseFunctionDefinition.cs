using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class CaseFunctionDefinition : IFunctionDefinition
    {
        public string Name => "CASE";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            if (arguments.Count < 4 || arguments.Count % 2 != 0) {
                error = FunctionUtils.CreateFunctionError(Name, arguments.Count);
                return false;
            }

            node = new CaseFunctionNode(arguments);
            return true;
        }
    }
}
