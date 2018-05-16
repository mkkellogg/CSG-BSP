using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG {
    public class SubtractionTest : BaseTest {
        public GameObject MeshA;
        public GameObject MeshB;
        public Transform ResultPosition;
        public Transform Anchor;

        void Start() {

        }

        public override void RunTest() {
            if (MeshA == null || MeshB == null) return;
            if (!MeshA.activeSelf) return;
            if (!MeshB.activeSelf) return;

            MeshFilter meshAFilter = MeshA.GetComponent<MeshFilter>();
            MeshFilter meshBFilter = MeshB.GetComponent<MeshFilter>();

            MeshRenderer meshARenderer = MeshA.GetComponent<MeshRenderer>();

            Transform meshATransform = MeshA.transform;
            Transform meshBTransform = MeshB.transform;

            Mesh meshA = meshAFilter.mesh;
            Mesh meshB = meshBFilter.mesh;

            float startTime = Time.realtimeSinceStartup;
            CSG.BSPTree meshATree = CSG.CSGUtil.FromMesh(meshA, meshATransform.localToWorldMatrix);
            CSG.BSPTree meshBTree = CSG.CSGUtil.FromMesh(meshB, meshBTransform.localToWorldMatrix);
            float bspCreateTime = Time.realtimeSinceStartup - startTime;

            startTime = Time.realtimeSinceStartup;
            CSG.BSPTree meshATreeClone = meshATree.Clone();
            CSG.BSPTree meshBTreeClone = meshBTree.Clone();
            float cloneTime = Time.realtimeSinceStartup - startTime;

            startTime = Time.realtimeSinceStartup;
            List<CSG.Triangle> meshATriangles = meshATree.GetAllTriangles();
            List<CSG.Triangle> meshBTriangles = meshBTree.GetAllTriangles();
            float getAlltrianglesTime = Time.realtimeSinceStartup - startTime;

            startTime = Time.realtimeSinceStartup;
            CSG.BSPTree subtracted = CSG.Operations.Subtract(meshATree, meshBTree);
            float subtractionTime = Time.realtimeSinceStartup - startTime;

            List<CSG.Triangle> subtractedTriangles = subtracted.GetAllTriangles();

            string debugLine1 = "SUBTRACTION  TEST: " + MeshA.name + " BSP tree triangle count: " + meshATriangles.Count +
                                ", target (" + MeshB.name + ") BSP tree triangle count: " + meshBTriangles.Count +
                                ", >> subtraction result BSP tree triangle count: " + subtractedTriangles.Count;

            string debugLine2 = "BSP creation time: " + bspCreateTime + ", BSP clone time: " + cloneTime +
                                ", all triangles: " + getAlltrianglesTime + ", subtraction time: " + subtractionTime;

            Debug.Log(debugLine1 + "\n" + debugLine2);

            Mesh subtractedMesh = CSG.CSGUtil.FromBSPtree(subtracted);
            GameObject subtractedObject = new GameObject();
            MeshFilter subtractedFilter = subtractedObject.AddComponent<MeshFilter>();
            MeshRenderer subtrctedRenderer = subtractedObject.AddComponent<MeshRenderer>();
            subtrctedRenderer.material = meshARenderer.material;
            subtractedFilter.mesh = subtractedMesh;
            Vector3 translateVector = ResultPosition.position - Anchor.position;
            subtractedObject.transform.Translate(translateVector);
        }

        void Update() {

        }
    }
}
