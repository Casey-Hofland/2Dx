using UnityEngine;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Mesh Collider 2Dx")]
    [DisallowMultipleComponent]
    public sealed class MeshCollider2Dx : ColliderConverter<MeshCollider, PolygonCollider2D>
    {
        #region Properties
        [Header("Convert to 2D")]
        [Tooltip("The outliner holding the settings for tracing a MeshCollider outline.")] [SerializeField] private Outliner _outliner;

        [Header("Convert to 3D")]
        [Tooltip("Determines the behaviour for converting PolygonCollider2Ds to MeshColliders.")] [SerializeField] private PolygonCollider2DConversionOptions _conversionOptions;

        public Outliner outliner
        {
            get => _outliner;
            set => _outliner = value;
        }

        public PolygonCollider2DConversionOptions conversionOptions
        {
            get => _conversionOptions;
            set => _conversionOptions = value;
        }
        #endregion

        protected override void ColliderToCollider(MeshCollider collider, MeshCollider other)
        {
            collider.ToMeshCollider(other);
        }

        protected override void Collider2DToCollider2D(PolygonCollider2D collider2D, PolygonCollider2D other)
        {
            collider2D.ToPolygonCollider2D(other);
        }

        protected override void ColliderToCollider2D(MeshCollider collider, PolygonCollider2D collider2D)
        {
            collider.ToPolygonCollider2D(collider2D, outliner);
        }

        protected override void Collider2DToCollider(PolygonCollider2D collider2D, MeshCollider collider)
        {
            collider2D.ToMeshCollider(collider, conversionOptions);
        }
    }
}
