using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class MonthFunctionDefinition : IFunctionDefinition
    {
        public string Name => "MONTH";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            if (arguments.Count == 1) {
                node = new MonthFunctionNode(arguments[0]);
                return true;
            }

            error = FunctionUtils.CreateFunctionError(Name, arguments.Count);
            return false;
        }
    }
}
