using ExpressionCompiler.Syntax.Nodes;
using System.Collections.Generic;

namespace ExpressionCompiler.Utility
{
    public interface IFunctionContext
    {
        bool IsFunction(string name);
        bool TryCreateFunctionNode(string name, List<Node> arguments, out Node node, out string error);
    }
}
