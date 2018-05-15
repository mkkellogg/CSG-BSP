using UnityEngine;
using System.Collections;

namespace CSG {
    public class Plane {
        private static Vector3D tempVectorA = new Vector3D();
        private static Vector3D tempVectorB = new Vector3D();
        private static Vector3D tempVectorC = new Vector3D();

        public Vector3D Normal;
        public float D;

        public Plane(Vector3D normal, float d) {
            Normal = new Vector3D(normal);
            D = d;
        }

        public Plane(Plane src) {
            Normal = new Vector3D(src.Normal);
            D = 0;
        }

        public Plane() {
            Normal = new Vector3D();
            D = 0;
        }

        public void Invert() {
            Normal.Invert();
            D = -D;
        }

        public static void BuildFromVertices(Vector3D a, Vector3D b, Vector3D c, Plane outPlane) {
            Vector3D edgeA = c.SubtractedBy(a, tempVectorA);
            Vector3D edgeB = b.SubtractedBy(a, tempVectorB);
            Vector3D cross = edgeA.Cross(edgeB, tempVectorC);

            // !! Important: inverted to be valid in left-handed space
            // TODO: make this work in either handed-ness automatically
            cross.Invert();
            outPlane.Normal.Copy(cross);
            outPlane.Normal.Normalize();
            outPlane.D = outPlane.Normal.Dot(a);
        }
    }
}