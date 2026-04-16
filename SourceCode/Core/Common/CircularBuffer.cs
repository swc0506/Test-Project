using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Core
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private int capacity;
        private T[] array;
        private int head;
        private int tail;
        private int size;
        private int version;
        private object syncRoot;

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException("capacity", "Non-negative number required.");
            }

            this.capacity = capacity;
            array = new T[capacity];
            head = tail = size = 0;
        }

        public int Capacity
        {
            get { return capacity; }
        }

        public int Count
        {
            get { return size; }
        }

        public bool IsFull
        {
            get { return Count == capacity; }
        }

        public T this[int index]
        {
            get
            {
                int realIndex = CheckRealIndex(index);
                return array[realIndex];
            }
            set
            {
                int realIndex = CheckRealIndex(index);
                array[realIndex] = value;
                version++;
            }
        }

        private int CheckRealIndex(int index)
        {
            if (index < 0 || index >= size)
            {
                throw new ArgumentOutOfRangeException(
                    "index", "Index was out of range. Must be non-negative and less than the size of the collection.");
            }

            int realIndex = (head + index) % array.Length;
            return realIndex;
        }

        /// <summary>
        /// 在圆形数组的末尾添加一个元素
        /// </summary>
        /// <param name="item">元素</param>
        public void Push(T item)
        {
            array[tail] = item;
            tail = (tail + 1) % array.Length;
            if (size < array.Length)
            {
                size++;
            }
            else
            {
                //把最前面数据挤出去
                head = (head + 1) % array.Length;
            }

            version++;
        }

        /// <summary>
        /// 把圆形数组中的最后一个元素删除
        /// </summary>
        public T Pop()
        {
            if (size <= 0)
            {
                throw new InvalidOperationException("InvalidOperation_EmptyQueue");
            }

            tail = (tail - 1) % array.Length;
            if (tail < 0)
            {
                tail += array.Length;
            }

            T removed = array[tail];
            size--;
            version++;
            return removed;
        }

        /// <summary>
        /// 把圆形数组中的第一个元素删除
        /// </summary>
        public T Shift()
        {
            if (size <= 0)
            {
                throw new InvalidOperationException("InvalidOperation_EmptyQueue");
            }

            T removed = array[head];
            array[head] = default(T);
            head = (head + 1) % array.Length;
            size--;
            version++;
            return removed;
        }

        /// <summary>
        /// 圆形数组中是否包含该元素
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        /// <summary>
        /// 该元素在圆形数组中的index
        /// </summary>
        /// <param name="item">元素</param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            for (int i = 0; i < size; i++)
            {
                if (item == null)
                {
                    if (this[i] == null)
                    {
                        return i;
                    }
                }
                else
                {
                    if (this[i] != null && EqualityComparer<T>.Default.Equals(this[i], item))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// 清楚所有元素
        /// </summary>
        public void Clear()
        {
            if (head < tail)
            {
                Array.Clear(array, head, size);
            }
            else
            {
                Array.Clear(array, head, array.Length - head);
                Array.Clear(array, 0, tail);
            }

            head = tail = size = 0;
            version++;
        }

        public object SyncRoot
        {
            get
            {
                if (this.syncRoot == null)
                {
                    Interlocked.CompareExchange(ref this.syncRoot, new object(), null);
                }

                return this.syncRoot;
            }
        }

        public override string ToString()
        {
            T[] temp = new T[size];
            for (int i = 0; i < size; i++)
            {
                temp[i] = this[i];
            }

            string val = string.Format("[{0}]", String.Join(",", temp));
            return val;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Serializable]
        private struct Enumerator : IEnumerator<T>
        {
            private CircularBuffer<T> buffer;
            private int index;
            private int version;
            private T current;

            internal Enumerator(CircularBuffer<T> buffer)
            {
                this.buffer = buffer;
                index = 0;
                version = buffer.version;
                current = default(T);
            }

            public void Dispose()
            {
                buffer = null;
            }

            public bool MoveNext()
            {
                if (version == buffer.version && ((uint) index < (uint) buffer.size))
                {
                    current = buffer[index];
                    index++;
                    return true;
                }

                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (version != buffer.version)
                {
                    throw new InvalidOperationException(
                        "Collection was modified; enumeration operation may not execute.");
                }

                index = buffer.size + 1;
                current = default(T);
                return false;
            }

            public T Current
            {
                get { return current; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            void IEnumerator.Reset()
            {
                if (version != buffer.version)
                {
                    throw new InvalidOperationException(
                        "Collection was modified; enumeration operation may not execute.");
                }

                index = 0;
                current = default(T);
            }
        }
    }
}