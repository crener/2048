using System;
using System.Collections;
using System.Collections.Generic;

namespace Code.Reusable
{
    public class ObjectPool<T> where T : class
    {
        Func<T> clone;
        int capacity;
        List<T> storage;
        Func<T, bool> useTest;
        bool expandable;

        /// <summary>
        /// Manages items and produces more if needed
        /// </summary>
        /// <param name="productionMethod">method that should be called if a new object needs to be created</param>
        /// <param name="maxSize">maximum size that this object can hold</param>
        /// <param name="objectInUse">method that is called when you need to determine if an object can be recycled</param>
        /// <param name="expandable">can the pool be expanded once you run out of usable items</param>
        public ObjectPool(Func<T> productionMethod, int maxSize, Func<T, bool> objectInUse, bool expandable)
        {
            clone = productionMethod;
            capacity = maxSize;
            storage = new List<T>(maxSize);
            useTest = objectInUse;
            this.expandable = expandable;
        }

        /// <summary>
        /// method to get the next available item. null if none are available
        /// </summary>
        public T GetObject()
        {
            int count = storage.Count;

            //look for a usable item
            foreach (T thing in storage)
            {
                if (!useTest(thing)) return thing;
            }

            //if there aren't any expand the array 
            if (count >= capacity && !expandable) return null;

            T newItem = clone();
            storage.Add(newItem);
            return newItem;
        }

        public void AddExisting(T existing)
        {
            storage.Add(existing);
        }

        //public List<T> GetPool() { return storage; }

        public IEnumerator GetEnumerator()
        {
            return storage.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return storage.Count;
            }
        }

        public T this[int i]
        {
            get
            {
                return storage[i];
            }
        }
    }
}
