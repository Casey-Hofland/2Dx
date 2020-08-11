using System.Collections.Generic;
using UnityEngine;

namespace Physics2DxSystem.Utilities
{
    public static class DouglasPeuckerReduction
    {
		// c# implementation of the Ramer-Douglas-Peucker-Algorithm by Craig Selbert slightly adapted for Unity Vector Types
		//http://www.codeproject.com/Articles/18936/A-Csharp-Implementation-of-Douglas-Peucker-Line-Ap

		public static Vector2[] Reduce(List<Vector2> points, float tolerance)
		{
			if(points == null || points.Count < 3 || tolerance <= 0)
				return points.ToArray();

			var firstPoint = 0;
			var lastPoint = points.Count - 1;

			//Add the first and last index to the point indexes.
			var pointIndexes = new List<int>
            {
                firstPoint,
                lastPoint
            };

            //The first and the last point cannot be the same
            while(points[firstPoint] == points[lastPoint])
			{
				lastPoint--;
			}

			Recursive(points, firstPoint, lastPoint, tolerance, pointIndexes);
			pointIndexes.Sort();

			var returnPoints = new Vector2[pointIndexes.Count];
			for(int i = 0; i < returnPoints.Length; i++)
            {
				returnPoints[i] = points[pointIndexes[i]];
            }

			return returnPoints;
		}

		private static void Recursive(IList<Vector2> points, int firstPoint, int lastPoint, float tolerance, List<int> pointIndexes)
		{
			var maxDistance = 0f;
			var farthestIndex = 0;

			for(var index = firstPoint; index < lastPoint; index++)
			{
				var distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);
				if(distance > maxDistance)
				{
					maxDistance = distance;
					farthestIndex = index;
				}
			}

			if(maxDistance > tolerance && farthestIndex != 0)
			{
				//Add the largest point that exceeds the tolerance
				pointIndexes.Add(farthestIndex);

				Recursive(points, firstPoint, farthestIndex, tolerance, pointIndexes);
				Recursive(points, farthestIndex, lastPoint, tolerance, pointIndexes);
			}
		}

		public static float PerpendicularDistance(Vector2 point1, Vector2 point2, Vector2 point)
		{
			float area = Mathf.Abs(.5f * (point1.x * point2.y + point2.x *
				point.y + point.x * point1.y - point2.x * point1.y - point.x *
				point2.y - point1.x * point.y));
			float bottom = Mathf.Sqrt(Mathf.Pow(point1.x - point2.x, 2f) +
				Mathf.Pow(point1.y - point2.y, 2f));
			float height = area / bottom * 2f;

			return height;
		}
	}
}
