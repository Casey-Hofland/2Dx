using System;
using UnityEngine;

namespace Physics2DxSystem
{
    /// <summary>Conversion2D settings that can be passed in the ToCollider method to define specific conversion behaviour.</summary>
    [Serializable]
    public struct Conversion2DSettings
    {
        [Tooltip("If enabled, PolygonCollider2D to BoxCollider will first go through a safety check to make sure the PolygonCollider2D is in an appropriate BoxCollider shape.")] public bool toBoxColliderSafe;
        [Tooltip("Determines the behaviour for converting PolygonCollider2Ds to MeshColliders.")] public PolygonCollider2DConversionOptions polygonCollider2DConversionOptions;

        public override bool Equals(object obj)
        {
            return obj is Conversion2DSettings conversionSettings2D
                && conversionSettings2D.toBoxColliderSafe == toBoxColliderSafe
                && conversionSettings2D.polygonCollider2DConversionOptions == polygonCollider2DConversionOptions;
        }

        public override int GetHashCode() => (polygonCollider2DConversionOptions).GetHashCode();

        public static bool operator ==(Conversion2DSettings left, Conversion2DSettings right) => left.Equals(right);
        public static bool operator !=(Conversion2DSettings left, Conversion2DSettings right) => !(left == right);
    }
}

