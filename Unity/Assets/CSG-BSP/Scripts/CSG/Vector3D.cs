using UnityEngine;
using System.Collections;

namespace CSG {
    public class Vector3D {
        public float X;
        public float Y;
        public float Z;

        public Vector3D(float x, float y, float z) {
            this.Set(x, y, z);
        }

        public Vector3D() {
            this.Set(0, 0, 0);
        }

        public Vector3D(Vector3D src) {
            this.Copy(src);
        }

        public void Set(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        public void Copy(Vector3D src) {
            this.Set(src.X, src.Y, src.Z);
        }

        public Vector3D Clone() {
            return new Vector3D(this);
        }

        public static float Dot(Vector3D a, Vector3D b) {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public float Dot(Vector3D b) {
            return Dot(this, b);
        }

        public static Vector3D Cross(Vector3D a, Vector3D b, Vector3D outVector) {
            outVector.X = a.Y * b.Z - a.Z * b.Y;
            outVector.Y = a.Z * b.X - a.X * b.Z;
            outVector.Z = a.X * b.Y - a.Y * b.X;
            return outVector;
        }

        public Vector3D Cross(Vector3D b, Vector3D outVector) {
            return Cross(this, b, outVector);
        }

        public float Magnitude() {
            return Mathf.Sqrt(Dot(this, this));
        }

        public void Normalize() {
            float mag = Magnitude();
            if (mag == 0.0f) return;
            MultiplyBy(1.0f / mag);
        }

        public Vector3D Normalized(Vector3D outVector) {
            outVector.Copy(this);
            outVector.Normalize();
            return outVector;
        }

        public void Invert() {
            X = -X;
            Y = -Y;
            Z = -Z;
        }

        public Vector3D Inverted(Vector3D outVector) {
            outVector.Copy(this);
            outVector.Invert();
            return outVector;
        }

        public void Lerp(Vector3D dest, float t) {
            this.X = (1 - t) * this.X + t * dest.X;
            this.Y = (1 - t) * this.Y + t * dest.Y;
            this.Z = (1 - t) * this.Z + t * dest.Z;
        }

        public Vector3D Lerped(Vector3D dest, float t, Vector3D outVector) {
            outVector.Copy(this);
            outVector.Lerp(dest, t);
            return outVector;
        }

        public void Subtract(Vector3D b) {
            this.X -= b.X;
            this.Y -= b.Y;
            this.Z -= b.Z;
        }

        public Vector3D SubtractedBy(Vector3D b, Vector3D outVector) {
            outVector.Copy(this);
            outVector.Subtract(b);
            return outVector;
        }

        public void Add(Vector3D b) {
            this.X += b.X;
            this.Y += b.Y;
            this.Z += b.Z;
        }

        public Vector3D AddedWith(Vector3D b, Vector3D outVector) {
            outVector.Copy(this);
            outVector.Add(b);
            return outVector;
        }

        public void MultiplyBy(float factor) {
            X *= factor;
            Y *= factor;
            Z *= factor;
        }

        public Vector3D MultipliedBy(float factor, Vector3D outVector) {
            outVector.Copy(this);
            outVector.MultiplyBy(factor);
            return outVector;
        }
    }
}
