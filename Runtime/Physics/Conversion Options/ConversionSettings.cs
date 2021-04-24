using System;
using UnityEngine;

namespace Unity2Dx.Physics
{
    /// <summary>Conversion settings that can be passed in the ToCollider2D method to define specific conversion behaviour.</summary>
    [Serializable]
    public struct ConversionSettings
    {
        [Tooltip("The outliner holding the settings for tracing a MeshCollider outline.")] public Outliner outliner;

        public ConversionSettings(Outliner outliner)
        {
            this.outliner = outliner;
        }

        public override bool Equals(object obj)
        {
            return obj is ConversionSettings conversionSettings
                && conversionSettings.outliner == outliner;
        }

        public override int GetHashCode() => (outliner).GetHashCode();

        public static bool operator ==(ConversionSettings left, ConversionSettings right) => left.Equals(right);
        public static bool operator !=(ConversionSettings left, ConversionSettings right) => !(left == right);
    }
}

