using ExpressionCompiler.Visitors;
using System;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class FunctionNode : Node
    {
        public FunctionNode()
        {

        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.None;

        public override Node Accept(INodeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
