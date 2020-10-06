using System;
using UnityEngine;

namespace DimensionConverter
{
    /// <summary>Conversion settings that can be passed in the ToCollider2D method to define specific conversion behaviour.</summary>
    [Serializable]
    public struct ConversionSettings
    {
        //[Tooltip("The RenderTexture size that is used for converting meshes to polygon colliders. Meshes are converted by rendering an image of the mesh and create a physics shape out of the image. Larger sizes create more accurate colliders while smallers sizes are generated faster and yield more performant results.")] public MeshColliderConversionRenderSize renderSize;
        public int resolution;
        public uint lineTolerance;
        public float outlineTolerance;
        [Tooltip("Tolerance for optimizing the PolygonCollider2D. 0 means no optimization. 0.03-0.05 is good for most cases.")] [SerializeField] public float simplifyTolerance;

        public override bool Equals(object obj)
        {
            return obj is ConversionSettings conversionSettings
                && conversionSettings.resolution == resolution
                && conversionSettings.lineTolerance == lineTolerance
                && conversionSettings.outlineTolerance == outlineTolerance
                && conversionSettings.simplifyTolerance == simplifyTolerance;
        }

        public override int GetHashCode() => (resolution, lineTolerance, outlineTolerance, simplifyTolerance).GetHashCode();

        public static bool operator ==(ConversionSettings left, ConversionSettings right) => left.Equals(right);
        public static bool operator !=(ConversionSettings left, ConversionSettings right) => !(left == right);
    }
}

