using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicTest : MonoBehaviour 
{
	public GameObject TargetMesh;

	void Start () 
	{
		MeshFilter thisFilter = GetComponent<MeshFilter>();
		MeshFilter targetFilter = TargetMesh.GetComponent<MeshFilter>();

		Transform thisTransform = transform;
		Transform targetTransform = TargetMesh.transform;

		Mesh thisMesh = thisFilter.mesh;
		Mesh targetMesh = targetFilter.mesh;

		CSG.BSPTree thisTree = CSG.CSGUtil.FromMesh(thisMesh, thisTransform.localToWorldMatrix);
		CSG.BSPTree targetTree = CSG.CSGUtil.FromMesh(targetMesh, targetTransform.localToWorldMatrix);

		List<CSG.Triangle> thisTriangles = thisTree.GetAllTriangles();
		List<CSG.Triangle> targetTriangles = targetTree.GetAllTriangles();

		Debug.Log("This count: " + thisTriangles.Count);
		Debug.Log("Target count: " + targetTriangles.Count);

		/*CSG.BSPTree subtracted = CSG.BSPOperations.Subtract(thisTree, targetTree);
		List<CSG.Triangle> subtractedTriangles = subtracted.GetAllTriangles();
		Debug.Log("Subtracted count: " + subtractedTriangles.Count);*/

		thisTree.ClipByTree(targetTree);
		List<CSG.Triangle> clippedTriangles = thisTree.GetAllTriangles();
		Debug.Log("Clipped count: " + clippedTriangles.Count);
	}

	void Update () 
	{
	
	}
}
