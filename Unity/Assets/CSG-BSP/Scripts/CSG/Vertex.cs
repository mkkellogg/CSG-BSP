using UnityEngine;
using System.Collections;

namespace CSG {
    public class Vertex {
        public Vector3D Position;
        public Vector3D Normal;
        public Vector4D Tangent;
        public UV UV1;
        public UV UV2;
        public Color4 Color;

        public Vertex() {
            Position = new Vector3D();
            Normal = new Vector3D();
            Tangent = new Vector4D();
            UV1 = new UV();
            UV2 = new UV();
            Color = new Color4();
        }

        public Vertex(Vertex vertex) {
            Position = new Vector3D(vertex.Position);
            Normal = new Vector3D(vertex.Normal);
            Tangent = new Vector4D(vertex.Tangent);
            UV1 = new UV(vertex.UV1);
            UV2 = new UV(vertex.UV2);
            Color = new Color4(vertex.Color);
        }

        public Vertex(Vector3D position, Vector3D normal, Vector4D tangent, UV uv1, UV uv2, Color4 color) {
            Position = new Vector3D(position);
            Normal = new Vector3D(normal);
            Tangent = new Vector4D(tangent);
            UV1 = new UV(uv1);
            UV2 = new UV(uv2);
            Color = new Color4(color);
        }

        public void Invert() {
            Normal.Invert();
        }

        public Vertex Lerped(Vertex dest, float t, Vertex outVertex) {
            outVertex.Position = Position.Lerped(dest.Position, t, outVertex.Position);
            outVertex.Normal = Normal.Lerped(dest.Normal, t, outVertex.Normal);
            outVertex.Tangent = Tangent.Lerped(dest.Tangent, t, outVertex.Tangent);
            outVertex.UV1 = UV1.Lerped(dest.UV1, t, outVertex.UV1);
            outVertex.UV2 = UV2.Lerped(dest.UV2, t, outVertex.UV2);
            outVertex.Color = Color.Lerped(dest.Color, t, outVertex.Color);
            return outVertex;
        }
    }
}
