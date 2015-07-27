using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubtractionTest : MonoBehaviour 
{
	public GameObject TargetMesh;
	public Transform ResultPosition;
	public Transform Anchor;

	void Start () 
	{
		if(!TargetMesh.activeSelf)return;

		MeshFilter thisFilter = GetComponent<MeshFilter>();
		MeshFilter targetFilter = TargetMesh.GetComponent<MeshFilter>();

		MeshRenderer thisRenderer = GetComponent<MeshRenderer>();

		Transform thisTransform = transform;
		Transform targetTransform = TargetMesh.transform;

		Mesh thisMesh = thisFilter.mesh;
		Mesh targetMesh = targetFilter.mesh;

		float startTime = Time.realtimeSinceStartup;
			CSG.BSPTree thisTree = CSG.CSGUtil.FromMesh(thisMesh, thisTransform.localToWorldMatrix);
			CSG.BSPTree targetTree = CSG.CSGUtil.FromMesh(targetMesh, targetTransform.localToWorldMatrix);
		float bspCreateTime = Time.realtimeSinceStartup - startTime;

		startTime = Time.realtimeSinceStartup;
			CSG.BSPTree thisClone = thisTree.Clone();
			CSG.BSPTree targetClone = targetTree.Clone();
		float cloneTime = Time.realtimeSinceStartup - startTime;

		startTime = Time.realtimeSinceStartup;
			List<CSG.Triangle> thisTriangles = thisTree.GetAllTriangles();
			List<CSG.Triangle> targetTriangles = targetTree.GetAllTriangles();
		float getAlltrianglesTime = Time.realtimeSinceStartup - startTime;

		startTime = Time.realtimeSinceStartup;
			CSG.BSPTree subtracted = CSG.Operations.Subtract(thisTree, targetTree);
		float subtractionTime = Time.realtimeSinceStartup - startTime;

		List<CSG.Triangle> subtractedTriangles = subtracted.GetAllTriangles();

		string debugLine1 = "SUBTRACTION  TEST: " + this.name + " BSP tree triangle count: " + thisTriangles.Count+
							", target ("+TargetMesh.name+") BSP tree triangle count: "+targetTriangles.Count+
							", >> subtraction result BSP tree triangle count: " + subtractedTriangles.Count;
		
		string debugLine2 = "BSP creation time: " + bspCreateTime + ", BSP clone time: " + cloneTime + 
							", all triangles: " + getAlltrianglesTime + ", subtraction time: "+subtractionTime;
		
		Debug.Log(debugLine1 + "\n" + debugLine2);
		
		Mesh subtractedMesh = CSG.CSGUtil.FromBSPtree(subtracted);
		GameObject subtractedObject = new GameObject();
		MeshFilter subtractedFilter = subtractedObject.AddComponent<MeshFilter>();
		MeshRenderer subtrctedRenderer = subtractedObject.AddComponent<MeshRenderer>();
		subtrctedRenderer.material = thisRenderer.material;
		subtractedFilter.mesh = subtractedMesh;
		Vector3 translateVector = ResultPosition.position - Anchor.position;
		subtractedObject.transform.Translate (translateVector);
	}

	void Update () 
	{
	
	}
}
