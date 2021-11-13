using ExpressionCompiler.Syntax.Nodes;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class MinFunctionDefinition : IFunctionDefinition
    {
        public string Name => "MIN";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            if (arguments.Count < 2) {
                error = "MIN function requires 2 or more arguments.";
                return false;
            }

            node = new MinFunctionNode(arguments);
            return true;
        }
    }
}
