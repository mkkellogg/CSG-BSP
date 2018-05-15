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
using System.Collections.Generic;

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

			float startTime = Time.realtimeSinceStartup;
			bClone.Invert ();
			bClone.ClipByTree (a, false);
			aClone.ClipByTree (b);
			aClone.AddTriangles(bClone.GetAllTriangles());

			return aClone;
		}

		/**
		 * Returns geometry corresponding to volume @a that is divided into
		 * two separate pieces by @b. Return results in the form of two BSPTree
		 * instances: @side and @side2.
		 */
		public static void Slice(BSPTree a, BSPTree b, out BSPTree side1, out BSPTree side2)
		{
			side1 = a.Clone();
			BSPTree bClone = b.Clone();

			List<Triangle> side1Discarded = new List<Triangle>();

			bClone.Invert ();
			bClone.ClipByTree (a, false);
			side1.ClipByTree (b, true, side1Discarded);
			side1.AddTriangles(bClone.GetAllTriangles());

			bClone.Invert();
			side2 = new BSPTree();
			side2.AddTriangles(bClone.GetAllTriangles());
			side2.AddTriangles(side1Discarded);
		}

		/**
		 * Returns geometry corresponding to volume @a that is divided into
		 * two separate pieces by @b. Return results in the form of two List<Triangle>
		 * instances: @side and @side2.
		 */
		public static void Slice(BSPTree a, BSPTree b, out List<Triangle> side1, out List<Triangle> side2)
		{
			BSPTree aClone = a.Clone();
			BSPTree bClone = b.Clone();
			
			List<Triangle> aDiscarded = new List<Triangle>();
			
			bClone.Invert ();
			bClone.ClipByTree (a, false);
			aClone.ClipByTree (b, true, aDiscarded);
			aClone.AddTriangles(bClone.GetAllTriangles());
			side1 = aClone.GetAllTriangles();
			
			bClone.Invert();
			side2 = new List<Triangle>();
			side2.AddRange(bClone.GetAllTriangles());
			side2.AddRange(aDiscarded);
		}

		/**
		 * Returns geometry corresponding to volume @a that is divided into
		 * two separate pieces by @b, but @b is treated as a plane (the first split plane
		 * from the first node is used for splitting).
		 * 
		 * Return results in the form of two List<Triangle> instances: @side and @side2.
		 */
		public static void FastSlice(BSPTree a, BSPTree b, out List<Triangle> side1, out List<Triangle> side2)
		{
			side1 = null;
			side2 = null;

			BSPTree bClone = b.Clone();
			float startTime = Time.realtimeSinceStartup;
			bClone.ClipByTree (a, false);
			float clipTime = Time.realtimeSinceStartup - startTime;

			List<Triangle> aTriangles = a.GetAllTriangles();
			List<Triangle> bTriangles = bClone.GetAllTriangles();
			bClone.Invert();
			List<Triangle> bInvertedTriangles = bClone.GetAllTriangles();

			if(bTriangles.Count > 0)
			{
				Plane splitPlane = bTriangles[0].OrientationPlane;
			
				side1 = new List<Triangle>();
				side2 = new List<Triangle>();
				var coplanar = new List<Triangle>();
				for(int i =0; i < aTriangles.Count; i++)
				{
					Triangle tri = aTriangles[i];
					Partitioner.SliceTriangle(tri, splitPlane, side1, side2, coplanar, coplanar);
				}
				side1.AddRange(bTriangles);
				side2.AddRange(bInvertedTriangles);
			}
			else
			{
				side1 = aTriangles;
				side2 = null;
			}			
		}
	}
}
