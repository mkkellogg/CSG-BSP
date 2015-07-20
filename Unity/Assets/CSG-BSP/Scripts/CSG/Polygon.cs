using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG
{
	public struct Polygon
	{
		public Plane Plane;

		private List<Vertex> vertices;

		public Polygon(Polygon polygon)
		{
			Plane = polygon.Plane;
			vertices = new List<Vertex>();
			if(polygon.vertices != null)
			{
				vertices.AddRange (polygon.vertices);
			}
		}

		public Polygon(List<Vertex> vertices, Plane plane)
		{
			this.vertices = new List<Vertex>(vertices.ToArray ());
			Plane = plane;
		}

		public Vertex GetVertex(int index)
		{
			if(vertices == null || index < 0 || index >= vertices.Count)
			{
				throw new System.IndexOutOfRangeException("Invalid vertex index.");
			}

			return vertices [index];
		}

		public int VertexCount()
		{
			if(vertices == null)return 0;
			return vertices.Count;
		}

		public void AddVertex(Vertex vertex)
		{
			if(vertices == null)vertices = new List<Vertex>();
			vertices.Add (vertex);
		}

		public void Flip()
		{
			Plane.Flip ();
			if(vertices != null)
			{
				for(int i =0; i < vertices.Count; i++)
				{
					Vertex vi = vertices[i];
					vi.Flip();
					vertices[i] = vi;
				}
			}
		}
	}
}
