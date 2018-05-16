using UnityEngine;
using System.Collections;

namespace CSG {
    public class Vector4D {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4D() {
            this.Set(0, 0, 0, 0);
        }

        public Vector4D(float x, float y, float z, float w) {
            this.Set(x, y, z, w);
        }

        public Vector4D(Vector4D src) {
            this.Set(src.X, src.Y, src.Z, src.W);
        }

        public Vector4D(float[] elements) {
            if (elements != null && elements.Length >= 4) {
                this.Set(elements[0], elements[1], elements[2], elements[3]);
            }
            else {
                this.Set(0, 0, 0, 0);
            }
        }

        public void Set(float x, float y, float z, float w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public void Copy(Vector4D src) {
            this.Set(src.X, src.Y, src.Z, src.W);
        }

        public Vector4D Clone() {
            return new Vector4D(this);
        }

        public void Lerp(Vector4D dest, float t) {
            this.X = (1 - t) * this.X + t * dest.X;
            this.Y = (1 - t) * this.Y + t * dest.Y;
            this.Z = (1 - t) * this.Z + t * dest.Z;
            this.W = (1 - t) * this.W + t * dest.W;
        }

        public Vector4D Lerped(Vector4D dest, float t, Vector4D outVector) {
            outVector.Copy(this);
            outVector.Lerp(dest, t);
            return outVector;
        }

        public void Subtract(Vector4D b) {
            this.X -= b.X;
            this.Y -= b.Y;
            this.Z -= b.Z;
            this.W -= b.W;
        }

        public Vector4D SubtractedBy(Vector4D b) {
            Vector4D subtracted = new Vector4D(this);
            subtracted.Subtract(b);
            return b;
        }

        public void Add(Vector4D b) {
            this.X += b.X;
            this.Y += b.Y;
            this.Z += b.Z;
            this.W += b.W;
        }

        public Vector4D AddedWith(Vector4D b) {
            Vector4D added = new Vector4D(this);
            added.Add(b);
            return b;
        }

        public void MultiplyBy(float factor) {
            X *= factor;
            Y *= factor;
            Z *= factor;
            W *= factor;
        }

        public Vector4D MultipliedBy(float factor) {
            Vector4D multiplied = new Vector4D(this);
            multiplied.MultiplyBy(factor);
            return multiplied;
        }
    }
}