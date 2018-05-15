using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG {
    public class Triangle {
        public Plane OrientationPlane;
        public Vertex A;
        public Vertex B;
        public Vertex C;

        public Triangle(Vertex a, Vertex b, Vertex c) {
            OrientationPlane = new Plane();
            Plane.BuildFromVertices(a.Position, b.Position, c.Position, OrientationPlane);
            A = new Vertex(a);
            B = new Vertex(b);
            C = new Vertex(c);
        }

        public Triangle(Triangle triangle) {
            OrientationPlane = new Plane(triangle.OrientationPlane);
            A = new Vertex(triangle.A);
            B = new Vertex(triangle.B);
            C = new Vertex(triangle.C);
        }

        public Triangle() {
            OrientationPlane = new Plane();
            A = new Vertex();
            B = new Vertex();
            C = new Vertex();
        }

        public void Invert() {
            OrientationPlane.Invert();

            A.Invert();
            B.Invert();
            C.Invert();

            Vertex temp = A;
            A = C;
            C = temp;
        }

        public Vertex GetVertexByIndex(int index) {
            switch (index) {
                case 1:
                    return B;
                case 2:
                    return C;
                default:
                    return A;
            }
        }

        public void SetVertexByIndex(int index, Vertex vertex) {
            switch (index) {
                case 1:
                    B = vertex;
                    break;
                case 2:
                    C = vertex;
                    break;
                default:
                    A = vertex;
                    break;
            }
        }

        public void RebuildOrientationPlane() {
            Plane.BuildFromVertices(A.Position, B.Position, C.Position, OrientationPlane);
        }
    }
}
