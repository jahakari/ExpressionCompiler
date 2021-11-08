using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Visitors
{
    public class FunctionBuildingVisitor : NodeVisitor
    {
        private readonly List<string> errors = new();
        private readonly IFunctionContext functionContext = FunctionContext.Instance;

        public override Node VisitFunction(FunctionNode node)
        {
            if (functionContext.TryCreateFunctionNode(node.Name, node.Arguments, out Node newNode, out string error)) {
                return newNode.Accept(this);
            }

            Error(error);
            return node;
        }

        private void Error(string error) => errors.Add(error);
    }
}
