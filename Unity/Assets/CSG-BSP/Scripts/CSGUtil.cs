using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG
{
	public class CSGUtil 
	{
		public static Mesh FromBSPtree(BSPTree tree)
		{
			if(tree == null) return null;

			List<Triangle> allTriangles = tree.GetAllTriangles();

			Vector3[] vertices = new Vector3[allTriangles.Count * 3];
			Vector3[] normals = new Vector3[allTriangles.Count * 3];
			Vector4[] tangents = new Vector4[allTriangles.Count * 3];
			GetVertexAttributes(allTriangles, vertices, normals, tangents);

			Mesh mesh = new Mesh();

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.tangents = tangents;

			return mesh;
		}

		private static void GetVertexAttributes(List<Triangle> allTriangles, Vector3[] positions, Vector3[] normals, Vector4[] tangents)
		{
			if(allTriangles == null)return;

			int vertexIndex = 0;
			for(int t = 0; t < allTriangles.Count; t++)
			{
				Triangle tri = allTriangles[t];

				positions[vertexIndex] = ConvertToUnity(tri.A.Position);
                positions[vertexIndex+1] = ConvertToUnity(tri.B.Position);
                positions[vertexIndex+2] = ConvertToUnity(tri.C.Position);

                normals[vertexIndex] = ConvertToUnity(tri.A.Normal);
                normals[vertexIndex+1] = ConvertToUnity(tri.B.Normal);
                normals[vertexIndex+2] = ConvertToUnity(tri.C.Normal);

                tangents[vertexIndex] = ConvertToUnity(tri.A.Tangent);
                tangents[vertexIndex+1] = ConvertToUnity(tri.B.Tangent);
                tangents[vertexIndex+2] = ConvertToUnity(tri.C.Tangent);

				vertexIndex += 3;
			}
		}

		public static BSPTree FromMesh(Mesh mesh)
		{
			return FromMesh(mesh, new Matrix4x4());
		}

		public static BSPTree FromMesh(Mesh mesh, Matrix4x4 transform)
		{
			BSPTree tree = new BSPTree();

			List<Triangle> meshTriangles = GetMeshTriangles(mesh);

			for(int i = 0; i < meshTriangles.Count; i++)
			{
				Triangle tri = meshTriangles[i];	

				for(int vi = 0; vi < 3; vi++)
				{
					Vertex vtx = TransformVertex(tri.GetVertexByIndex(vi), transform);
					tri.SetVertexByIndex(vi, vtx);
				}

				tri.RebuildOrientationPlane();
				meshTriangles[i] = tri;
			}

			tree.AddTriangles(meshTriangles);
			return tree;
		}

		public static List<Triangle> GetMeshTriangles(Mesh mesh)
		{
			List<Triangle> triangles = new List<Triangle>();

			for(int i = 0; i < mesh.subMeshCount; i++)
			{
				AddSubMeshTriangles(mesh, i, triangles);
			}

			return triangles;
		}

		public static Vertex TransformVertex(Vertex vertex, Matrix4x4 transform)
		{
			Vector3 uVector = new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
			uVector = transform * uVector;
			vertex.Position = new Vector3D(uVector.x, uVector.y, uVector.z);

			uVector = new Vector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
			uVector = transform * uVector;
			vertex.Normal = new Vector3D(uVector.x, uVector.y, uVector.z);

			Vector4 tVector = new Vector4(vertex.Tangent.X, vertex.Tangent.Y, vertex.Tangent.Z, vertex.Tangent.W);
			tVector = transform * tVector;
			vertex.Tangent = new Vector4D(tVector.x, tVector.y, tVector.z, tVector.w);

			return vertex;
		}

		private static void AddSubMeshTriangles(Mesh mesh, int index, List<Triangle> dest)
		{
			int[] indices = mesh.GetTriangles(index);
			
			for(int vi = 0; vi < indices.Length; vi+= 3)
			{
				Vector3 v1 = mesh.vertices[indices[vi]];
				Vector3 v2 = mesh.vertices[indices[vi + 1]];
				Vector3 v3 = mesh.vertices[indices[vi + 2]];

				Vector3 n1 = mesh.normals[indices[vi]];
				Vector3 n2 = mesh.normals[indices[vi + 1]];
				Vector3 n3 = mesh.normals[indices[vi + 2]];

				Vector4 t1 = mesh.tangents[indices[vi]];
				Vector4 t2 = mesh.tangents[indices[vi + 1]];
				Vector4 t3 = mesh.tangents[indices[vi + 2]];
				
				Vector3D csgV1 = new Vector3D(v1.x, v1.y, v1.z);
				Vector3D csgV2 = new Vector3D(v2.x, v2.y, v2.z);
				Vector3D csgV3 = new Vector3D(v3.x, v3.y, v3.z);

				Vector3D csgN1 = new Vector3D(n1.x, n1.y, n1.z);
				Vector3D csgN2 = new Vector3D(n2.x, n2.y, n2.z);
				Vector3D csgN3 = new Vector3D(n3.x, n3.y, n3.z);

				Vector4D csgT1 = new Vector4D(t1.x, t1.y, t1.z, t1.w);
				Vector4D csgT2 = new Vector4D(t2.x, t2.y, t2.z, t2.w);
				Vector4D csgT3 = new Vector4D(t3.x, t3.y, t3.z, t3.w);
				
				Vertex vtx1 = new Vertex();
				vtx1.Position = csgV1;
				vtx1.Normal = csgN1;
				vtx1.Tangent = csgT1;
				Vertex vtx2 = new Vertex();
				vtx2.Position = csgV2;
				vtx2.Normal = csgN2;
				vtx2.Tangent = csgT2;
				Vertex vtx3 = new Vertex();
				vtx3.Position = csgV3;
				vtx3.Normal = csgN3;
				vtx3.Tangent = csgT3;
				
				Triangle tri = new Triangle(vtx1, vtx2, vtx3);
				dest.Add(tri);
			}
		}

		private static Vector3 ConvertToUnity(Vector3D vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}

		private static Vector4 ConvertToUnity(Vector4D vector)
		{
			return new Vector4(vector.X, vector.Y, vector.Z, vector.W);
		}
	}
}
