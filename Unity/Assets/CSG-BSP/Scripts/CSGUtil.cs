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

namespace CSG
{
	public class CSGUtil 
	{
		/**
		 * Create a Mesh instance from @tree.
		 */
		public static Mesh FromBSPtree(BSPTree tree)
		{
			if(tree == null) return null;

			List<Triangle> allTriangles = tree.GetAllTriangles();

			// create the various attribute arrays required by a Mesh object
			Vector3[] vertices = new Vector3[allTriangles.Count * 3];
			Vector3[] normals = new Vector3[allTriangles.Count * 3];
			Vector4[] tangents = new Vector4[allTriangles.Count * 3];
			Vector2[] uvs1 = new Vector2[allTriangles.Count * 3];

			// pull the attributes from @tree and put them into the above arrays
			GetVertexAttributes(allTriangles, vertices, normals, tangents, uvs1);

			Mesh mesh = new Mesh();

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.tangents = tangents;
			mesh.uv = uvs1;

			// copy vertex positions the mesh's 'triangles' array
			int[] triangles = new int[allTriangles.Count * 3];
			for(int i=0; i < vertices.Length; i++)
			{
				triangles[i] = i;
			}
			mesh.triangles = triangles;

			mesh.RecalculateNormals();

			return mesh;
		}

		/**
		 * Pull vertex attributes from each vertex in each triangle in @allTriangles, and copy
		 * them into the appropriate array parameter.
		 */ 
		private static void GetVertexAttributes(List<Triangle> allTriangles, Vector3[] positions, Vector3[] normals, Vector4[] tangents, Vector2[] uvs1)
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

				uvs1[vertexIndex] = ConvertToUnity(tri.A.UV1);
				uvs1[vertexIndex+1] = ConvertToUnity(tri.B.UV1);
				uvs1[vertexIndex+2] = ConvertToUnity(tri.C.UV1);

				vertexIndex += 3;
			}
		}

		/**
		 * Create a new BSPTree instance from @mesh using the Identity matrix
		 * to transform all vertices, normals, and tangents.
		 */ 
		public static BSPTree FromMesh(Mesh mesh)
		{
			return FromMesh(mesh, new Matrix4x4());
		}

		/**
		 * Create a new BSPTree instance from @mesh using @transform
		 * to transform all vertices, normals, and tangents.
		 */
		public static BSPTree FromMesh(Mesh mesh, Matrix4x4 transform)
		{
			BSPTree tree = new BSPTree();

			// get a list of all triangles in @mesh
			List<Triangle> meshTriangles = GetMeshTriangles(mesh);

			// loop through each triangle and transform its vertices by @transform
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

		/**
		 * Transform the relevant attributes of @vertex by @transform.
		 */ 
		public static Vertex TransformVertex(Vertex vertex, Matrix4x4 transform)
		{
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
		public static List<Triangle> GetMeshTriangles(Mesh mesh)
		{
			List<Triangle> triangles = new List<Triangle>();

			for(int i = 0; i < mesh.subMeshCount; i++)
			{
				AddSubMeshTriangles(mesh, i, triangles);
			}

			return triangles;
		}

		/**
		 * 
		 * Create a list of Triangle instances from the vertex data in the sub-mesh
		 * of @mesh specified by @index, and place them in @dest.
		 */
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

				Vector2 uv1 = mesh.uv[indices[vi]];
				Vector2 uv2 = mesh.uv[indices[vi + 1]];
				Vector2 uv3 = mesh.uv[indices[vi + 2]];
				
				Vector3D csgV1 = new Vector3D(v1.x, v1.y, v1.z);
				Vector3D csgV2 = new Vector3D(v2.x, v2.y, v2.z);
				Vector3D csgV3 = new Vector3D(v3.x, v3.y, v3.z);

				Vector3D csgN1 = new Vector3D(n1.x, n1.y, n1.z);
				Vector3D csgN2 = new Vector3D(n2.x, n2.y, n2.z);
				Vector3D csgN3 = new Vector3D(n3.x, n3.y, n3.z);

				Vector4D csgT1 = new Vector4D(t1.x, t1.y, t1.z, t1.w);
				Vector4D csgT2 = new Vector4D(t2.x, t2.y, t2.z, t2.w);
				Vector4D csgT3 = new Vector4D(t3.x, t3.y, t3.z, t3.w);

				UV csgUV1 = new UV(uv1.x, uv1.y);
				UV csgUV2 = new UV(uv2.x, uv2.y);
				UV csgUV3 = new UV(uv3.x, uv3.y);
				
				Vertex vtx1 = new Vertex();
				vtx1.Position = csgV1;
				vtx1.Normal = csgN1;
				vtx1.Tangent = csgT1;
				vtx1.UV1 = csgUV1;

				Vertex vtx2 = new Vertex();
				vtx2.Position = csgV2;
				vtx2.Normal = csgN2;
				vtx2.Tangent = csgT2;
				vtx2.UV1 = csgUV2;

				Vertex vtx3 = new Vertex();
				vtx3.Position = csgV3;
				vtx3.Normal = csgN3;
				vtx3.Tangent = csgT3;
				vtx3.UV1 = csgUV3;
				
				Triangle tri = new Triangle(vtx1, vtx2, vtx3);
				dest.Add(tri);
			}
		}

		private static Vector2 ConvertToUnity(UV vector)
		{
			return new Vector2(vector.U, vector.V);
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
