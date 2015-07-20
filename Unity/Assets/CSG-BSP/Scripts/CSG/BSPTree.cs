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
			root = null;
		}

		public void Build(List<Polygon> polygons)
		{
			root = Node.BuildTree(polygons);
		}

		public void ClipOutPolygons(List<Polygon> polygons)
		{
			ClipOutPolygons (root, polygons);
		}

		private void ClipOutPolygons(Node node, List<Polygon> polygons)
		{
			if(node == null)return;

			List<Polygon> lessThan = new List<Polygon>();
			List<Polygon> greaterThan = new List<Polygon>();

			for (int i = 0; i < polygons.Count; i++) 
			{
				Partitioner.SlicePolygon(polygons[i], node.SplitPlane, lessThan, greaterThan, lessThan, greaterThan);
			}

			if(node.LessThan != null)
				ClipOutPolygons (node.LessThan, lessThan);
			else 
				lessThan.Clear();
			ClipOutPolygons (node.GreaterThan, greaterThan);

			polygons.Clear ();
			polygons.AddRange (lessThan);
			polygons.AddRange (greaterThan);
		}

		public void ClipByTree(BSPTree tree) 
		{
			ClipByTree (root, tree);
		}

		private void ClipByTree(Node node, BSPTree tree)
		{
			if(node == null)return;

			tree.ClipOutPolygons (node.GetPolygonList());
			ClipByTree (node.LessThan, tree);
			ClipByTree (node.GreaterThan, tree);
		}

		public List<Polygon> GetAllPolygons()
		{
			List<Polygon> allPolys = new List<Polygon> ();
			GetAllPolygons (root, allPolys);
			return allPolys;
		}

		private void GetAllPolygons(Node node, List<Polygon> polygons)
		{
			if(node == null)return;

			for(int i = 0; i < node.PolygonCount(); i++)
			{
				polygons.Add(node.GetPolygon(i));
			}
			GetAllPolygons(node.LessThan, polygons);
			GetAllPolygons(node.GreaterThan, polygons);
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

		private class Node
		{
			public Plane SplitPlane;
			public Node LessThan;
			public Node GreaterThan;

			private List<Polygon> polygons;

			public Node()
			{
				LessThan = null;
				GreaterThan = null;
				polygons = new List<Polygon>();
			}

			public static Node BuildTree(List<Polygon> polygons)
			{
				if (polygons == null || polygons.Count <= 0) return null;

				Node node = new Node ();

				node.SplitPlane = polygons[0].Plane;

				List<Polygon> lessThan = new List<Polygon> ();
				List<Polygon> greaterThan = new List<Polygon> ();

				for (int i = 0; i < polygons.Count; i++) 
				{
					Partitioner.SlicePolygon(polygons[i], node.SplitPlane, lessThan, greaterThan, node.polygons, node.polygons);
				}

				node.LessThan = BuildTree(lessThan);
				node.GreaterThan = BuildTree(greaterThan);

				return node;
			}

			public void Invert()
			{
				for (int i = 0; i < polygons.Count; i++) 
				{
					Polygon poly = polygons[i];
					poly.Flip();
					polygons[i] = poly;
				}

				SplitPlane.Flip ();

				Node tempList = LessThan;
				LessThan = GreaterThan;
				GreaterThan = tempList;
			}

			public int PolygonCount()
			{
				if(polygons != null)return polygons.Count;
				else return 0;
			}

			public Polygon GetPolygon(int index)
			{
				if(polygons == null || index < 0 || index >= polygons.Count)
				{
					throw new System.IndexOutOfRangeException("Invalid polygon index.");
				}

				return polygons [index];
			}

			public List<Polygon> GetPolygonList()
			{
				return polygons;
			}
		}
	}
}