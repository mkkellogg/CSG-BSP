using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG
{
	public struct Triangle
	{
		public Plane OrientationPlane;
		public Vertex A;
		public Vertex B;
		public Vertex C;

		public Triangle(Vertex a, Vertex b, Vertex c)
		{
			OrientationPlane = Plane.BuildFromVertices(a.Position, b.Position, c.Position);
			A = a;
			B = b;
			C = c;
		}

		public Triangle(Triangle triangle)
		{
			OrientationPlane = triangle.OrientationPlane;
			A = triangle.A;
			B = triangle.B;
			C = triangle.C;
		}

		public void Invert()
		{
			OrientationPlane.Invert ();

			A.Invert();
			B.Invert();
			C.Invert();

			Vertex temp = A;
			A = C;
			C = temp;
		}

		public Vertex GetVertexByIndex(int index)
		{
			Vertex r;
			switch(index)
			{
			case 1:
				return B;
			case 2:
				return C;
			default:
				return A;
			}
		}

		public void SetVertexByIndex(int index, Vertex vertex)
		{
			switch(index)
			{
			case 1:
				B = vertex;
				break;
			case 2:
				C = vertex;
				break;
			default:
				A = vertex;
				break;
			}
		}

		public void RebuildOrientationPlane()
		{
			OrientationPlane = Plane.BuildFromVertices(A.Position, B.Position, C.Position);
		}
	}
}
