using System;
using UnityEngine;

namespace DimensionConverter
{
    /// <summary>Conversion2D settings that can be passed in the ToCollider method to define specific conversion behaviour.</summary>
    [Serializable]
    public struct Conversion2DSettings
    {
        [Tooltip("If disabled, PolygonCollider2D to BoxCollider will skip the safety check to make sure the PolygonCollider2D is in an appropriate BoxCollider shape.")] public bool toBoxColliderSkipSafetyCheck;
        [Tooltip("Determines the behaviour for converting PolygonCollider2Ds to MeshColliders.")] public PolygonCollider2DConversionOptions polygonCollider2DConversionOptions;

        public Conversion2DSettings(bool toBoxColliderSkipSafetyCheck, PolygonCollider2DConversionOptions polygonCollider2DConversionOptions)
        {
            this.toBoxColliderSkipSafetyCheck = toBoxColliderSkipSafetyCheck;
            this.polygonCollider2DConversionOptions = polygonCollider2DConversionOptions;
        }

        public override bool Equals(object obj)
        {
            return obj is Conversion2DSettings conversionSettings2D
                && conversionSettings2D.toBoxColliderSkipSafetyCheck == toBoxColliderSkipSafetyCheck
                && conversionSettings2D.polygonCollider2DConversionOptions == polygonCollider2DConversionOptions;
        }

        public override int GetHashCode() => (polygonCollider2DConversionOptions).GetHashCode();

        public static bool operator ==(Conversion2DSettings left, Conversion2DSettings right) => left.Equals(right);
        public static bool operator !=(Conversion2DSettings left, Conversion2DSettings right) => !(left == right);
    }
}

