using DimensionConverter.Utilities;
using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Mesh Converter")]
    [DisallowMultipleComponent]
    public sealed class MeshConverter : ColliderConverter<MeshCollider, PolygonCollider2D>
    {
        [Header("Convert to 2D")]
        public int resolution = ConversionSettings.Default.resolution;
        public uint lineTolerance = ConversionSettings.Default.lineTolerance;
        [Range(0f, 1f)] public float outlineTolerance = ConversionSettings.Default.outlineTolerance;
        [Tooltip("Tolerance for optimizing the PolygonCollider2D. 0 means no optimization. 0.03-0.05 is good for most cases.")] public float simplifyTolerance = ConversionSettings.Default.simplifyTolerance;
        [Header("Convert to 3D")]
        [Tooltip("Determines the behaviour for converting PolygonCollider2Ds to MeshColliders.")] public PolygonCollider2DConversionOptions conversionOptions = Conversion2DSettings.Default.polygonCollider2DConversionOptions;

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
            collider.ToPolygonCollider2D(collider2D, resolution, lineTolerance, outlineTolerance, simplifyTolerance);
        }

        protected override void Collider2DToCollider(PolygonCollider2D collider2D, MeshCollider collider)
        {
            collider2D.ToMeshCollider(collider, conversionOptions);
        }
    }
}
