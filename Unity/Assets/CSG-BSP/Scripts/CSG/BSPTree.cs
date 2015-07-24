using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG
{
	public class BSPTree
	{
		private Node root;

		public BSPTree()
		{

		}

		public void AddTriangles(List<Triangle> triangles)
		{
			if (triangles == null || triangles.Count <= 0) return;
		
			if(root == null)
			{
				root = Node.Create(triangles[0].OrientationPlane);
			}

			FastLinkedList<Triangle> linkedTriangles = new FastLinkedList<Triangle>();
			var enumerator = triangles.GetEnumerator();
			while(enumerator.MoveNext())
			{
				linkedTriangles.AddLast(enumerator.Current);
			}

			AddTriangles(root, linkedTriangles);
		}

		private void AddTriangles(Node node, FastLinkedList<Triangle> allTriangles)
		{
			if (allTriangles == null)return;
			if(node == null)return;

			FastLinkedList<Triangle> nodeTriangles =  node.GetTriangleList();			
			FastLinkedList<Triangle> lessThan = new FastLinkedList<Triangle> ();
			FastLinkedList<Triangle> greaterThan = new FastLinkedList<Triangle> ();

			FastLinkedList<Triangle>.Node current = allTriangles.First;
			while(current != null)
			{
				Partitioner.Orientation orient = Partitioner.SliceTriangle(current.Value, node.SplitPlane, lessThan, greaterThan, nodeTriangles, nodeTriangles);
				current = current.Next;
			}	
				
			allTriangles.Clear();

			if(lessThan.First != null)
			{
				if(node.LessThan == null)
					node.LessThan = Node.Create(lessThan.First.Value.OrientationPlane);
				AddTriangles(node.LessThan, lessThan);
			}

			if(greaterThan.First != null)
			{
				if( node.GreaterThan == null)
					node.GreaterThan = Node.Create(greaterThan.First.Value.OrientationPlane);
				AddTriangles(node.GreaterThan,  greaterThan);
			}
		}

		public void ClipOutTriangles(FastLinkedList<Triangle> triangles, bool clipLessThan = true)
		{
			ClipOutTriangles (root, triangles, clipLessThan);
		}
		
		private void ClipOutTriangles(Node node, FastLinkedList<Triangle> triangles, bool clipLessThan = true)
		{
			if (triangles == null || triangles.First == null)return;
			if(node == null)return;
			
			FastLinkedList<Triangle> lessThan = new FastLinkedList<Triangle>();
			FastLinkedList<Triangle> greaterThan = new FastLinkedList<Triangle>();
			
			FastLinkedList<Triangle>.Node current = triangles.First;
			while(current != null)
			{
				Partitioner.Orientation orient = Partitioner.SliceTriangle(current.Value, node.SplitPlane, lessThan, greaterThan, lessThan, greaterThan);
				current = current.Next;
			}

			triangles.Clear();

			if(node.LessThan != null)
				ClipOutTriangles (node.LessThan, lessThan, clipLessThan);
			else 
				if(clipLessThan)lessThan.Clear();

			if(node.GreaterThan != null)
				ClipOutTriangles (node.GreaterThan, greaterThan, clipLessThan);	
			else 
				if(!clipLessThan)greaterThan.Clear();
			
			triangles.AppendIntoList(lessThan);
			triangles.AppendIntoList(greaterThan);
		}

		public void ClipByTree(BSPTree tree, bool clipLessThan = true) 
		{
			ClipByTree (root, tree, clipLessThan);
		}

		private void ClipByTree(Node node, BSPTree tree, bool clipLessThan = true)
		{
			if(node == null)return;

			tree.ClipOutTriangles (node.GetTriangleList(), clipLessThan);
			ClipByTree (node.LessThan, tree, clipLessThan);
			ClipByTree (node.GreaterThan, tree, clipLessThan);
		}

		public List<Triangle> GetAllTriangles()
		{
			List<Triangle> allTriangles = new List<Triangle> ();
			GetAllTriangles (root, allTriangles);
			return allTriangles;
		}

		private void GetAllTriangles(Node node, List<Triangle> triangles)
		{
			if(node == null)return;

			FastLinkedList<Triangle>.Node current = node.GetTriangleList().First;
			while(current != null)
			{
				triangles.Add(current.Value);
				current = current.Next;
			}

			GetAllTriangles(node.LessThan, triangles);
			GetAllTriangles(node.GreaterThan, triangles);
		}

		public void Invert()
		{
			Invert (root);
		}

		private void Invert(Node node)
		{
			if(node == null)return;

			node.Invert ();
			Invert (node.LessThan);
			Invert (node.GreaterThan);
		}

		public BSPTree Clone()
		{
			BSPTree copy = new BSPTree();
			copy.root = Clone(root);
			return copy;
		}

		private Node Clone(Node node)
		{
			if(node == null) return null;
			Node copy = node.Clone();
			copy.LessThan = Clone(node.LessThan);
			copy.GreaterThan = Clone(node.GreaterThan);
			return copy;
		}

		private class Node
		{
			public Plane SplitPlane;
			public Node LessThan;
			public Node GreaterThan;

			private FastLinkedList<Triangle> triangles;

			public static Node Create(Plane splitPlane)
			{
				Node newNode = new Node();
				newNode.SplitPlane = splitPlane;
				return newNode;
			}

			private Node()
			{
				LessThan = null;
				GreaterThan = null;
				triangles = new FastLinkedList<Triangle>();
			}

			public Node Clone()
			{
				Node copy = new Node();

				copy.SplitPlane = SplitPlane;

				FastLinkedList<Triangle>.Node current = triangles.First;
				while(current != null)
				{
					copy.triangles.AddLast(current.Value);
					current = current.Next;
				}

				copy.LessThan = LessThan;
				copy.GreaterThan = GreaterThan;

				return copy;
			}

			public void Invert()
			{
				FastLinkedList<Triangle>.Node current = triangles.First;
				while(current != null)
				{
					current.Value.Invert();
					current = current.Next;
				}
				
				SplitPlane.Invert ();

				Node tempList = LessThan;
				LessThan = GreaterThan;
				GreaterThan = tempList;
			}

			public FastLinkedList<Triangle> GetTriangleList()
			{
				return triangles;
			}
		}
	}
}