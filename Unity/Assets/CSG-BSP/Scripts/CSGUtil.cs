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
			return FromList(tree.GetAllTriangles());
		}

		public static Mesh FromList(List<Triangle> allTriangles)
		{
			if(allTriangles == null) return null;
			
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
		private static void GetVertexAttributes(List<Triangle> allTriangles, out Vector3[] positions, out Vector2[] uvs1, out int[] triangles)
		{
			positions = null;
			uvs1 = null;
			triangles = null;

			if(allTriangles == null)return;	

			//VectorComparer comparer = new VectorComparer();
			Dictionary<Vector3, int> positionMap = new Dictionary<Vector3, int>();

			List<Vector3> uniquePositions = new List<Vector3>();
			List<Vector2> uniqueUVs = new List<Vector2>();
			List<int> triangleList = new List<int>();

			int vertexIndex = 0;
			int[] tempIndices = new int[3];
			Vector3[] tempPositions = new Vector3[3];
			Vector2[] tempUVs = new Vector2[3];

			for(int t = 0; t < allTriangles.Count; t++)
			{
				Triangle tri = allTriangles[t];

				tempPositions[0] = ConvertToUnity(tri.A.Position);
				tempPositions[1] = ConvertToUnity(tri.B.Position);
				tempPositions[2]= ConvertToUnity(tri.C.Position);

				tempUVs[0] = ConvertToUnity(tri.A.UV1);
				tempUVs[1] = ConvertToUnity(tri.B.UV1);
				tempUVs[2] = ConvertToUnity(tri.C.UV1);

				for(int i =0; i < 3; i++)
				{
					Vector3 cp = tempPositions[i];
					Vector3 cuv = tempUVs[i];
					//if(positionMap.ContainsKey(cp))					
					//{
					//	tempIndices[i] = positionMap[cp];
					//}
					//else
					//{
						//positionMap.Add(cp, uniquePositions.Count);
						tempIndices[i] = uniquePositions.Count;
						uniquePositions.Add(cp);
						uniqueUVs.Add(cuv);
					//}
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
		private static void AddSubMeshTriangles(Mesh mesh, int subMeshIndex, List<Triangle> dest)
		{
			int[] indices = mesh.GetTriangles(subMeshIndex);
			
			for(int vi = 0; vi < indices.Length; vi+= 3)
			{
				Triangle tri = ConvertMeshTriangle(mesh, indices, vi);
				dest.Add(tri);
			}
		}

		/**
		 * Given @meshIndices, the array of vertex indices for @mesh, construct a triangle 
		 * with all relevant vertex attribute data starting at the vertex pointed to by 
		 * @index, using @index, @index+1, and @index+2 as the triangle's vertices.
		 */
		private static Triangle ConvertMeshTriangle(Mesh mesh, int[] meshIndices, int index)
		{
			Vector3 v1 = mesh.vertices[meshIndices[index]];
			Vector3 v2 = mesh.vertices[meshIndices[index + 1]];
			Vector3 v3 = mesh.vertices[meshIndices[index + 2]];
			
			Vector3 n1 = mesh.normals[meshIndices[index]];
			Vector3 n2 = mesh.normals[meshIndices[index + 1]];
			Vector3 n3 = mesh.normals[meshIndices[index + 2]];
			
			Vector4 t1 = mesh.tangents[meshIndices[index]];
			Vector4 t2 = mesh.tangents[meshIndices[index + 1]];
			Vector4 t3 = mesh.tangents[meshIndices[index + 2]];
			
			Vector2 uv1 = mesh.uv[meshIndices[index]];
			Vector2 uv2 = mesh.uv[meshIndices[index + 1]];
			Vector2 uv3 = mesh.uv[meshIndices[index + 2]];
			
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

			return tri;
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

		private class VectorComparer : IEqualityComparer<Vector3>
		{
			private static float epsilon = .0000001f;
			public bool Equals(Vector3 v1, Vector3 v2)
			{
				if (v1.x <= v2.x + epsilon && v1.x >= v2.x - epsilon &&
				    v1.y <= v2.y + epsilon && v1.y >= v2.y - epsilon &&
				    v1.z <= v2.z + epsilon && v1.z >= v2.z - epsilon)
				{
					return true;
				}
				else
				{
					return false;
				}

				return false;
			}
			
			
			public int GetHashCode(Vector3 v)
			{
				int hCode = (int)v.x ^ (int)v.y ^ (int)v.z;
				return hCode.GetHashCode();
			}	
		}
	}
}
