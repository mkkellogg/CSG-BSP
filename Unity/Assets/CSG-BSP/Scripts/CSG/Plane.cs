using UnityEngine;
using System.Collections;

namespace CSG
{
	public struct Plane
	{
		public Vector3D Normal;
		public float D;

		public Plane(Vector3D normal, float d)
		{
			Normal = normal;
			D = d;
		}

		public void Flip()
		{
			Normal.Invert ();
			D = -D;
		}

		public static Plane BuildFromVertices(Vector3D a, Vector3D b, Vector3D c)
		{
			Plane p = new Plane ();
			Vector3D edgeA = c.SubtractedBy (a);
			Vector3D edgeB = b.SubtractedBy (a);
			p.Normal = edgeA.Cross (edgeB).Normalized();
			p.D = p.Normal.Dot (a);
			return p;
		}
	}
}