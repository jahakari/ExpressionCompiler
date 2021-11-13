using ExpressionCompiler.Syntax.Nodes;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class MaxFunctionDefinition : IFunctionDefinition
    {
        public string Name => "MAX";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            if (arguments.Count < 2) {
                error = "MAX function requires 2 or more arguments.";
                return false;
            }

            node = new MaxFunctionNode(arguments);
            return true;
        }
    }
}
