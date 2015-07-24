/**
 * 
 * class: BSPTree
 * 
 * Author: Mark Kellogg
 * 
 * This class contains functionality for classifying, partitioning and storing
 * 3D triangles using binary space partitioning.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG
{
	public class BSPTree
	{
		private Node root;

		/**
		 * Add the triangles from @triangles into this BSPTree instance.
		 */
		public void AddTriangles(List<Triangle> triangles)
		{
			if (triangles == null || triangles.Count <= 0) return;
		
			// ensure the root node exists
			if(root == null)
			{
				root = Node.Create(triangles[0].OrientationPlane);
			}

			// convert @triangles to a FastLinkedList structure.
			FastLinkedList<Triangle> linkedTriangles = new FastLinkedList<Triangle>();
			var enumerator = triangles.GetEnumerator();
			while(enumerator.MoveNext())
			{
				linkedTriangles.AddLast(enumerator.Current);
			}

			// call the private, recursive version of AddTriangles
			AddTriangles(root, linkedTriangles);
		}

		/**
		 * Rescursive version of AddTriangles. This method partitions the triangles
		 * in @triangles using @node's split plane, and then recursively calls itself
		 * with the resulting greater-than and less-than lists.
		 */
		private void AddTriangles(Node node, FastLinkedList<Triangle> triangles)
		{
			if (triangles == null)return;
			if(node == null)return;

			// get a reference to the list of triangles that are co-planar with
			// @node's split plane
			FastLinkedList<Triangle> nodeTriangles =  node.GetTriangleList();	

			FastLinkedList<Triangle> lessThan = new FastLinkedList<Triangle> ();
			FastLinkedList<Triangle> greaterThan = new FastLinkedList<Triangle> ();

			// iterate through each triangle in @triangles and classify/partition each according
			// @node's split plane. co-planar triangles go into @nodeTriangles, triangles on the front
			// side go into @greaterThan, traingles on the back side go into @lessThan.
			FastLinkedList<Triangle>.Node current = triangles.First;
			while(current != null)
			{
				Partitioner.Orientation orient = Partitioner.SliceTriangle(current.Value, node.SplitPlane, lessThan, greaterThan, nodeTriangles, nodeTriangles);
				current = current.Next;
			}	
				
			// release clear memory occupied by @triangles
			triangles.Clear();

			// recurse on the back side of @node's split plane
			if(lessThan.First != null)
			{
				if(node.LessThan == null)
					node.LessThan = Node.Create(lessThan.First.Value.OrientationPlane);
				AddTriangles(node.LessThan, lessThan);
			}

			// recurse on the front side of @node's split plane
			if(greaterThan.First != null)
			{
				if( node.GreaterThan == null)
					node.GreaterThan = Node.Create(greaterThan.First.Value.OrientationPlane);
				AddTriangles(node.GreaterThan,  greaterThan);
			}
		}

		/**
		 * Remove the triangles in @triangles that are completely inside the geometry contained
		 * by this BSPTree instance. Triangles that are partially inside the geometry are clipped
		 * against it.
		 * 
		 * If @clipLessThan is false, the operation is reversed and triangles portions outside the geometry
		 * of this BSPTree instance are removed.
		 */ 
		public void ClipOutTriangles(FastLinkedList<Triangle> triangles, bool clipLessThan = true)
		{
			// ensure the root node exists
			if(root == null)return;

			// call the private, recursive version of ClipOutTriangles
			ClipOutTriangles (root, triangles, clipLessThan);
		}

		/**
		 * Recursive version of ClipOutTriangles. This method partitions the triangles
		 * in @triangles using @node's split plane, and then recursively calls itself
		 * with the resulting greater-than and less-than lists. If the recursion reaches a 
		 * point where triangles in @triangles are on the back side of @node's split plane, 
		 * but this instance of BSPTree contains no geometry on that side (node.LessThan == null),
		 * then the triangles placed in @lessThan are deleted from @triangles. This removes
		 * the portions of triangles in @triangles that lie inside the geometry of this BSPTree
		 * instance.
		 * 
		 * If @clippLessThan is false, then we perform the reverse of the above oepration.
		 * Triangles placed in @greaterThan than are removed when node.GreaterThan == null. 
		 * In that case the portions of triangles in @triangles that lie outside the geometry 
		 * of this BSPTree instance are removed.
		 */ 
		private void ClipOutTriangles(Node node, FastLinkedList<Triangle> triangles, bool clipLessThan = true)
		{
			if (triangles == null || triangles.First == null)return;
			if(node == null)return;
			
			FastLinkedList<Triangle> lessThan = new FastLinkedList<Triangle>();
			FastLinkedList<Triangle> greaterThan = new FastLinkedList<Triangle>();

			// iterate through each triangle in @triangles and classify/partition each according
			// @node's split plane. triangles on the front side go into @greaterThan, triangles 
			// on the back side go into @lessThan. co-planar triangles whose normal matches that of
			// @node's split plane go into @greaterThan; the rest go into @lessThan.
			FastLinkedList<Triangle>.Node current = triangles.First;
			while(current != null)
			{
				Partitioner.Orientation orient = Partitioner.SliceTriangle(current.Value, node.SplitPlane, lessThan, greaterThan, lessThan, greaterThan);
				current = current.Next;
			}

			// release memory used by @triangles
			triangles.Clear();

			// recurse on the back side of @node's split plane if this BSPTree contains
			// geometry on that side. if it does not, and we want to clip out triangles
			// inside this BSPTree's geometry (@clipLessThan == true), then we clear out @lessThan.
			if(node.LessThan != null)
				ClipOutTriangles (node.LessThan, lessThan, clipLessThan);
			else 
				if(clipLessThan)lessThan.Clear();

			// recurse on the front side of @node's split plane if this BSPTree contains
			// geometry on that side. if it does not, and we want to clip out triangles
			// outside this BSPTree's geometry (@clipLessThan == false), then we clear out @greaterThan.
			if(node.GreaterThan != null)
				ClipOutTriangles (node.GreaterThan, greaterThan, clipLessThan);	
			else 
				if(!clipLessThan)greaterThan.Clear();

			// rebuild @triangles with the properly clipped triangles
			triangles.AppendIntoList(lessThan);
			triangles.AppendIntoList(greaterThan);
		}

		/**
		 * Remove the triangles in this BSPTree instance that are completely inside the 
		 * geometry contained by @tree. Triangles that are partially inside the geometry 
		 * are clipped against it.
		 * 
		 * If @clipLessThan is false, the operation is reversed and triangle portions 
		 * outside the geometry of @tree instance are removed.
		 */ 
		public void ClipByTree(BSPTree tree, bool clipLessThan = true) 
		{
			ClipByTree (root, tree, clipLessThan);
		}

		/**
		 * Recursive version of ClipByTree. This method recursively visits each node in this
		 * BSPTree instance and clips the triangles of each based on the geometry of @tree.
		 * By default it will remove the portions of triangles that are completely inside the 
		 * geometry contained by @tree. 		 
		 * 
		 * If @clipLessThan is false, the operation is reversed and triangle portions 
		 * outside the geometry of @tree instance are removed.
		 */ 
		private void ClipByTree(Node node, BSPTree tree, bool clipLessThan = true)
		{
			if(node == null)return;

			tree.ClipOutTriangles (node.GetTriangleList(), clipLessThan);
			ClipByTree (node.LessThan, tree, clipLessThan);
			ClipByTree (node.GreaterThan, tree, clipLessThan);
		}

		/**
		 * Return a list of all the triangles contained in the geometry of this
		 * BSPTree instance.
		 */
		public List<Triangle> GetAllTriangles()
		{
			List<Triangle> allTriangles = new List<Triangle> ();
			GetAllTriangles (root, allTriangles);
			return allTriangles;
		}

		/**
		 * Recursive component of GetAllTriangles().
		 */ 
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

		/**
		 * Reverse the winding order and normal direction of all triangles
		 * and split planes in this BSPTree instance.
		 */
		public void Invert()
		{
			Invert (root);
		}

		/**
		 * Recursively visit each node in this BSPTree instance and invert each.
		 */
		private void Invert(Node node)
		{
			if(node == null)return;

			node.Invert ();
			Invert (node.LessThan);
			Invert (node.GreaterThan);
		}

		/**
		 * Produce a deep copy of this BSPTree instance.
		 */
		public BSPTree Clone()
		{
			BSPTree copy = new BSPTree();
			copy.root = Clone(root);
			return copy;
		}

		/**
		 * Produce a DEEP copy of @node (including all Node instances attached to it) recursively.
		 */
		private Node Clone(Node node)
		{
			if(node == null) return null;
			Node copy = node.Clone();
			copy.LessThan = Clone(node.LessThan);
			copy.GreaterThan = Clone(node.GreaterThan);
			return copy;
		}

		/**
		 * Node represents a single of a BSPTree.
		 */
		private class Node
		{
			// the dividing plane for this node
			public Plane SplitPlane;
			// pointer to geometry on the back-side @SplitPlane
			public Node LessThan;
			// pointer to geometry on the front-side @SplitPlane
			public Node GreaterThan;
			// triangles that are co-planar with this Node's split plane
			private FastLinkedList<Triangle> triangles;

			// Only way to instantiate a new Node, since the constructor is private
			// The node's split plane must be specified during its creation.
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

			/** 
			* Produce a SHALLOW copy of this Node instance (the children of
			* this instance are NOT cloned).
			*/
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

			/**
			 * Invert this Node instances's split plane and triangles.
			 */
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

			/**
			 * Get a reference to the list that contains this node's triangles.
			 */ 
			public FastLinkedList<Triangle> GetTriangleList()
			{
				return triangles;
			}
		}
	}
}