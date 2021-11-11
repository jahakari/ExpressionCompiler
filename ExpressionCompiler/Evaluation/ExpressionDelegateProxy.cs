using ExpressionCompiler.Utility;
using System;
using System.Reflection;

namespace ExpressionCompiler.Evaluation
{
    public class ExpressionDelegateProxy : IExpressionDelegateProxy
    {
        private readonly IExpressionDelegateProxy typedProxy;

        public ExpressionDelegateProxy(MethodInfo methodInfo)
        {
            typedProxy = (IExpressionDelegateProxy)typeof(ExpressionDelegateProxyOfT<>)
                .MakeGenericType(methodInfo.ReturnType)
                .GetConstructor(new[] { typeof(MethodInfo) })
                .Invoke(new[] { methodInfo });
        }

        public object Invoke(IIdentifierDataContext context)
            => typedProxy.Invoke(context);

        private class ExpressionDelegateProxyOfT<T> : IExpressionDelegateProxy
        {
            private readonly Func<IIdentifierDataContext, T> internalDelegate;

            public ExpressionDelegateProxyOfT(MethodInfo methodInfo)
            {
                internalDelegate = methodInfo.CreateDelegate<Func<IIdentifierDataContext, T>>();
            }

            public object Invoke(IIdentifierDataContext context)
                => internalDelegate.Invoke(context);
        }
    }
}
