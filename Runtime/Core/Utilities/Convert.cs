#nullable enable
using UnityEngine;

namespace Unity2Dx
{
    public static partial class Convert
    {
        /// <include file='./Convert.xml' path='docs/Convert/ToVector2D/*'/>
        public static Vector2 ToVector2D(this Vector3 vector, Component component, Component component2D) => vector.ToVector2D(component.transform.rotation, component2D.transform.rotation);
        /// <include file='./Convert.xml' path='docs/Convert/ToVector2D/*'/>
        public static Vector2 ToVector2D(this Vector3 vector, Quaternion rotation, Quaternion rotation2D)
        {
            return Quaternion.Inverse(rotation2D) * rotation * vector;
        }

        /// <include file='./Convert.xml' path='docs/Convert/ToVector/*'/>
        public static Vector3 ToVector(this Vector2 vector2D, Component component2D, Component component, Vector3 original) => vector2D.ToVector(component2D.transform.rotation, component.transform.rotation, original);
        /// <include file='./Convert.xml' path='docs/Convert/ToVector/*'/>
        public static Vector3 ToVector(this Vector2 vector2D, Quaternion rotation2D, Quaternion rotation, Vector3 original)
        {
            var relativeVector3 = rotation2D * vector2D;
            relativeVector3.z = (rotation * original).z;
            return Quaternion.Inverse(rotation) * relativeVector3;
        }
    }
}
