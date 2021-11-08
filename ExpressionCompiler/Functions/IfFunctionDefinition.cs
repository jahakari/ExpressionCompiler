using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class IfFunctionDefinition : IFunctionDefinition
    {
        public string Name => "IF";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            if (arguments.Count == 3) {
                node = new IfFunctionNode(arguments[0], arguments[1], arguments[2]);
                return true;
            }

            error = FunctionUtils.CreateFunctionError(Name, arguments.Count);
            return false;
        }
    }
}
