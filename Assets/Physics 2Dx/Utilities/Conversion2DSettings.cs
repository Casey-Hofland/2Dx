using System;
using UnityEngine;

namespace Physics2DxSystem.Utilities
{
    /// <summary>Conversion2D settings that can be passed in the ToCollider method to define specific conversion behaviour.</summary>
    [Serializable]
    public struct Conversion2DSettings
    {
        [Tooltip("Determines the behaviour for converting PolygonCollider2Ds to MeshColliders.")] public PolygonCollider2DConversionMethod polygonCollider2DConversionMethod;

        public override bool Equals(object obj)
        {
            return obj is Conversion2DSettings conversionSettings2D
                && conversionSettings2D.polygonCollider2DConversionMethod == polygonCollider2DConversionMethod;
        }

        public override int GetHashCode() => (polygonCollider2DConversionMethod).GetHashCode();

        public static bool operator ==(Conversion2DSettings left, Conversion2DSettings right) => left.Equals(right);
        public static bool operator !=(Conversion2DSettings left, Conversion2DSettings right) => !(left == right);
    }
}

