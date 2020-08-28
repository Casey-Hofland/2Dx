using DimensionConverter.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionConverter.Tests
{
    /// <summary>
    /// Edge collider optimizer. Creates an edge collider by casting rays from the upper edge of the bounding box
    /// into the edgeNormalOpposite direction (by default downwards). The resulting points are then reduced with 
    /// the given tolerance
    /// </summary>
    [RequireComponent(typeof(EdgeCollider2D))]
	[Obsolete("This class isn't functioning properly.", false)]
	public class EdgeColliderOptimizer : MonoBehaviour 
	{
		#region Required Components
		private EdgeCollider2D _edgeCollider2D;
		public EdgeCollider2D edgeCollider2D => _edgeCollider2D ? _edgeCollider2D : (_edgeCollider2D = GetComponent<EdgeCollider2D>());
		#endregion

		public Vector2 edgeNormalOpposite = Vector2.down;
		public int rayBudget = 1000;
		[SerializeField] [Range(0f, 1f)] private float _tolerance = default;

		public float tolerance
        {
			get => _tolerance;
			set => _tolerance = Mathf.Clamp01(value);
        }

        private void OnValidate()
        {
			var path = new List<Vector2>();
			var upperRight = edgeCollider2D.bounds.max;
			var upperLeft = edgeCollider2D.bounds.min;
			upperLeft.y = upperRight.y;

			for(float i = 0; i < rayBudget; i++)
            {
				var t = i / rayBudget;

				//interpolate along the upper edge of the collider bounds
				var rayOrigin = Vector2.Lerp(upperLeft, upperRight, t);
				var hits = Physics2D.RaycastAll(rayOrigin, edgeNormalOpposite, edgeCollider2D.bounds.size.y);

				foreach(var hit in hits)
                {
					if(hit.collider == edgeCollider2D)
                    {
						var localHitPoint = transform.InverseTransformPoint(hit.point);
						path.Add(localHitPoint);
						break;
					}
                }
			}

			var reducedPath = DouglasPeuckerReduction.Reduce(path, tolerance);
			edgeCollider2D.points = reducedPath;
		}
    }
}
