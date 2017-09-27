namespace Assets.Code.Reusable
{
    class DropOutStack<T>
    {
        private T[] items;
        private int top = 0;
        public DropOutStack(int capacity)
        {
            items = new T[capacity];
        }

        /// <summary>
        /// Add a new item to the stack
        /// </summary>
        /// <param name="item">new item</param>
        public void Push(T item)
        {
            items[top] = item;
            top = (top + 1) % items.Length;
        }

        /// <summary>
        /// retrieve the last item from the stack
        /// </summary>
        /// <returns>latest item</returns>
        public T Pop()
        {
            top = (items.Length + top - 1) % items.Length;
            return items[top];
        }
    }
}
