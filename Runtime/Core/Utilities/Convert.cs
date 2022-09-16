#nullable enable
using UnityEngine;

namespace Unity2Dx
{
    public enum RotationSource : byte
    {
        Transform,
        Rigidbody2D,
        ZOnly
    }

    public static partial class Convert
    {
        private static readonly Quaternion zRotation90Deg = new(0f, 0f, 0.7071068f, 0.7071068f);

        #region ToVector2D
        /// <include file='./Convert.xml' path='docs/Convert/ToVector2D/*'/>
        public static Vector2 ToVector2D(this Vector3 vector, Component component, Component component2D) => vector.ToVector2D(component.transform.rotation, component2D.transform.rotation);
        
        /// <include file='./Convert.xml' path='docs/Convert/ToVector2D/*'/>
        public static Vector2 ToVector2D(this Vector3 vector, Component component, Component component2D, RotationSource rotation2DSource) => component2D is Rigidbody2D rigidbody2D 
            ? vector.ToVector2D(component, rigidbody2D, rotation2DSource) 
            : vector.ToVector2D(component.transform.rotation, component2D.transform.rotation, rotation2DSource);

        /// <include file='./Convert.xml' path='docs/Convert/ToVector2D/*'/>
        public static Vector2 ToVector2D(this Vector3 vector, Component component, Rigidbody2D rigidbody2D, RotationSource rotation2DSource) => vector.ToVector2D(component.transform.rotation, rigidbody2D.ToQuaternion2D(rotation2DSource));

        /// <include file='./Convert.xml' path='docs/Convert/ToVector2D/*'/>
        public static Vector2 ToVector2D(this Vector3 vector, Quaternion rotation, Quaternion rotation2D) => Quaternion.Inverse(rotation2D) * rotation * vector;
        /// <include file='./Convert.xml' path='docs/Convert/ToVector2D/*'/>
        public static Vector2 ToVector2D(this Vector3 vector, Quaternion rotation, Quaternion rotation2D, RotationSource rotation2DSource) => vector.ToVector2D(rotation, rotation2D.ToQuaternion2D(rotation2DSource));
        #endregion

        #region ToVector
        /// <include file='./Convert.xml' path='docs/Convert/ToVector/*'/>
        public static Vector3 ToVector(this Vector2 vector2D, Component component2D, Component component, Vector3 original) => vector2D.ToVector(component2D.transform.rotation, component.transform.rotation, original);

        /// <include file='./Convert.xml' path='docs/Convert/ToVector/*'/>
        public static Vector3 ToVector(this Vector2 vector2D, Component component2D, Component component, Vector3 original, RotationSource rotation2DSource) => component2D is Rigidbody2D rigidbody2D
            ? vector2D.ToVector(rigidbody2D, component, original, rotation2DSource)
            : vector2D.ToVector(component2D.transform.rotation, component.transform.rotation, original, rotation2DSource);

        /// <include file='./Convert.xml' path='docs/Convert/ToVector/*'/>
        public static Vector3 ToVector(this Vector2 vector2D, Rigidbody2D rigidbody2D, Component component, Vector3 original, RotationSource rotation2DSource) => vector2D.ToVector(rigidbody2D.ToQuaternion2D(rotation2DSource), component.transform.rotation, original);

        /// <include file='./Convert.xml' path='docs/Convert/ToVector/*'/>
        public static Vector3 ToVector(this Vector2 vector2D, Quaternion rotation2D, Quaternion rotation, Vector3 original)
        {
            var relativeVector = rotation2D * vector2D;
            relativeVector.z = (rotation * original).z;
            return Quaternion.Inverse(rotation) * relativeVector;
        }

        /// <include file='./Convert.xml' path='docs/Convert/ToVector/*'/>
        public static Vector3 ToVector(this Vector2 vector2D, Quaternion rotation2D, Quaternion rotation, Vector3 original, RotationSource rotation2DSource) => vector2D.ToVector(rotation2D.ToQuaternion2D(rotation2DSource), rotation, original);
        #endregion

        public static Quaternion ToQuaternion2D(this Quaternion rotation) => rotation.ToQuaternion2D((CapsuleDirection2D)default);
        public static Quaternion ToQuaternion2D(this Quaternion rotation, CapsuleDirection2D direction2D)
        {
            return direction2D switch
            {
                CapsuleDirection2D.Vertical => Quaternion.LookRotation(Vector3.forward, rotation * Vector3.up),
                CapsuleDirection2D.Horizontal => Quaternion.LookRotation(Vector3.forward, zRotation90Deg * (rotation * Vector3.right)),
                _ => rotation
            };
        }

        private static Quaternion ToQuaternion2D(this Quaternion rotation, RotationSource rotationSource) => rotationSource switch
        {
            RotationSource.Rigidbody2D => Quaternion.Euler(0f, 0f, rotation.ToRotation2D()),
            RotationSource.ZOnly => Quaternion.Euler(0f, 0f, rotation.eulerAngles.z),
            _ => rotation
        };

        private static Quaternion ToQuaternion2D(this Rigidbody2D rigidbody2D, RotationSource rotationSource) => rotationSource switch
        {
            RotationSource.Rigidbody2D => Quaternion.Euler(0f, 0f, rigidbody2D.rotation),
            RotationSource.ZOnly => Quaternion.Euler(0f, 0f, rigidbody2D.transform.eulerAngles.z),
            _ => rigidbody2D.transform.rotation
        };

        public static float ToRotation2D(this Quaternion rotation)
        {
            const float Rot2Rot2D = 2f * Mathf.Rad2Deg;
            
            return Rot2Rot2D * (rotation.w < 0f
                ? Mathf.Atan2(-rotation.z, -rotation.w)
                : Mathf.Atan2(rotation.z, rotation.w));
        }
    }
}
