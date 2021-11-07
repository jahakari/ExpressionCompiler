using ExpressionCompiler.Syntax.Nodes;

namespace ExpressionCompiler.Visitors
{
    public interface INodeVisitor
    {
        Node VisitAbs(AbsFunctionNode node);
        Node VisitAnd(AndFunctionNode node);
        Node VisitBinary(BinaryExpressionNode node);
        Node VisitCInt(CIntFunctionNode node);
        Node VisitCString(CStringFunctionNode node);
        Node VisitDate(DateFunctionNode node);
        Node VisitDay(DayFunctionNode node);
        Node VisitGroup(GroupNode node);
        Node VisitIdentifier(IdentifierNode node);
        Node VisitIf(IfFunctionNode node);
        Node VisitLeft(LeftFunctionNode node);
        Node VisitLiteral(LiteralValueNode node);
        Node VisitMonth(MonthFunctionNode node);
        Node VisitNegation(NegationNode node);
        Node VisitOr(OrFunctionNode node);
        Node VisitRight(RightFunctionNode node);
        Node VisitToday(TodayFunctionNode node);
        Node VisitYear(YearFunctionNode node);
    }
}
