using ExpressionCompiler.Functions;
using ExpressionCompiler.Syntax.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExpressionCompiler.Utility
{
    public class FunctionContext : IFunctionContext
    {
        private readonly Dictionary<string, IFunctionDefinition> functionDefinitions;

        private FunctionContext()
        {
            Type functionDefinitionType = typeof(IFunctionDefinition);

            functionDefinitions = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t.Namespace == functionDefinitionType.Namespace)
                .Where(t => t.IsAssignableTo(functionDefinitionType))
                .Where(t => t != functionDefinitionType)
                .Select(t => (IFunctionDefinition)Activator.CreateInstance(t))
                .ToDictionary(d => d.Name, d => d);
        }

        private static FunctionContext instance;
        public static FunctionContext Instance => instance ??= new();

        public bool IsFunction(string name) => functionDefinitions.ContainsKey(name);

        public bool TryCreateFunctionNode(string name, List<Node> arguments, out Node node, out string error)
        {
            node = null;

            if (!functionDefinitions.TryGetValue(name, out IFunctionDefinition definition)) {
                error = $"Definition for function '{name}' was not found";
                return false;
            }

            if (!definition.TryCreateFunctionNode(arguments, out node, out error)) {
                return false;
            }

            return true;
        }
    }
}
