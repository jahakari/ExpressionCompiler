using ExpressionCompiler.Syntax.Nodes;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public class TodayFunctionDefinition : IFunctionDefinition
    {
        public string Name => "TODAY";

        public bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error)
        {
            node = null;
            error = null;

            if (arguments.Count == 0) {
                node = new TodayFunctionNode();
                return true;
            }

            error = $"'{Name}' function does not accept any arguments";
            return false;
        }
    }
}
