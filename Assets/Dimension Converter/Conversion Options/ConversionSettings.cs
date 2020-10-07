using System;
using UnityEngine;

namespace DimensionConverter
{
    /// <summary>Conversion settings that can be passed in the ToCollider2D method to define specific conversion behaviour.</summary>
    [Serializable]
    public struct ConversionSettings
    {
        public static readonly ConversionSettings Default = new ConversionSettings(256, 3, 0.99f, 0.03f);

        public int resolution;
        public uint lineTolerance;
        public float outlineTolerance;
        [Tooltip("Tolerance for optimizing the PolygonCollider2D. 0 means no optimization. 0.03-0.05 is good for most cases.")] [SerializeField] public float simplifyTolerance;

        public ConversionSettings(int resolution, uint lineTolerance, float outlineTolerance, float simplifyTolerance)
        {
            this.resolution = resolution;
            this.lineTolerance = lineTolerance;
            this.outlineTolerance = outlineTolerance;
            this.simplifyTolerance = simplifyTolerance;
        }

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

