using UnityEngine;
using System.Collections;

namespace CSG
{
	public struct Vector4D
	{
		public float X;
		public float Y;
		public float Z;
		public float W;
		
		public Vector4D(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
		
		public Vector4D(Vector4D src)
		{
			X = src.X;
			Y = src.Y;
			Z = src.Z;
			W = src.W;
		}
		
		public Vector4D(float[] elements)
		{
			if(elements != null && elements.Length >= 4)
			{
				X = elements[0];
				Y = elements[1];
				Z = elements[2];
				W = elements[3];
			}
			else
			{
				X = 0;
				Y = 0;
				Z = 0;
				W = 0;
			}
		}
		
		public Vector4D Clone()
		{
			return new Vector4D (this);
		}

		public Vector4D Lerped(Vector4D dest, float t)
		{
			return new Vector4D(this).AddedWith(dest.SubtractedBy (this).MultipliedBy (t));
		}

		public void Subtract(Vector4D b)
		{
			this.X -= b.X;
			this.Y -= b.Y;
			this.Z -= b.Z;
			this.W -= b.W;
		}
		
		public Vector4D SubtractedBy(Vector4D b)
		{
			Vector4D subtracted = new Vector4D (this);
			subtracted.Subtract (b);
			return b;
		}
		
		public void Add(Vector4D b)
		{
			this.X += b.X;
			this.Y += b.Y;
			this.Z += b.Z;
			this.W += b.W;
		}
		
		public Vector4D AddedWith(Vector4D b)
		{
			Vector4D added = new Vector4D (this);
			added.Add (b);
			return b;
		}
		
		public void MultiplyBy(float factor)
		{
			X *= factor;
			Y *= factor;
			Z *= factor;
			W *= factor;
		}
		
		public Vector4D MultipliedBy(float factor)
		{
			Vector4D multiplied = new Vector4D(this);
			multiplied.MultiplyBy (factor);
			return multiplied;
		}
	}
}