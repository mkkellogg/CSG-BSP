using UnityEngine;
using System.Collections;

namespace CSG {

    public class Plane {
        private static Vector3D tempVectorA = new Vector3D();
        private static Vector3D tempVectorB = new Vector3D();
        private static Vector3D tempVectorC = new Vector3D();

        public Vector3D Normal;
        public float D;
        public Box3D PlaneBounds;

        public Plane(Vector3D normal, float d, Box3D bounds = null) {
            PlaneBounds = bounds != null ? new Box3D(bounds) : new Box3D();
            Normal = new Vector3D(normal);
            D = d;
        }

        public Plane(Plane src) {
            PlaneBounds = new Box3D(src.PlaneBounds);
            Normal = new Vector3D(src.Normal);
            D = 0;
        }

        public Plane() {
            PlaneBounds = new Box3D();
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

            float padding = 0.15f;

            outPlane.PlaneBounds.min.X = Mathf.Min(a.X, b.X, c.X) - padding;
            outPlane.PlaneBounds.min.Y = Mathf.Min(a.Y, b.Y, c.Y) - padding;
            outPlane.PlaneBounds.min.Z = Mathf.Min(a.Z, b.Z, c.Z) - padding;

            outPlane.PlaneBounds.max.X = Mathf.Max(a.X, b.X, c.X) + padding;
            outPlane.PlaneBounds.max.Y = Mathf.Max(a.Y, b.Y, c.Y) + padding;
            outPlane.PlaneBounds.max.Z = Mathf.Max(a.Z, b.Z, c.Z) + padding;

        }
    }

}