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

		public Triangle(Triangle polygon)
		{
			OrientationPlane = polygon.OrientationPlane;
			A = polygon.A;
			B = polygon.B;
			C = polygon.C;
		}
	
		public Triangle Clone()
		{
			return new Triangle(this);
		}

		public void Flip()
		{
			OrientationPlane.Flip ();

			A.Flip();
			B.Flip();
			C.Flip();

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
				break;
			case 2:
				return C;
				break;
			default:
				return A;
				break;
			}
			return r;
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
