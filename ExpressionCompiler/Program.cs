using ExpressionCompiler.Compilation;
using ExpressionCompiler.Nodes;
using ExpressionCompiler.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExpressionCompiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var exp = new BinaryExpressionNode
            (
                left: new LiteralValueNode<int>(501),
                @operator: new BinaryOperatorNode("%"),
                right: new LiteralValueNode<int>(500)
            );

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

            var ctx = new DummyDataContext();
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

            //Node exp = new RightFunctionNode
            //(
            //    new LiteralValueNode<string>("Hello, world!"),
            //    new LiteralValueNode<int>(6)
            //);

            var compiler = new NodeCompiler(exp);

            MethodInfo mInfo = compiler.Compile();
            var del = mInfo.CreateDelegate<Func<IIdentifierDataContext, int>>();

            Console.WriteLine(del.Invoke(ctx));
        }
    }
}
