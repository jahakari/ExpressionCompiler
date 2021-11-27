using ExpressionCompiler.Visitors;

namespace ExpressionCompiler.Syntax.Nodes
{
    public class DayFunctionNode : Node
    {
        public DayFunctionNode(Node date)
        {
            Date = date;
        }

        public override NodeType NodeType => NodeType.Function;
        public override NodeValueType ValueType => NodeValueType.Integer;
        public override SemanticType SemanticType => SemanticType.Value;

        public Node Date { get; }

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
