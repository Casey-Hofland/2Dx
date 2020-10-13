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
	public class EdgeColliderOptimizer : MonoBehaviour 
	{
		#region Components
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

		[SerializeField] [HideInInspector] private List<Vector2> originalPath = new List<Vector2>();
		private List<Vector2> reducedPath = new List<Vector2>();

        private void OnValidate()
        {
			if(!_edgeCollider2D)
			{
				//When first getting a reference to the collider save the paths so that the optimization is redoable (by performing it on the original path every time).
				originalPath = new List<Vector2>(edgeCollider2D.points);
			}
			else
            {
				edgeCollider2D.points = originalPath.ToArray();
            }

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

			LineUtility.Simplify(path, tolerance, reducedPath);
			edgeCollider2D.points = reducedPath.ToArray();
		}
    }
}
