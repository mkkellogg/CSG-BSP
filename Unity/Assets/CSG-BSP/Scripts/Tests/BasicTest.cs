﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicTest : MonoBehaviour 
{
	public GameObject TargetMesh;

	void Start () 
	{
		MeshFilter thisFilter = GetComponent<MeshFilter>();
		MeshFilter targetFilter = TargetMesh.GetComponent<MeshFilter>();

		MeshRenderer thisRenderer = GetComponent<MeshRenderer>();

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

		CSG.BSPTree subtracted = CSG.BSPOperations.Subtract(thisTree, targetTree);
		List<CSG.Triangle> subtractedTriangles = subtracted.GetAllTriangles();
		Debug.Log("Subtracted count: " + subtractedTriangles.Count);

		thisTree.ClipByTree(targetTree);
		List<CSG.Triangle> clippedTriangles = thisTree.GetAllTriangles();
		Debug.Log("Clipped count: " + clippedTriangles.Count);

		Mesh subtractedMesh = CSG.CSGUtil.FromBSPtree(subtracted);
		GameObject subtractedObject = new GameObject();
		MeshFilter subtractedFilter = subtractedObject.AddComponent<MeshFilter>();
		MeshRenderer subtrctedRenderer = subtractedObject.AddComponent<MeshRenderer>();
		subtrctedRenderer.material = thisRenderer.material;
		subtractedFilter.mesh = subtractedMesh;
		subtractedObject.transform.Translate(3,0,0);
		Debug.Log("subtractedMesh: "+subtractedMesh.subMeshCount+"," + subtractedMesh.GetTriangles(0).Length);
	}

	void Update () 
	{
	
	}
}