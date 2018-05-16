using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG
{
    public class FastLinkedList<T> : IList<T>
    {
        public class Node : Indexer
        {
            private int index = -1;

            public T Value;
            public Node Next;
            public Node Previous;

            public Node(T value) {
                Value = value;
            }

            public Node() {
                Value = default(T);
            }

            public int GetIndex() {
                return index;
            }

            public void SetIndex(int index) {
                this.index = index;
            }
        }

        private Node _First;
        private Node _Last;
        private int length;
        private ObjectPool<Node> nodePool;

        public FastLinkedList() {
           // this.nodePool = new ObjectPool<Node>(100000, 50000);
            _First = null;
            _Last = null;
            length = 0;
        }

        public Node First
        {
            get { return _First; }
        }

        public Node Last
        {
            get { return _Last; }
        }

        public Node AddLast(T value)
        {
            // Node newNode = this.nodePool.ClaimObject();
            // newNode.Value = value;

            Node newNode = new Node(value);

            if (_First == null)
            {
                _First = _Last = newNode;
            }
            else
            {
                _Last.Next = newNode;
                newNode.Previous = _Last;
                _Last = newNode;
            }

            length++;

            return newNode;
        }

        public void AppendIntoList(FastLinkedList<T> list)
        {
            if (list == null) return;
            if (list._First == null) return;

            if (_First == null)
            {
                _First = list._First;
                _Last = list._Last;
            }
            else
            {
                Node temp = _Last;
                _Last.Next = list._First;
                _Last.Next.Previous = _Last;
                _Last = list._Last;
                if (_Last.Previous == null) _Last.Previous = temp;
            }

            length = list.length + length;
        }

        public void CopyInto(IList<T> copy)
        {
            if (copy == null) return;

            FastLinkedList<T>.Node current = First;
            while (current != null)
            {
                copy.Add(current.Value);
                current = current.Next;
            }
        }

        public void CopyFrom(IList<T> source)
        {
            if (source == null) return;

            IEnumerator<T> enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AddLast(enumerator.Current);
            }
        }

        public void Iterate(System.Action<T> action)
        {
            Node current = First;
            while (current != null)
            {
                action.Invoke(current.Value);
                current = current.Next;
            }
        }

        // IList Members 
        public void Clear()
        {
            _First = null;
            _Last = null;
            length = 0;
        }

        public bool Contains(T value)
        {
            bool inList = false;

            Node current = First;
            while (current != null)
            {
                if (current.Value.Equals(value))
                {
                    inList = true;
                    break;
                }
                current = current.Next;
            }

            return inList;
        }

        public int IndexOf(T value)
        {
            Node current = First;
            int index = 0;
            while (current != null)
            {
                if (current.Value.Equals(value))
                {
                    return index;
                }
                index++;
                current = current.Next;
            }

            return -1;
        }

        public void Insert(int index, T value)
        {
            throw new System.NotSupportedException();
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void RemoveAt(int index)
        {
            throw new System.NotSupportedException("The method or operation is not implemented.");
        }

        public T this[int index]
        {
            get
            {
                Node current = First;
                int i = 0;
                while (current != null)
                {
                    if (i == index)
                    {
                        return current.Value;
                    }
                    i++;
                    current = current.Next;
                }

                throw new System.IndexOutOfRangeException("Index is out of range.");
            }

            set
            {
                Node current = First;
                int i = 0;
                while (current != null)
                {
                    if (i == index)
                    {
                        current.Value = value;
                        break;
                    }
                    i++;
                    current = current.Next;
                }
            }
        }

        // ICollection Members 
        void ICollection<T>.Add(T val)
        {
            AddLast(val);
        }

        bool ICollection<T>.Remove(T val)
        {
            RemoveAt(IndexOf(val));
            return true;
        }

        void ICollection<T>.CopyTo(T[] array, int index)
        {
            int j = index;
            Node current = First;
            while (current != null)
            {
                array[j] = current.Value;
                j++;
                current = current.Next;
            }
        }

        public int Count
        {
            get
            {
                return length;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        // IEnumerable Members
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new System.NotSupportedException("The method or operation is not implemented.");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotSupportedException("The method or operation is not implemented.");
        }
    }
}