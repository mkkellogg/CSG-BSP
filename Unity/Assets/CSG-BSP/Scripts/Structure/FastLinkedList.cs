using UnityEngine;
using System.Collections;

namespace CSG
{
	public class FastLinkedList<T>
	{
		private Node _First;
		private Node _Last;

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
		}

		public void Clear()
		{
			_First = null;
			_Last = null;
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
	}
}