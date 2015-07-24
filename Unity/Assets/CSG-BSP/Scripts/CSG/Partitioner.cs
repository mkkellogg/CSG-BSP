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

		public static Orientation SliceTriangle(Triangle triangle, Plane plane, 
		                                        FastLinkedList<Triangle> lessThan, FastLinkedList<Triangle> greaterThan, 
		                                        FastLinkedList<Triangle> lessThanPlanar, FastLinkedList<Triangle> greaterThanPlanar)
		{
			Orientation[] vertOrientations = new Orientation[3];
			Orientation triOrientation = Orientation.CoPlanar;
			int orientationsFound = 0;

			for(int i =0; i < 3; i++)
			{
				Orientation currentOrientation =  ClassifyVertexOrientation(triangle.GetVertexByIndex(i), plane);
				vertOrientations[i] = currentOrientation;
				orientationsFound |= (int)currentOrientation;
			}

			if (orientationsFound > (int)Orientation.GreaterThan)
				triOrientation = Orientation.Split;
			else
				triOrientation = (Orientation)orientationsFound;

			switch(triOrientation)
			{
				case Orientation.CoPlanar:
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
					List<Vertex> ltSpanning = new List<Vertex>();
					List<Vertex> gtSpanning = new List<Vertex>();

					for(int i=0; i < 3; i++)
					{
						int currentIndex = i;
						int nextIndex = currentIndex < 2 ? currentIndex + 1 : 0;

						Orientation currentOrientation = vertOrientations[currentIndex];
						Orientation nextOrientation = vertOrientations[nextIndex];

						Vertex currentVertex = triangle.GetVertexByIndex(currentIndex);
						Vertex nextVertex = triangle.GetVertexByIndex(nextIndex); 

						Vector3D currentEdge = nextVertex.Position.SubtractedBy(currentVertex.Position);

						switch(currentOrientation)
						{
							case Orientation.CoPlanar:
								gtSpanning.Add(currentVertex);
								ltSpanning.Add(currentVertex);
							break;
							case Orientation.LessThan:
								ltSpanning.Add(currentVertex);
							break;
							case Orientation.GreaterThan:
								gtSpanning.Add(currentVertex);
							break;
						}

						if(currentOrientation == Orientation.GreaterThan && nextOrientation == Orientation.LessThan ||
					       currentOrientation == Orientation.LessThan && nextOrientation == Orientation.GreaterThan )
						{
							float splitPortion = plane.D - plane.Normal.Dot(currentVertex.Position);
							float fullPortion = plane.Normal.Dot(currentEdge);
							float splitFraction = splitPortion / fullPortion;
							Vertex splitVertex = currentVertex.Lerped(nextVertex, splitFraction);
							
							gtSpanning.Add(splitVertex);
							ltSpanning.Add(splitVertex);
						}  		
					}

					if (ltSpanning.Count >= 3) 
					{
						lessThan.AddLast(new Triangle(ltSpanning[0], ltSpanning[1], ltSpanning[2]));
						if(ltSpanning.Count >= 4)
							lessThan.AddLast(new Triangle(ltSpanning[0], ltSpanning[2], ltSpanning[3]));
					}

					if (gtSpanning.Count >= 3) 
					{
						greaterThan.AddLast(new Triangle(gtSpanning[0], gtSpanning[1], gtSpanning[2]));
						if(gtSpanning.Count >= 4)
							greaterThan.AddLast(new Triangle(gtSpanning[0], gtSpanning[2], gtSpanning[3]));
					}
				break;
			}

			return triOrientation;
		}

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
