using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class TodayFunctionNode : Node
    {
        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.Date;

        public override Node Accept(INodeVisitor visitor)
            => visitor.VisitToday(this);

        public override string ToString() => "TODAY()";
    }
}
