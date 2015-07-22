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
					
			Vector3D cross = edgeA.Cross (edgeB);
			// !! Important: inverted to be valid in left-handed space
			// TODO: make this work in either handed-ness automatically
			cross.Invert();
			p.Normal = cross.Normalized();

			p.D = p.Normal.Dot (a);
			return p;
		}
	}
}