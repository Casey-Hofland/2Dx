using System;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionConverter.Tests
{
    /// <summary>
    /// Polygon collider optimizer. Removes points from the collider polygon with 
    /// the given reduction Tolerance
    /// </summary>
    [RequireComponent(typeof(PolygonCollider2D))]
	public class PolygonColliderOptimizer : MonoBehaviour 
	{
		#region Components
		private PolygonCollider2D _polygonCollider2D;
		public PolygonCollider2D polygonCollider2D => _polygonCollider2D ? _polygonCollider2D : (_polygonCollider2D = GetComponent<PolygonCollider2D>());
		#endregion

		[SerializeField] [Range(0f, 1f)] private float _tolerance = default;

		public float tolerance
        {
			get => _tolerance;
			set => _tolerance = Mathf.Clamp01(value);
        }

		[SerializeField] [HideInInspector] private List<ListWrapper> originalPaths = new List<ListWrapper>();
		private List<Vector2> reducedPath = new List<Vector2>();

		[Serializable]
		private struct ListWrapper
        {
			public List<Vector2> path;
			
			public ListWrapper(List<Vector2> path)
            {
				this.path = path;
            }
        }
		
		private void OnValidate()
		{
			if(!_polygonCollider2D)
            {
				//When first getting a reference to the collider save the paths so that the optimization is redoable (by performing it on the original path every time).
				for(int i = 0; i < polygonCollider2D.pathCount; i++)
				{
					originalPaths.Add(new ListWrapper(new List<Vector2>()));
					polygonCollider2D.GetPath(i, originalPaths[i].path);
				}
			}

			for(int i = 0; i < originalPaths.Count; i++)
            {
				var originalPath = originalPaths[i].path;
				LineUtility.Simplify(originalPath, tolerance, reducedPath);
				polygonCollider2D.SetPath(i, reducedPath);
            }
		}
	}
}
