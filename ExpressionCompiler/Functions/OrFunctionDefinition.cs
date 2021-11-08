using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class OrFunctionDefinition : IFunctionDefinition
    {
        public string Name => "OR";

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
                    node = new OrFunctionNode(arguments);
                    return true;
            }
        }
    }
}
