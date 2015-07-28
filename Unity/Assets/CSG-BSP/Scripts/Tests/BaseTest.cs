using UnityEngine;
using System.Collections;

namespace CSG
{
	public abstract class BaseTest : MonoBehaviour 
	{
		public Transform CameraPosition;
		public string TestName;

		public abstract void RunTest();
	}
}
