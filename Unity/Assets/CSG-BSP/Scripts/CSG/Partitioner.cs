/**
 * 
 * class: Partitioner
 * 
 * Author: Mark Kellogg
 * 
 * Paritioner is a utility class for subdividing & partitioning 3D geometry.
 * 
 * Currently it only contains methods for categorizing & splitting triangles
 * based on planes.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG
{
	public class Partitioner 
	{
		public enum Orientation
		{
			CoPlanar = 0,
			LessThan = 1,
			GreaterThan = 2,
			Split = 4
		}
		
		private const float SplitEpsilon = 0.00001f;
		
		/**
		 * Categorize or slice @triangle using @plane. Triangles existing completely on the front side of @plane
		 * are placed in @greaterThan, triangles exisitng completely on the back side of @plane are placed in
		 * @lessThan. Co-planar triangles go in @greaterThanPlanar if their normals are the same direction as 
		 * @plane's normal, otherwise they go in @lessThanPlanar.
		 * 
		 * If a triangle spans @plane and has vertices on both sides, it is split into multiple triangles 
		 * along @plane. The resulting triangles are then categorized as specified above (in this case they are either
		 * in front or back, they could not be co-planar).
		 * 
		 * Determining the orientation of a given vertex involves taking the dot product of the vertex's position with
		 * @plane's normal and comparing that to @plane's offset from the origin (plane.D). If the dot-product is greater
		 * than that offset (plus some small epsilon), it is in front; if it is less than that offset (minus a small epsilon)
		 * it is in back. Otherwise it is co-planar.
		 *
		 */
		public static Orientation SliceTriangle(Triangle triangle, Plane plane, 
		                                        FastLinkedList<Triangle> lessThan, FastLinkedList<Triangle> greaterThan, 
		                                        FastLinkedList<Triangle> lessThanPlanar, FastLinkedList<Triangle> greaterThanPlanar)
		{
			Orientation[] vertOrientations = new Orientation[3];
			Orientation triOrientation = Orientation.CoPlanar;
			int orientationsFound = 0;
			
			// loop through each vertex and categorize each based
			// on its position relative to @plane.
			for(int i =0; i < 3; i++)
			{
				Orientation currentOrientation =  ClassifyVertexOrientation(triangle.GetVertexByIndex(i), plane);
				vertOrientations[i] = currentOrientation;
				orientationsFound |= (int)currentOrientation;
			}
			
			// classify @triangle's orientation relative to @plane based
			// on the orientations of all vertices.
			if (orientationsFound > (int)Orientation.GreaterThan)
				triOrientation = Orientation.Split;
			else
				triOrientation = (Orientation)orientationsFound;
			
			// place @triangle in the appropriate list based on its orientation, or
			// split it if necessary
			switch(triOrientation)
			{
			case Orientation.CoPlanar:
				// if @triangle's normal matches @plane's normal, i.e. dot product will be 1,
				// then we consider @triangle to be front-facing
				float planeTriOrientation = triangle.OrientationPlane.Normal.Dot(plane.Normal);
				if(planeTriOrientation > 0)
					greaterThanPlanar.AddLast(triangle);
				else
					lessThanPlanar.AddLast(triangle);
				break;
			case Orientation.LessThan:
				lessThan.AddLast(triangle);
				break;
			case Orientation.GreaterThan:
				greaterThan.AddLast(triangle);
				break;
			case Orientation.Split:
				List<Vertex> ltSplit = new List<Vertex>();
				List<Vertex> gtSplit = new List<Vertex>();
				
				// loop through each edge in @triangle. 
				// @currentVertex = edge's first vertex.
				// @nextVertex = edge's second vertex.
				// @currentOrientation = orientation of the edge's first vertex.
				// @nextOrientation = orientation of the edge's second vertex.
				for(int i=0; i < 3; i++)
				{
					int currentIndex = i;
					int nextIndex = currentIndex < 2 ? currentIndex + 1 : 0;
					
					Orientation currentOrientation = vertOrientations[currentIndex];
					Orientation nextOrientation = vertOrientations[nextIndex];
					
					Vertex currentVertex = triangle.GetVertexByIndex(currentIndex);
					Vertex nextVertex = triangle.GetVertexByIndex(nextIndex); 
					
					Vector3D currentEdge = nextVertex.Position.SubtractedBy(currentVertex.Position);
					
					// vertices are traversed in clock-wise order, which means @triangle's edges
					// are also traversed in clock-wise order. this means that as vertices are added to
					// @gtSplit and @ltSplit, their respective vertices will also be in clock-wise order.
					switch(currentOrientation)
					{
					case Orientation.CoPlanar:
						gtSplit.Add(currentVertex);
						ltSplit.Add(currentVertex);
						break;
					case Orientation.LessThan:
						ltSplit.Add(currentVertex);
						break;
					case Orientation.GreaterThan:
						gtSplit.Add(currentVertex);
						break;
					}
					
					// if we move from a "GreaterThan" orientation to a "LessThan" orientation or vice-versa,
					// then we have crossed @plane. this means we need to interpolate between the edge's vertices
					// to create a new vertex that is co-planar with @plane.
					if(currentOrientation == Orientation.GreaterThan && nextOrientation == Orientation.LessThan ||
					   currentOrientation == Orientation.LessThan && nextOrientation == Orientation.GreaterThan )
					{
						float splitPortion = plane.D - plane.Normal.Dot(currentVertex.Position);
						float fullPortion = plane.Normal.Dot(currentEdge);
						float splitFraction = splitPortion / fullPortion;
						Vertex splitVertex = currentVertex.Lerped(nextVertex, splitFraction);
						
						gtSplit.Add(splitVertex);
						ltSplit.Add(splitVertex);
					}  		
				}
				
				// create 1 new triangle if @ltSplit contains 3 vertices. create 2 triangles if it
				// contains 4
				if (ltSplit.Count >= 3) 
				{
					lessThan.AddLast(new Triangle(ltSplit[0], ltSplit[1], ltSplit[2]));
					if(ltSplit.Count == 4)
						lessThan.AddLast(new Triangle(ltSplit[0], ltSplit[2], ltSplit[3]));
				}
				
				// create 1 new triangle if @gtSplit contains 3 vertices. create 2 triangles if it
				// contains 4
				if (gtSplit.Count >= 3) 
				{
					greaterThan.AddLast(new Triangle(gtSplit[0], gtSplit[1], gtSplit[2]));
					if(gtSplit.Count == 4)
						greaterThan.AddLast(new Triangle(gtSplit[0], gtSplit[2], gtSplit[3]));
				}
				break;
			}
			
			return triOrientation;
		}

		/**
		 * Classify @vertex based on which side of @plane it lies.
		 */
		private static Orientation ClassifyVertexOrientation(Vertex vertex, Plane plane)
		{
			float diff = plane.Normal.Dot(vertex.Position) - plane.D;
			if(diff < -SplitEpsilon)
				return Orientation.LessThan;
			else if(diff > SplitEpsilon)
				return Orientation.GreaterThan;
			else
				return Orientation.CoPlanar;
		}
	}
}
