using ExpressionCompiler.Syntax.Nodes;
using System.Collections.Generic;

namespace ExpressionCompiler.Functions
{
    public interface IFunctionDefinition
    {
        string Name { get; }

        bool TryCreateFunctionNode(List<Node> arguments, out Node node, out string error);
    }
}
