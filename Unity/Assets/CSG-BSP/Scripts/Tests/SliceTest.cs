using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG {
    public class SliceTest : BaseTest {
        public GameObject MeshA;
        public GameObject MeshB;
        public Transform ResultPosition;
        public Transform Anchor;

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
            CSG.BSPTree cloneA = meshATree.Clone();
            CSG.BSPTree cloneB = meshBTree.Clone();
            float cloneTime = Time.realtimeSinceStartup - startTime;

            startTime = Time.realtimeSinceStartup;
            List<CSG.Triangle> meshATriangles = meshATree.GetAllTriangles();
            List<CSG.Triangle> meshBTriangles = meshBTree.GetAllTriangles();
            float getAlltrianglesTime = Time.realtimeSinceStartup - startTime;

            startTime = Time.realtimeSinceStartup;
            List<CSG.Triangle> slicedHalf1triangles;
            List<CSG.Triangle> slicedHalf2triangles;
            CSG.Operations.FastSlice(meshATree, meshBTree, out slicedHalf1triangles, out slicedHalf2triangles);
            float sliceTime = Time.realtimeSinceStartup - startTime;

            string debugLine1 = "SLICE TEST: " + MeshA.name + " BSP tree triangle count: " + meshATriangles.Count +
                                ", target (" + MeshB.name + ") BSP tree triangle count: " + meshBTriangles.Count +
                                ", >> slice result BSP tree triangle count: " + slicedHalf1triangles.Count;

            string debugLine2 = "BSP creation time: " + bspCreateTime + ", BSP clone time: " + cloneTime +
                                ", all triangles: " + getAlltrianglesTime + ", slice time: " + sliceTime;

            Debug.Log(debugLine1 + "\n" + debugLine2);

            Mesh slicedHalf1Mesh = CSG.CSGUtil.FromList(slicedHalf1triangles);
            GameObject slicedHalf1Object = new GameObject();
            MeshFilter slicedHalf1Filter = slicedHalf1Object.AddComponent<MeshFilter>();
            MeshRenderer slicedHalf1Renderer = slicedHalf1Object.AddComponent<MeshRenderer>();
            slicedHalf1Renderer.material = meshARenderer.material;
            slicedHalf1Filter.mesh = slicedHalf1Mesh;
            Vector3 translateVector = ResultPosition.position - Anchor.position;
            slicedHalf1Object.transform.Translate(translateVector);
            slicedHalf1Object.transform.Translate(0.5f, 0, 0);

            Mesh slicedHalf2Mesh = CSG.CSGUtil.FromList(slicedHalf2triangles);
            GameObject slicedHalf2Object = new GameObject();
            MeshFilter slicedHalf2Filter = slicedHalf2Object.AddComponent<MeshFilter>();
            MeshRenderer slicedHalf2Renderer = slicedHalf2Object.AddComponent<MeshRenderer>();
            slicedHalf2Renderer.material = meshARenderer.material;
            slicedHalf2Filter.mesh = slicedHalf2Mesh;
            translateVector = ResultPosition.position - Anchor.position;
            slicedHalf2Object.transform.Translate(translateVector);
            slicedHalf2Object.transform.Translate(-0.5f, 0, 0);
        }

        void Update() {

        }
    }
}
