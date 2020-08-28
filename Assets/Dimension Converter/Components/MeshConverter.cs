using DimensionConverter.Utilities;
using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Mesh Converter")]
    [DisallowMultipleComponent]
    public sealed class MeshConverter : ColliderConverter<MeshCollider, PolygonCollider2D>
    {
        [Header("Convert to 2D")]
        [Tooltip("The RenderTexture size that is used for converting meshes to polygon colliders. Meshes are converted by rendering an image of the mesh and creating a physics shape out of the image. Larger sizes create more accurate colliders while smallers sizes generate faster and yield more performant results.")] public MeshColliderConversionRenderSize renderSize = MeshColliderConversionRenderSize._256;
        [Tooltip("Tolerance for optimizing the PolygonCollider2D. 0 means no optimization. 0.03-0.05 is good for most cases.")] public float tolerance = 0.05f;
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
            collider.ToPolygonCollider2D(collider2D, renderSize, tolerance);
        }
        
        protected override void Collider2DToCollider(PolygonCollider2D collider2D, MeshCollider collider)
        {
            collider2D.ToMeshCollider(collider, conversionOptions);
        }
    }
}
