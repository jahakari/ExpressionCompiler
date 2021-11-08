using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class RightFunctionDefinition : IFunctionDefinition
    {
        public string Name => "RIGHT";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            if (arguments.Count == 2) {
                node = new RightFunctionNode(arguments[0], arguments[1]);
                return true;
            }

            error = FunctionUtils.CreateFunctionError(Name, arguments.Count);
            return false;
        }
    }
}
