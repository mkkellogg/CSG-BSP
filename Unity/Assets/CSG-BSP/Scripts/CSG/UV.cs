using UnityEngine;
using System.Collections;

namespace CSG
{
	public struct UV
	{
		public float U;
		public float V;

		public UV(float u, float v)
		{
			U = u;
			V = v;
		}

		public UV(UV uv)
		{
			U = uv.U;
			V = uv.V;
		}

		public UV Lerped(UV dest, float t)
		{
			float lerpedU = (dest.U - U) * t + U;
			float lerpedV = (dest.V - V) * t + V;

			return new UV(lerpedU, lerpedV);
		}
	}
}