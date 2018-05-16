/**
 * 
 * class: CSGUtil
 * 
 * Author: Mark Kellogg
 * 
 * Contains utility functions for converting from BSPTrees to meshes
 * and vice-versa.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG {
    public class CSGUtil {
        /**
         * Create a Mesh instance from @tree.
         */
        public static Mesh FromBSPtree(BSPTree tree) {
            if (tree == null) return null;
            return FromList(tree.GetAllTriangles());
        }

        public static Mesh FromList(List<Triangle> allTriangles) {
            if (allTriangles == null) return null;

            // create the various attribute arrays required by a Mesh object
            Vector3[] positions = null;
            Vector2[] uvs1 = null;
            int[] triangles = null;

            // pull the attributes from @tree and put them into the above arrays
            GetVertexAttributes(allTriangles, out positions, out uvs1, out triangles);

            Mesh mesh = new Mesh();

            mesh.vertices = positions;
            mesh.uv = uvs1;

            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

        /**
         * Pull vertex attributes from each vertex in each triangle in @allTriangles, and copy
         * them into the appropriate array parameter.
         */
        private static void GetVertexAttributes(List<Triangle> allTriangles, out Vector3[] positions, out Vector2[] uvs1, out int[] triangles) {
            positions = null;
            uvs1 = null;
            triangles = null;

            if (allTriangles == null) return;

            Dictionary<Vector3, int> positionMap = new Dictionary<Vector3, int>();

            List<Vector3> uniquePositions = new List<Vector3>();
            List<Vector2> uniqueUVs = new List<Vector2>();
            List<int> triangleList = new List<int>();

            int vertexIndex = 0;
            int[] tempIndices = new int[3];
            Vector3[] tempPositions = new Vector3[3];
            Vector2[] tempUVs = new Vector2[3];

            for (int t = 0; t < allTriangles.Count; t++) {
                Triangle tri = allTriangles[t];

                tempPositions[0] = ConvertToUnity(tri.A.Position);
                tempPositions[1] = ConvertToUnity(tri.B.Position);
                tempPositions[2] = ConvertToUnity(tri.C.Position);

                tempUVs[0] = ConvertToUnity(tri.A.UV1);
                tempUVs[1] = ConvertToUnity(tri.B.UV1);
                tempUVs[2] = ConvertToUnity(tri.C.UV1);

                for (int i = 0; i < 3; i++) {
                    Vector3 cp = tempPositions[i];
                    Vector3 cuv = tempUVs[i];
                    tempIndices[i] = uniquePositions.Count;
                    uniquePositions.Add(cp);
                    uniqueUVs.Add(cuv);
                }

                triangleList.Add(tempIndices[0]);
                triangleList.Add(tempIndices[1]);
                triangleList.Add(tempIndices[2]);

                vertexIndex += 3;
            }

            positions = uniquePositions.ToArray();
            uvs1 = uniqueUVs.ToArray();
            triangles = triangleList.ToArray();
        }

        /**
         * Create a new BSPTree instance from @mesh using the Identity matrix
         * to transform all vertices, normals, and tangents.
         */
        public static BSPTree FromMesh(Mesh mesh) {
            return FromMesh(mesh, new Matrix4x4());
        }

        /**
         * Create a new BSPTree instance from @mesh using @transform
         * to transform all vertices, normals, and tangents.
         */
        public static BSPTree FromMesh(Mesh mesh, Matrix4x4 transform) {
            BSPTree tree = new BSPTree();

            // get a list of all triangles in @mesh
            List<Triangle> meshTriangles = GetMeshTriangles(mesh);

            // loop through each triangle and transform its vertices by @transform
            for (int i = 0; i < meshTriangles.Count; i++) {
                Triangle tri = meshTriangles[i];

                for (int vi = 0; vi < 3; vi++) {
                    Vertex vtx = TransformVertex(tri.GetVertexByIndex(vi), transform);
                    tri.SetVertexByIndex(vi, vtx);
                }

                tri.RebuildOrientationPlane();
                meshTriangles[i] = tri;
            }

            tree.AddTriangles(meshTriangles);
            return tree;
        }

        /**
         * Transform the relevant attributes of @vertex by @transform.
         */
        public static Vertex TransformVertex(Vertex vertex, Matrix4x4 transform) {
            // transform vertex position
            Vector4 uVector = new Vector4(vertex.Position.X, vertex.Position.Y, vertex.Position.Z, 1);
            uVector = transform * uVector;
            vertex.Position = new Vector3D(uVector.x, uVector.y, uVector.z);

            // transform vertex normal
            uVector = new Vector4(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z, 0);
            uVector = transform * uVector;
            vertex.Normal = new Vector3D(uVector.x, uVector.y, uVector.z);

            // transform vertex tangent
            Vector4 tVector = new Vector4(vertex.Tangent.X, vertex.Tangent.Y, vertex.Tangent.Z, 0);
            tVector = transform * tVector;
            vertex.Tangent = new Vector4D(tVector.x, tVector.y, tVector.z, tVector.w);

            return vertex;
        }

        /**
         * Create a list of Triangle instances from the vertex data in @mesh.
         */
        public static List<Triangle> GetMeshTriangles(Mesh mesh) {
            List<Triangle> triangles = new List<Triangle>();

            for (int i = 0; i < mesh.subMeshCount; i++) {
                AddSubMeshTriangles(mesh, i, triangles);
            }

            return triangles;
        }

        /**
         * 
         * Create a list of Triangle instances from the vertex data in the sub-mesh
         * of @mesh specified by @index, and place them in @dest.
         */
        private static void AddSubMeshTriangles(Mesh mesh, int subMeshIndex, List<Triangle> dest) {
            int[] meshIndices = mesh.GetTriangles(subMeshIndex);
            Vector3[] meshVertices = mesh.vertices;
            Vector3[] meshNormals = mesh.normals;
            Vector4[] meshTangents = mesh.tangents;
            Vector2[] meshUVs = mesh.uv;

            for (int vi = 0; vi < meshIndices.Length; vi += 3) {
                Triangle tri = ConvertMeshTriangle(meshVertices, meshNormals, meshTangents, meshUVs, meshIndices, vi);
                dest.Add(tri);
            }
        }

        /**
         * Given @meshIndices, the array of vertex indices for @mesh, construct a triangle 
         * with all relevant vertex attribute data starting at the vertex pointed to by 
         * @index, using @index, @index+1, and @index+2 as the triangle's vertices.
         */
        private static Triangle ConvertMeshTriangle(Vector3[] meshVertices, Vector3[] meshNormals, Vector4[] meshTangents, Vector2[] meshUVs, int[] meshIndices, int index) {
            Vector3 v1 = meshVertices[meshIndices[index]];
            Vector3 v2 = meshVertices[meshIndices[index + 1]];
            Vector3 v3 = meshVertices[meshIndices[index + 2]];

            Vector3 n1 = meshNormals[meshIndices[index]];
            Vector3 n2 = meshNormals[meshIndices[index + 1]];
            Vector3 n3 = meshNormals[meshIndices[index + 2]];

            Vector4 t1 = meshNormals[meshIndices[index]];
            Vector4 t2 = meshNormals[meshIndices[index + 1]];
            Vector4 t3 = meshNormals[meshIndices[index + 2]];

            Vector2 uv1 = meshUVs[meshIndices[index]];
            Vector2 uv2 = meshUVs[meshIndices[index + 1]];
            Vector2 uv3 = meshUVs[meshIndices[index + 2]];

            Triangle tri = new Triangle();

            tri.A.Position.Set(v1.x, v1.y, v1.z);
            tri.A.Normal.Set(n1.x, n1.y, n1.z);
            tri.A.Tangent.Set(t1.x, t1.y, t1.z, t1.w);
            tri.A.UV1.Set(uv1.x, uv1.y);

            tri.B.Position.Set(v2.x, v2.y, v2.z);
            tri.B.Normal.Set(n2.x, n2.y, n2.z);
            tri.B.Tangent.Set(t2.x, t2.y, t2.z, t2.w);
            tri.B.UV1.Set(uv2.x, uv2.y);

            tri.C.Position.Set(v3.x, v3.y, v3.z);
            tri.C.Normal.Set(n3.x, n3.y, n3.z);
            tri.C.Tangent.Set(t3.x, t3.y, t3.z, t3.w);
            tri.C.UV1.Set(uv3.x, uv3.y);

            return tri;
        }

        private static Vector2 ConvertToUnity(UV vector) {
            return new Vector2(vector.U, vector.V);
        }

        private static Vector3 ConvertToUnity(Vector3D vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        private static Vector4 ConvertToUnity(Vector4D vector) {
            return new Vector4(vector.X, vector.Y, vector.Z, vector.W);
        }

        private class VectorComparer : IEqualityComparer<Vector3> {
            private static float epsilon = .0000001f;
            public bool Equals(Vector3 v1, Vector3 v2) {
                if (v1.x <= v2.x + epsilon && v1.x >= v2.x - epsilon &&
                    v1.y <= v2.y + epsilon && v1.y >= v2.y - epsilon &&
                    v1.z <= v2.z + epsilon && v1.z >= v2.z - epsilon) {
                    return true;
                }
                else {
                    return false;
                }
            }

            public int GetHashCode(Vector3 v) {
                int hCode = (int)v.x ^ (int)v.y ^ (int)v.z;
                return hCode.GetHashCode();
            }
        }
    }
}
