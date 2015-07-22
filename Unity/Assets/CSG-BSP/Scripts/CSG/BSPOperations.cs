using UnityEngine;
using System.Collections;

namespace CSG
{
	public class BSPOperations
	{
		public static BSPTree Subtract(BSPTree a, BSPTree b)
		{
			BSPTree aClone = a.Clone();
			BSPTree bClone = b.Clone();

			aClone.Invert();
			aClone.ClipByTree(bClone);
			bClone.ClipByTree(aClone);
			bClone.Invert();
			bClone.ClipByTree(aClone);
			bClone.Invert();
			aClone.AddTriangles(bClone.GetAllTriangles());
			aClone.Invert();

			return aClone;
		}

		/*
	subtract: function(csg) {
			var a = new CSG.Node(this.clone().polygons);
			var b = new CSG.Node(csg.clone().polygons);
			a.invert();
			a.clipTo(b);
			b.clipTo(a);
			b.invert();
			b.clipTo(a);
			b.invert();
			a.build(b.allPolygons());
			a.invert();
			return CSG.fromPolygons(a.allPolygons());
		},*/

	}
}
