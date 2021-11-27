using ExpressionCompiler.Compilation;
using ExpressionCompiler.Syntax;
using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Utility;
using ExpressionCompiler.Visitors;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExpressionCompiler.Evaluation
{
    public class ExpressionEvaluator
    {
        private readonly Dictionary<string, IExpressionDelegateProxy> compiledExpressionCache = new();
        private readonly SyntaxParser parser = new();
        private readonly FunctionBuildingVisitor functionBuilder = new();
        private readonly NodeValidatingVisitor nodeValidator = new();
        private readonly NodeRewritingVisitor nodeRewriter = new();

        private IIdentifierDataContext dataContext = new DummyDataContext();

        public ExpressionEvaluator UseDataContext(IIdentifierDataContext ctx)
        {
            dataContext = ctx;
            return this;
        }

        public bool TryEvaluate(string expression, out object result, out List<string> errors)
        {
            result = null;

            if (!TryGetOrCreateExpressionDelegateProxy(expression, out IExpressionDelegateProxy proxy, out errors)) {
                return false;
            }

            result = proxy.Invoke(dataContext);
            return true;
        }

        private bool TryGetOrCreateExpressionDelegateProxy(string expression, out IExpressionDelegateProxy proxy, out List<string> errors)
        {
            errors = null;

            if (compiledExpressionCache.TryGetValue(expression, out proxy)) {
                return true;
            }

            ParseResult parseResult = parser.Parse(expression);

            if (!parseResult.ParseSuccessful) {
                errors = new List<string> { parseResult.Error };
                return false;
            }

            if (parseResult.ParsedNode.SemanticType != SemanticType.Value) {
                errors = new List<string> { $"Expression '{parseResult.ParsedNode}' cannot be evaluated." };
                return false;
            }

            functionBuilder.Reset();
            Node node = parseResult.ParsedNode.Accept(functionBuilder);

            if (functionBuilder.Errors.Any()) {
                errors = functionBuilder.Errors;
                return false;
            }

            nodeValidator.Reset();
            node = node.Accept(nodeValidator);

            if (nodeValidator.Errors.Any()) {
                errors = nodeValidator.Errors;
                return false;
            }

            node = node.Accept(nodeRewriter);

            var compiler = new NodeCompiler(node);
            MethodInfo method = compiler.Compile();
            proxy = new ExpressionDelegateProxy(method);

            compiledExpressionCache.Add(expression, proxy);
            return true;
        }
    }
}
