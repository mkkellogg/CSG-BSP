using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CSG
{
	public class Partitioner 
	{
		public class PartitionException : System.Exception
		{
			public PartitionException(string msg) : base(msg)
			{

			}
		}

		private const float SplitEpsilon = 0.00001f;

		private enum VertexOrientation
		{
			CoPlanar = 0,
			GreaterThan = 1,
			LessThan = 2
		}

		private enum PolygonOrientation
			{
			CoPlanar = 0,
			LessThan = 1,
			GreaterThan = 2,
			Spanning = 4
		}

		public static void SlicePolygon(Polygon polygon, Plane plane, 
		                                List<Polygon> lessThan, List<Polygon> greaterThan, 
		                                List<Polygon> lessThanPlanar, List<Polygon> greaterThanPlanar)
		{
			if(polygon.VertexCount() < 3)throw new PartitionException("All polygons must have at least 3 vertices!");

			VertexOrientation[] vertOrientations = new VertexOrientation[polygon.VertexCount()];
			PolygonOrientation polyOrientation = PolygonOrientation.CoPlanar;
			int orientationsFound = 0;

			for(int i=0; i < polygon.VertexCount(); i++)
			{
				VertexOrientation currentOrientation =  ClassifyVertexOrientation(polygon.GetVertex(i), plane);
				vertOrientations[i] = currentOrientation;
				orientationsFound |= (int)currentOrientation;
			}

			if (orientationsFound > (int)PolygonOrientation.GreaterThan)
				polyOrientation = PolygonOrientation.Spanning;
			else
				polyOrientation = (PolygonOrientation)orientationsFound;

			switch(polyOrientation)
			{
				case PolygonOrientation.CoPlanar:
					float planePolyOrientation = polygon.Plane.Normal.Dot(plane.Normal);
					if(planePolyOrientation > 0)greaterThanPlanar.Add(polygon);
					else lessThanPlanar.Add(polygon);
				break;
				case PolygonOrientation.LessThan:
					lessThan.Add(polygon);
				break;
				case PolygonOrientation.GreaterThan:
					greaterThan.Add(polygon);
				break;
				case PolygonOrientation.Spanning:
					List<Vertex> ltSpanning = new List<Vertex>();
					List<Vertex> gtSpanning = new List<Vertex>();

					for(int i=0; i < polygon.VertexCount(); i++)
					{
						int currentIndex = i;
						int nextIndex = currentIndex < polygon.VertexCount() - 1 ? currentIndex + 1 : 0;

						VertexOrientation currentOrientation = vertOrientations[currentIndex];
						VertexOrientation nextOrientation = vertOrientations[nextIndex];

						Vertex currentVertex = polygon.GetVertex(currentIndex);
						Vertex nextVertex = polygon.GetVertex(nextIndex);

						Vector3D currentEdge = nextVertex.Position.SubtractedBy(currentVertex.Position);

						if(currentOrientation != VertexOrientation.LessThan)gtSpanning.Add(currentVertex);
						if(currentOrientation != VertexOrientation.GreaterThan)ltSpanning.Add(currentVertex);

						if(currentOrientation != nextOrientation &&
						   currentOrientation != VertexOrientation.CoPlanar && 
					       nextOrientation != VertexOrientation.CoPlanar)
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
						Plane polyPlane = Plane.BuildFromVertices(ltSpanning[0].Position, ltSpanning[1].Position, ltSpanning[2].Position);
						lessThan.Add(new Polygon(ltSpanning, polyPlane));
					}

					if (gtSpanning.Count >= 3) 
					{
						Plane polyPlane = Plane.BuildFromVertices(gtSpanning[0].Position, gtSpanning[1].Position, gtSpanning[2].Position);
						greaterThan.Add(new Polygon(gtSpanning, polyPlane));
					}
				break;
			}
		}

		private static VertexOrientation ClassifyVertexOrientation(Vertex vertex, Plane plane)
		{
			float diff = plane.Normal.Dot(vertex.Position) - plane.D;
			if(diff < -SplitEpsilon)
				return VertexOrientation.LessThan;
			else if(diff > SplitEpsilon)
				return VertexOrientation.GreaterThan;
			else
				return VertexOrientation.CoPlanar;
		}
	}
}
