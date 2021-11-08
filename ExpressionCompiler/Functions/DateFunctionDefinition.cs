using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class DateFunctionDefinition : IFunctionDefinition
    {
        public string Name => "DATE";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            switch (arguments.Count) {
                case 3:
                    node = new DateFunctionNode(arguments[0], arguments[1], arguments[2]);
                    return true;

                default:
                    error = FunctionUtils.CreateFunctionError(Name, arguments.Count);
                    return false;
            }
        }
    }
}
