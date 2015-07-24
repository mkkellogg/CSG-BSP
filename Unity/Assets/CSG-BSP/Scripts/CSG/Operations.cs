/**
 * 
 * class: Operations
 * 
 * Author: Mark Kellogg
 * 
 * This class contains the implementation of standard
 * boolean and arithmetic operations on geometry that is
 * represented by BSPTree instances.
 * 
 */

using UnityEngine;
using System.Collections;

namespace CSG
{
	public class Operations
	{
		/**
		 * Returns geometry corresponding to volume that is occupied by @a,
		 * but not by @b.
		 */
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
