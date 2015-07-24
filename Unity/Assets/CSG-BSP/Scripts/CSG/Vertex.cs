using UnityEngine;
using System.Collections;

namespace CSG
{
	public struct Vertex 
	{
		public Vector3D Position;
		public Vector3D Normal;
		public Vector4D Tangent;
		public UV UV1;
		public UV UV2;
		public Color4 Color;

		public Vertex(Vertex vertex)
		{
			Position = vertex.Position;
			Normal = vertex.Normal;
			Tangent = vertex.Tangent;
			UV1 = vertex.UV1;
			UV2 = vertex.UV2;
			Color = vertex.Color;
		}

		public Vertex(Vector3D position, Vector3D normal, Vector4D tangent, UV uv1, UV uv2, Color4 color)
		{
			Position = position;
			Normal = normal;
			Tangent = tangent;
			UV1 = uv1;
			UV2 = uv2;
			Color = color;
		}

		public void Invert()
		{
			Normal.Invert ();
		}

		public Vertex Lerped (Vertex dest, float t) 
		{
			Vertex lerped;

			lerped.Position = Position.Lerped (dest.Position, t);
			lerped.Normal = Normal.Lerped (dest.Normal, t);
			lerped.Tangent = Tangent.Lerped (dest.Tangent, t);
			lerped.UV1 = UV1.Lerped (dest.UV1, t);
			lerped.UV2 = UV2.Lerped (dest.UV2, t);
			lerped.Color = Color.Lerped (dest.Color, t);

			return lerped;
		}
	}
}
