using ExpressionCompiler.Compilation;
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
            //var exp = new BinaryExpressionNode
            //(
            //    left: new LiteralValueNode<int>(501),
            //    @operator: new BinaryOperatorNode("%"),
            //    right: new LiteralValueNode<int>(500)
            //);

            //var exp = new LiteralValueNode<DateTime>(DateTime.Today);

            //Node exp = new BinaryExpressionNode
            //(
            //    new MonthFunctionNode(new LiteralValueNode<DateTime>(DateTime.Today)),
            //    new BinaryOperatorNode("+"),
            //    new LiteralValueNode<int>(10)
            //);

            //Node exp = new OrFunctionNode
            //(
            //    new List<Node>
            //    {
            //        new LiteralValueNode<bool>(false),
            //        new LiteralValueNode<bool>(false),
            //        new LiteralValueNode<bool>(false)
            //    }
            //);

            //Node exp = new BinaryExpressionNode
            //(
            //    new LiteralValueNode<decimal>(2.23m),
            //    new BinaryOperatorNode("^"),
            //    new LiteralValueNode<decimal>(2.34m)
            //);

            //Node exp = new BinaryExpressionNode
            //(
            //    new IdentifierNode("FOO", NodeValueType.Integer),
            //    new BinaryOperatorNode("*"),
            //    new LiteralValueNode<int>(3)
            //);

            //Node exp = new DayFunctionNode(new TodayFunctionNode());

            //Node exp = new DateFunctionNode
            //(
            //    new LiteralValueNode<int>(1988),
            //    new LiteralValueNode<int>(1),
            //    new LiteralValueNode<int>(19)
            //);

            //Node exp = new LeftFunctionNode
            //(
            //    new CStringFunctionNode(new LiteralValueNode<decimal>(1.2345m)),
            //    new LiteralValueNode<int>(3)
            //);

            //Node exp = new CIntFunctionNode
            //(
            //    new LiteralValueNode<decimal>(5.23m)
            //);

            //Node exp = new AbsFunctionNode
            //(
            //    new BinaryExpressionNode
            //    (
            //        new LiteralValueNode<int>(5),
            //        new BinaryOperatorNode("-"),
            //        new BinaryExpressionNode
            //        (
            //            new LiteralValueNode<int>(4),
            //            new BinaryOperatorNode("^"),
            //            new LiteralValueNode<int>(3)
            //        )
            //    )
            //);

            string input = "1 + 2 ^ 3 * 4";
            var result = new SyntaxParser().Parse(input);

            if (result.ParseSuccessful) {
                var rewriter = new NodeRewritingVisitor();
                Node node = result.ParsedNode.Accept(rewriter);
            } else {
                Console.WriteLine(result.Error);
            }

            //var compiler = new NodeCompiler(exp);

            //MethodInfo mInfo = compiler.Compile();
            //var del = mInfo.CreateDelegate<Func<IIdentifierDataContext, int>>();

            //var ctx = new DummyDataContext();
            //Console.WriteLine(del.Invoke(ctx));
        }
    }
}
