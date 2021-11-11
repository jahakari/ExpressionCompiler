using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using System.Collections.Generic;

namespace ExpressionCompiler.Visitors
{
    public class FunctionBuildingVisitor : NodeVisitor
    {
        private readonly IFunctionContext functionContext = FunctionContext.Instance;

        public List<string> Errors { get; } = new();

        public override Node VisitFunction(FunctionNode node)
        {
            if (functionContext.TryCreateFunctionNode(node.Name, node.Arguments, out Node newNode, out string error)) {
                return newNode.Accept(this);
            }

            Error(error);
            return node;
        }

        public void Reset() => Errors.Clear();

        private void Error(string error) => Errors.Add(error);
    }
}
