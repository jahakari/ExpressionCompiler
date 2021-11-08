using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class AndFunctionDefinition : IFunctionDefinition
    {
        public string Name => "AND";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            switch (arguments.Count) {
                case 0:
                    error = FunctionUtils.CreateFunctionError(Name, 0);
                    return false;

                case 1:
                    error = $"'{Name}' function should have more than 1 argument";
                    return false;

                default:
                    node = new AndFunctionNode(arguments);
                    return true;
            }
        }
    }
}
