using UnityEngine;
using System.Collections;

namespace CSG
{
	public struct Color4
	{
		public float R;
		public float G;
		public float B;
		public float A;

		public Color4(float r, float g, float b, float a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public Color4(Color4 color)
		{
			R = color.R;
			G = color.G;
			B = color.B;
			A = color.A;
		}

		public Color4 Lerped(Color4 dest, float t)
		{
			float lerpedR = (dest.R - R) * t + R;
			float lerpedG = (dest.G - G) * t + G;
			float lerpedB = (dest.B - B) * t + B;
			float lerpedA = (dest.A - A) * t + A;

			return new Color4(lerpedR, lerpedG, lerpedB, lerpedA);
		}
	}
}