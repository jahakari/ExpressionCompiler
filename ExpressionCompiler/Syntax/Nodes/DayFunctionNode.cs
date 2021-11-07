using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class DayFunctionNode : Node, IFunctionNode
    {
        public DayFunctionNode(Node date)
        {
            Date = date;
        }

        public override NodeType NodeType => NodeType.Function;

        public override NodeValueType ValueType => NodeValueType.Integer;

        public Node Date { get; }

        public string FunctionName => "DAY";

        public override Node Accept(NodeVisitor visitor)
            => visitor.VisitDay(this);

        public override string ToString() => $"DAY({Date})";

        public DayFunctionNode Update(Node date)
        {
            if (date == Date) {
                return this;
            }

            return new DayFunctionNode(date);
        }
    }
}
