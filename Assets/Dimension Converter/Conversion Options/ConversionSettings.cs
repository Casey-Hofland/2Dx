using System;
using UnityEngine;

namespace DimensionConverter
{
    /// <summary>Conversion settings that can be passed in the ToCollider2D method to define specific conversion behaviour.</summary>
    [Serializable]
    public struct ConversionSettings
    {
        [Tooltip("The RenderTexture size that is used for converting meshes to polygon colliders. Meshes are converted by rendering an image of the mesh and create a physics shape out of the image. Larger sizes create more accurate colliders while smallers sizes are generated faster and yield more performant results.")] public MeshColliderConversionRenderSize renderSize;
        [Tooltip("Tolerance for optimizing the PolygonCollider2D. 0 means no optimization. 0.03-0.05 is good for most cases.")] [SerializeField] public float tolerance;

        public override bool Equals(object obj)
        {
            return obj is ConversionSettings conversionSettings
                && conversionSettings.renderSize == renderSize
                && conversionSettings.tolerance == tolerance;
        }

        public override int GetHashCode() => (renderSize, tolerance).GetHashCode();

        public static bool operator ==(ConversionSettings left, ConversionSettings right) => left.Equals(right);
        public static bool operator !=(ConversionSettings left, ConversionSettings right) => !(left == right);
    }
}

