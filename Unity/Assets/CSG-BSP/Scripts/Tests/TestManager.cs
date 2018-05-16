using UnityEngine;
using System.Collections;

namespace CSG {
    public class TestManager : MonoBehaviour {
        public GameObject[] Tests;
        public GameObject Arrow;
        public GameObject MainCamera;
        public Transform BetweenTestsPosition;
        public GameObject NextTestText;
        public GameObject CurrentTestText;
        public GameObject ClickToContinueText;
        public float MinimumTimeBetweenTests = 1.5f;

        private UnityEngine.UI.Text currentTestText;
        private UnityEngine.UI.Text nextTestText;

        void Start() {
            currentTestText = CurrentTestText.GetComponent<UnityEngine.UI.Text>();
            nextTestText = NextTestText.GetComponent<UnityEngine.UI.Text>();
            StartCoroutine(RunAllTests());
        }

        private IEnumerator RunAllTests() {
            SwitchToInBetweenTests(1);
            yield return new WaitForSeconds(.05f);

            for (int i = 0; i < Tests.Length; i++) {
                float testStartTime = Time.realtimeSinceStartup;
                BaseTest bt = RunTest(i);
                float testTime = Time.realtimeSinceStartup - testStartTime;

                if (testTime < MinimumTimeBetweenTests) {
                    yield return new WaitForSeconds(MinimumTimeBetweenTests - testTime);
                }

                SwitchToTestResultMode(bt, i);

                while (!Input.GetMouseButtonDown(0)) {
                    yield return null;
                }
                yield return new WaitForSeconds(.05f);

                if (i < Tests.Length - 1) {
                    SwitchToInBetweenTests(i + 2);
                    yield return new WaitForSeconds(.05f);
                }
            }
        }

        private void SwitchToTestResultMode(BaseTest bt, int testIndex) {
            NextTestText.SetActive(false);
            if (testIndex < Tests.Length - 1) ClickToContinueText.SetActive(true);
            currentTestText.text = bt.TestName;
            MainCamera.transform.position = bt.CameraPosition.position;
            Arrow.transform.position = bt.CameraPosition.position;
            Vector3 cameraWorldForward = MainCamera.transform.localToWorldMatrix * Vector3.forward;
            Arrow.transform.Translate(cameraWorldForward * 5);
        }

        private void SwitchToInBetweenTests(int testNumber) {
            MainCamera.transform.position = BetweenTestsPosition.position;
            NextTestText.SetActive(true);
            nextTestText.text = "Preparing test " + testNumber;
            ClickToContinueText.SetActive(false);
            currentTestText.text = "";
        }

        private BaseTest RunTest(int index, bool enableClickToContinue = true) {
            BaseTest bt = Tests[index].GetComponent<BaseTest>();
            if (bt != null) {
                bt.RunTest();
            }
            return bt;
        }
    }
}