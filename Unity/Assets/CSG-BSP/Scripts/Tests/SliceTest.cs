using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SliceTest : MonoBehaviour 
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
			CSG.BSPTree cloneA = thisTree.Clone();
			CSG.BSPTree cloneB = targetTree.Clone();
		float cloneTime = Time.realtimeSinceStartup - startTime;

		startTime = Time.realtimeSinceStartup;
			List<CSG.Triangle> thisTriangles = thisTree.GetAllTriangles();
			List<CSG.Triangle> targetTriangles = targetTree.GetAllTriangles();
		float getAlltrianglesTime = Time.realtimeSinceStartup - startTime;

		startTime = Time.realtimeSinceStartup;
			List<CSG.Triangle> slicedHalf1triangles;
			List<CSG.Triangle> slicedHalf2triangles;
			CSG.Operations.FastSlice(thisTree, targetTree, out slicedHalf1triangles, out slicedHalf2triangles);
		float sliceTime =  Time.realtimeSinceStartup - startTime;

		string debugLine1 = "SLICE TEST: " + this.name + " BSP tree triangle count: " + thisTriangles.Count+
							", target ("+TargetMesh.name+") BSP tree triangle count: "+targetTriangles.Count+
							", >> slice result BSP tree triangle count: " + slicedHalf1triangles.Count;

		string debugLine2 = "BSP creation time: " + bspCreateTime + ", BSP clone time: " + cloneTime + 
							", all triangles: " + getAlltrianglesTime + ", slice time: "+sliceTime;

		Debug.Log(debugLine1 + "\n" + debugLine2);

		Mesh slicedHalf1Mesh = CSG.CSGUtil.FromList(slicedHalf1triangles);
		GameObject slicedHalf1Object = new GameObject();
		MeshFilter slicedHalf1Filter = slicedHalf1Object.AddComponent<MeshFilter>();
		MeshRenderer  slicedHalf1Renderer = slicedHalf1Object.AddComponent<MeshRenderer>();
		slicedHalf1Renderer.material = thisRenderer.material;
		slicedHalf1Filter.mesh =  slicedHalf1Mesh;
		Vector3 translateVector = ResultPosition.position - Anchor.position;
		slicedHalf1Object.transform.Translate (translateVector);
		slicedHalf1Object.transform.Translate(-0.5f,0,0);

		Mesh slicedHalf2Mesh = CSG.CSGUtil.FromList(slicedHalf2triangles);
		GameObject slicedHalf2Object = new GameObject();
		MeshFilter slicedHalf2Filter = slicedHalf2Object.AddComponent<MeshFilter>();
		MeshRenderer  slicedHalf2Renderer = slicedHalf2Object.AddComponent<MeshRenderer>();
		slicedHalf2Renderer.material = thisRenderer.material;
		slicedHalf2Filter.mesh =  slicedHalf2Mesh;
		translateVector = ResultPosition.position - Anchor.position; 
		slicedHalf2Object.transform.Translate (translateVector);
		slicedHalf2Object.transform.Translate(0.5f,0,0);
	}

	void Update () 
	{
	
	}
}
