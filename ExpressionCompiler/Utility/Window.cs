using System.Collections.Generic;
using System.Linq;

namespace ExpressionCompiler.Utility
{
    public class Window<T>
    {
        private T[] items;
        private int index = 0;

        public Window(IEnumerable<T> items) => this.items = items.ToArray();

        public int Position => index;
        public int Length => items.Length;
        public bool HasItem => IsIndexInRange(index);
        public T Current => HasItem ? items[index] : default;
        public T Next => Peek();
        public T Previous => Peek(-1);

        public void Advance(int count = 1) => index += count;
        public void Recede() => index--;
        public T Peek(int offset = 1)
        {
            int newIndex = index + offset;

            if (newIndex < 0 || newIndex >= Length) {
                return default;
            }

            return items[newIndex];
        }

        private bool IsIndexInRange(int i) => i >= 0 && i < Length;
    }
}
