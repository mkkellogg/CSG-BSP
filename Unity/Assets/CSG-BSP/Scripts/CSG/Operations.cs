using UnityEngine;
using System.Collections;

namespace CSG
{
	public class Operations
	{
		public static BSPTree Subtract(BSPTree a, BSPTree b)
		{
			BSPTree aClone = a.Clone();
			BSPTree bClone = b.Clone();

			bClone.Invert ();
			bClone.ClipByTree (a, false);
			aClone.ClipByTree (b);
			aClone.AddTriangles(bClone.GetAllTriangles());

			return aClone;
		}
	}
}
