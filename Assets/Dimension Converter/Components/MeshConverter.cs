using DimensionConverter.Utilities;
using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Mesh Converter")]
    [DisallowMultipleComponent]
    public sealed class MeshConverter : ColliderConverter<MeshCollider, PolygonCollider2D>
    {
        [Header("Convert to 2D")]
        [Tooltip("The outliner holding the settings for tracing a MeshCollider outline.")] public Outliner outliner;

        [Header("Convert to 3D")]
        [Tooltip("Determines the behaviour for converting PolygonCollider2Ds to MeshColliders.")] public PolygonCollider2DConversionOptions conversionOptions;

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
