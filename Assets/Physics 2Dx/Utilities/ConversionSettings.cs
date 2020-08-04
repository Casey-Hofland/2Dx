using System;
using UnityEngine;

namespace Physics2DxSystem.Utilities
{
    /// <summary>Conversion settings that can be passed in the ToCollider2D method to define specific conversion behaviour.</summary>
    [Serializable]
    public struct ConversionSettings
    {
        [Tooltip("The RenderTexture size that is used for converting meshes to polygon colliders. Meshes are converted by rendering an image of the mesh and create a physics shape out of the image. Larger sizes create more accurate colliders while smallers sizes are generated faster and yield more performant results. Note that if a size is too large the generated collider might have overlapping points and not function properly.")] public MeshColliderConversionRenderSize renderSize;

        public override bool Equals(object obj)
        {
            return obj is ConversionSettings conversionSettings
                && conversionSettings.renderSize == renderSize;
        }

        public override int GetHashCode() => (renderSize).GetHashCode();

        public static bool operator ==(ConversionSettings left, ConversionSettings right) => left.Equals(right);
        public static bool operator !=(ConversionSettings left, ConversionSettings right) => !(left == right);
    }
}

