using ExpressionCompiler.Utility;

namespace ExpressionCompiler.Evaluation
{
    public interface IExpressionDelegateProxy
    {
        object Invoke(IIdentifierDataContext context);
    }
}
