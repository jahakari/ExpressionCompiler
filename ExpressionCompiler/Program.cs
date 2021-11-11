using ExpressionCompiler.Compilation;
using ExpressionCompiler.Evaluation;
using ExpressionCompiler.Syntax;
using ExpressionCompiler.Syntax.Nodes;
using ExpressionCompiler.Tokenizing;
using ExpressionCompiler.Utility;
using ExpressionCompiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExpressionCompiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var evaluator = new ExpressionEvaluator();

            while (true) {
                Console.WriteLine("Enter an expression to evaluate, or 'q' to quit:");

                string input = Console.ReadLine();

                if (input.ToLower() == "q") {
                    break;
                }

                Console.WriteLine();

                if (evaluator.TryEvaluate(input, out object result, out List<string> errors)) {
                    Console.WriteLine(result);
                }
                else {
                    foreach (var error in errors) {
                        Console.WriteLine(error);
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
