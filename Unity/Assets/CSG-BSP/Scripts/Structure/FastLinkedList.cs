using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG
{
	public class FastLinkedList<T> : IList<T>
	{
		private Node _First;
		private Node _Last;
		private int length;

		public Node First
		{
			get { return _First;}
		}

		public Node Last
		{
			get { return _Last;}
		}
		
		public FastLinkedList()
		{
			_First = null;
			_Last = null;
			length = 0;
		}

		public Node AddLast(T value)
		{
			Node newNode = new Node(value);

			if(_First == null)
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
			if(list == null)return;
			if(list._First == null)return;

			if(_First == null)
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
				if(_Last.Previous == null)_Last.Previous = temp;
			}

			length = list.length + length;
		}

		public void CopyInto(IList<T> copy)
		{
			FastLinkedList<T>.Node current = First;
			while(current != null)
			{
				copy.Add(current.Value);
				current = current.Next;
			}
		}

		public class Node
		{
			public T Value;
			public Node Next;
			public Node Previous;

			public Node(T value)
			{
				Value = value;
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
			while(current != null)
			{
				if(current.Value.Equals(value))
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
			while(current != null)
			{
				if(current.Value.Equals(value))
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
				while(current != null)
				{
					if(i == index)
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
				while(current != null)
				{
					if(i == index)
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
			while(current != null)
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