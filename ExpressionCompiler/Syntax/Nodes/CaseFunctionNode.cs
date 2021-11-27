using ExpressionCompiler.Utility;
using ExpressionCompiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class CaseFunctionNode : Node
    {
        public CaseFunctionNode(IEnumerable<Node> arguments)
        {
            Arguments = arguments.ToArray();

            SwitchArgument = Arguments[0];
            DefaultArgument = Arguments[Arguments.Length - 1];
            CaseArguments = Arguments[1..^1].EveryNth(2).ToArray();
            ReturnArguments = Arguments.Skip(2).EveryNth(2).ToArray();

            ValueType = NodeUtils.GetCommonDataType(ReturnArguments.Concat(new[] { DefaultArgument }));
        }

        public override NodeType NodeType => NodeType.Function;
        public override NodeValueType ValueType { get; }
        public override SemanticType SemanticType => SemanticType.Value;

        public Node[] Arguments { get; }
        public Node SwitchArgument { get; set; }
        public Node DefaultArgument { get; set; }
        public Node[] CaseArguments { get; set; }
        public Node[] ReturnArguments { get; set; }

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitCase(this);

        public override string ToString() => $"CASE({string.Join(", ", Arguments.AsEnumerable())})";

        public CaseFunctionNode Update(IEnumerable<Node> arguments)
        {
            if (arguments.SequenceEqual(Arguments)) {
                return this;
            }

            return new CaseFunctionNode(arguments);
        }
    }
}
