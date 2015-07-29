using UnityEngine;
using System.Collections;

namespace CSG
{
	public class UnshareNormals : MonoBehaviour 
	{
		void Start () 
		{
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

			if(meshFilter != null)
			{
				Mesh mesh = meshFilter.mesh;
				if(mesh != null)
				{
					mesh.RecalculateNormals(0);
				}
			}

		}	
	}
}
