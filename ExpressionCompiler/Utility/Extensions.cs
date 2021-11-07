using System;
using System.Collections;
using System.Collections.Generic;

namespace ExpressionCompiler.Utility
{
    public static class Extensions
    {
        public static IEnumerable<T> EveryNth<T>(this IEnumerable<T> source, int n)
            => new EveryNthEnumerableIterator<T>(source, n);

        private class EveryNthEnumerableIterator<T> : IEnumerable<T>, IEnumerator<T>
        {
            private readonly IEnumerable<T> source;
            private readonly int n;

            private IEnumerator<T> sourceEnumerator;
            private T current;
            private bool initialState = true;

            public EveryNthEnumerableIterator(IEnumerable<T> source, int n)
            {
                this.source = source;
                this.n = n;
            }

            public T Current => current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                current = default;
                initialState = true;
                sourceEnumerator?.Dispose();
            }

            public IEnumerator<T> GetEnumerator() => this;

            public bool MoveNext()
            {
                if (initialState) {
                    initialState = false;
                    sourceEnumerator = source.GetEnumerator();

                    if (sourceEnumerator.MoveNext()) {
                        current = sourceEnumerator.Current;
                        return true;
                    }

                    return false;
                }

                int count = 0;

                while (sourceEnumerator.MoveNext()) {
                    if (++count == n) {
                        current = sourceEnumerator.Current;
                        return true;
                    }
                }

                return false;
            }

            public void Reset() => throw new NotImplementedException();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
