using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics2DxSystem.Utilities
{
    public static class Converter2Dx
    {
        #region PhysicMaterial
        private static Dictionary<PhysicMaterial, PhysicsMaterial2D> physicMaterialPairs = new Dictionary<PhysicMaterial, PhysicsMaterial2D>();
        private static Dictionary<PhysicsMaterial2D, PhysicMaterial> physicsMaterial2DPairs = new Dictionary<PhysicsMaterial2D, PhysicMaterial>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitPhysicMaterialPairs()
        {
            physicMaterialPairs = new Dictionary<PhysicMaterial, PhysicsMaterial2D>();
            physicsMaterial2DPairs = new Dictionary<PhysicsMaterial2D, PhysicMaterial>();
        }

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/PhysicMaterial/*'/>
        public static PhysicsMaterial2D AsPhysicsMaterial2D(this PhysicMaterial physicMaterial)
        {
            if(!physicMaterial)
            {
                return null;
            }

            // If there is no 2D equivalent of the physicMaterial, create a new one.
            if(!physicMaterialPairs.TryGetValue(physicMaterial, out var physicsMaterial2D))
            {
                physicsMaterial2D = new PhysicsMaterial2D();

                physicMaterialPairs.Add(physicMaterial, physicsMaterial2D);
                physicsMaterial2DPairs.Add(physicsMaterial2D, physicMaterial);
            }

            // Carry over the physicMaterial settings to its 2D equivalent.
            physicsMaterial2D.name = physicMaterial.name;
            physicsMaterial2D.hideFlags = physicMaterial.hideFlags;
            physicsMaterial2D.bounciness = physicMaterial.bounciness;
            physicsMaterial2D.friction = physicMaterial.dynamicFriction;

            return physicsMaterial2D;
        }

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/PhysicsMaterial2D/*'/>
        public static PhysicMaterial AsPhysicMaterial(this PhysicsMaterial2D physicsMaterial2D)
        {
            if(!physicsMaterial2D)
            {
                return null;
            }

            // If there is no 3D equivalent of the physicsMaterial2D, create a new one.
            if(!physicsMaterial2DPairs.TryGetValue(physicsMaterial2D, out var physicMaterial))
            {
                physicMaterial = new PhysicMaterial();

                physicsMaterial2DPairs.Add(physicsMaterial2D, physicMaterial);
                physicMaterialPairs.Add(physicMaterial, physicsMaterial2D);
            }

            // Carry over the physicsMaterial2D settings to its 3D equivalent.
            physicMaterial.name = physicsMaterial2D.name;
            physicMaterial.hideFlags = physicsMaterial2D.hideFlags;
            physicMaterial.bounciness = physicsMaterial2D.bounciness;
            physicMaterial.dynamicFriction = physicsMaterial2D.friction;

            return physicMaterial;
        }
        #endregion

        #region Rigidbody
        /// <include file='../Documentation.xml' path='docs/Convert2dx/Rigidbody/*'/>
        public static void ToRigidbody2D(this Rigidbody rigidbody, Rigidbody2D rigidbody2D)
        {
            // Carry over the Rigidbody settings to its 2D equivalent.
            rigidbody2D.angularDrag = Mathf.Min(rigidbody.angularDrag, 1000000f);
            rigidbody2D.angularVelocity = rigidbody.angularVelocity.z * Mathf.Rad2Deg;
            rigidbody2D.bodyType =
                rigidbody.isKinematic
                ? RigidbodyType2D.Kinematic
                : RigidbodyType2D.Dynamic;
            rigidbody2D.collisionDetectionMode =
                (rigidbody.collisionDetectionMode == CollisionDetectionMode.Discrete)
                ? CollisionDetectionMode2D.Discrete
                : CollisionDetectionMode2D.Continuous;
            rigidbody2D.drag = Mathf.Min(rigidbody.drag, 1000000f);
            rigidbody2D.gravityScale =
                rigidbody.useGravity
                ? 1f
                : 0f;
            rigidbody2D.hideFlags = rigidbody.hideFlags;
            rigidbody2D.interpolation = (RigidbodyInterpolation2D)rigidbody.interpolation;
            rigidbody2D.mass = Mathf.Min(rigidbody.mass, 1000000);
            rigidbody2D.sleepMode = RigidbodySleepMode2D.StartAwake;
            rigidbody2D.useAutoMass = false;
            rigidbody2D.velocity = rigidbody.velocity;

            rigidbody2D.constraints = RigidbodyConstraintsToRigidbodyConstraints2D(rigidbody.constraints);
        }

        /// <include file='../Documentation.xml' path='docs/Convert2dx/Rigidbody2D/*'/>
        public static void ToRigidbody(this Rigidbody2D rigidbody2D, Rigidbody rigidbody)
        {
            // Carry over the Rigidbody2D settings to its 3D equivalent.
            rigidbody.angularDrag = rigidbody2D.angularDrag;
            rigidbody.angularVelocity = Vector3.forward * rigidbody2D.angularVelocity * Mathf.Deg2Rad;
            //rigidbody.centerOfMass = new Vector3(rigidbody2D.centerOfMass.x, rigidbody2D.centerOfMass.y, rigidbody.centerOfMass.z);
            rigidbody.drag = rigidbody2D.drag;
            rigidbody.hideFlags = rigidbody2D.hideFlags;
            rigidbody.interpolation = (RigidbodyInterpolation)rigidbody2D.interpolation;
            rigidbody.isKinematic = rigidbody2D.bodyType != RigidbodyType2D.Dynamic;
            rigidbody.mass = rigidbody2D.mass;
            //rigidbody.position = new Vector3(rigidbody2D.position.x, rigidbody2D.position.y, rigidbody.position.z);
            rigidbody.useGravity = rigidbody2D.gravityScale > float.Epsilon;
            rigidbody.velocity = rigidbody2D.velocity;

            // Carry over the constraints2D to their 3D equivalent.
            if(rigidbody2D.constraints.HasFlag(RigidbodyConstraints2D.FreezePositionX))
            {
                rigidbody.constraints |= RigidbodyConstraints.FreezePositionX;
            }
            else if(rigidbody.constraints.HasFlag(RigidbodyConstraints.FreezePositionX))
            {
                rigidbody.constraints ^= RigidbodyConstraints.FreezePositionX;
            }

            if(rigidbody2D.constraints.HasFlag(RigidbodyConstraints2D.FreezePositionY))
            {
                rigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
            }
            else if(rigidbody.constraints.HasFlag(RigidbodyConstraints.FreezePositionY))
            {
                rigidbody.constraints ^= RigidbodyConstraints.FreezePositionY;
            }

            if(rigidbody2D.constraints.HasFlag(RigidbodyConstraints2D.FreezeRotation))
            {
                rigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
            }
            else if(rigidbody.constraints.HasFlag(RigidbodyConstraints.FreezeRotationZ))
            {
                rigidbody.constraints ^= RigidbodyConstraints.FreezeRotationZ;
            }
        }

        private static RigidbodyConstraints2D RigidbodyConstraintsToRigidbodyConstraints2D(RigidbodyConstraints constraints)
        {
            RigidbodyConstraints2D constraints2D = RigidbodyConstraints2D.None;

            if(constraints.HasFlag(RigidbodyConstraints.FreezePositionX))
            {
                constraints2D |= RigidbodyConstraints2D.FreezePositionX;
            }

            if(constraints.HasFlag(RigidbodyConstraints.FreezePositionY))
            {
                constraints2D |= RigidbodyConstraints2D.FreezePositionY;
            }

            if(constraints.HasFlag(RigidbodyConstraints.FreezeRotationZ))
            {
                constraints2D |= RigidbodyConstraints2D.FreezeRotation;
            }

            return constraints2D;
        }
        #endregion

        #region Colliders
        private static void CopyGenericProperties(this Collider collider, Collider2D collider2D)
        {
            collider2D.enabled = collider.enabled;
            collider2D.hideFlags = collider.hideFlags;
            collider2D.isTrigger = collider.isTrigger;
            collider2D.sharedMaterial = collider.sharedMaterial.AsPhysicsMaterial2D();
        }

        private static Vector2 CenterToOffset(Transform transform, Vector3 center, Transform transform2D)
        {
            return Quaternion.Inverse(transform2D.rotation) * transform.rotation * center;
        }

        private static void CopyGenericProperties(this Collider2D collider2D, Collider collider)
        {
            collider.enabled = collider2D.enabled;
            collider.hideFlags = collider2D.hideFlags;
            collider.isTrigger = collider2D.isTrigger;
            collider.sharedMaterial = collider2D.sharedMaterial.AsPhysicMaterial();
        }

        private static Vector3 OffsetToCenter(Transform transform2D, Vector2 offset, Transform transform, Vector3 center)
        {
            var relativeOffset = transform2D.rotation * offset;
            relativeOffset.z = (transform.rotation * center).z;
            return Quaternion.Inverse(transform.rotation) * relativeOffset;
        }

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Collider/*'/>
        public static void ToCollider2D(this Collider collider, Collider2D collider2D)
        {
            switch(collider)
            {
                case SphereCollider sphereCollider when collider2D is CircleCollider2D circleCollider2D:
                    sphereCollider.ToCircleCollider2D(circleCollider2D);
                    break;
                case CapsuleCollider capsuleCollider when collider2D is CapsuleCollider2D capsuleCollider2D:
                    capsuleCollider.ToCapsuleCollider2D(capsuleCollider2D);
                    break;
                default:
                    collider.CopyGenericProperties(collider2D);
                    break;
            }
        }

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Collider2D/*'/>
        public static void ToCollider(this Collider2D collider2D, Collider collider)
        {
            switch(collider2D)
            {
                case CircleCollider2D circleCollider2D when collider is SphereCollider sphereCollider:
                    circleCollider2D.ToSphereCollider(sphereCollider);
                    break;
                case CapsuleCollider2D capsuleCollider2D when collider is CapsuleCollider capsuleCollider:
                    capsuleCollider2D.ToCapsuleCollider(capsuleCollider);
                    break;
                default:
                    collider2D.CopyGenericProperties(collider);
                    break;
            }
        }

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Sphere/*'/>
        public static void ToCircleCollider2D(this SphereCollider sphereCollider, CircleCollider2D circleCollider2D)
        {
            sphereCollider.CopyGenericProperties(circleCollider2D);

            // Set the CircleCollider2D settings.
            circleCollider2D.offset = CenterToOffset(sphereCollider.transform, sphereCollider.center, circleCollider2D.transform);
            circleCollider2D.radius = sphereCollider.radius;
        }

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Circle2D/*'/>
        public static void ToSphereCollider(this CircleCollider2D circleCollider2D, SphereCollider sphereCollider)
        {
            circleCollider2D.CopyGenericProperties(sphereCollider);

            // Set the SphereCollider settings.
            sphereCollider.center = OffsetToCenter(circleCollider2D.transform, circleCollider2D.offset, sphereCollider.transform, sphereCollider.center);
            sphereCollider.radius = circleCollider2D.radius;
        }

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Capsule/*'/>
        public static void ToCapsuleCollider2D(this CapsuleCollider capsuleCollider, CapsuleCollider2D capsuleCollider2D)
        {
            capsuleCollider.CopyGenericProperties(capsuleCollider2D);
            
            // Set the CapsuleCollider2D offset.
            capsuleCollider2D.offset = CenterToOffset(capsuleCollider.transform, capsuleCollider.center, capsuleCollider2D.transform);

            // Determine if and in what direction to convert the CapsuleCollider2D size and direction.
            Vector3 direction;
            switch(capsuleCollider.direction)
            {
                case 0:
                    direction = capsuleCollider.transform.right;
                    capsuleCollider2D.direction = CapsuleDirection2D.Horizontal;
                    break;
                case 1:
                    direction = capsuleCollider.transform.up;
                    capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
                    break;
                case 2:
                    Debug.LogWarning($"Capsule Collider direction Z-AXIS is unsupported for 2D conversion. Capsule Collider 2D size and direction won't be converted.");
                    return;
                default:
                    return;
            }

            // Get the 2D distance that the capsules ends are apart.
            var diameter = capsuleCollider.radius * 2;
            var point1 = direction * (capsuleCollider.height + diameter) * 0.5f;
            var point2 = point1 - direction * (capsuleCollider.height - diameter);
            var distance = Vector2.Distance(point1, point2);

            // Set the CapsuleCollider2D size based on its direction.
            switch(capsuleCollider2D.direction)
            {
                case CapsuleDirection2D.Vertical:
                    capsuleCollider2D.size = new Vector2(diameter, diameter + distance);
                    break;
                case CapsuleDirection2D.Horizontal:
                    capsuleCollider2D.size = new Vector2(diameter + distance, diameter);
                    break;
            }
        }

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Capsule2D/*'/>
        public static void ToCapsuleCollider(this CapsuleCollider2D capsuleCollider2D, CapsuleCollider capsuleCollider)
        {
            capsuleCollider2D.CopyGenericProperties(capsuleCollider);

            // Set the CapsuleCollider center.
            capsuleCollider.center = OffsetToCenter(capsuleCollider2D.transform, capsuleCollider2D.offset, capsuleCollider.transform, capsuleCollider.center);

            // Determine if the CapsuleCollider can be converted to.
            if(capsuleCollider.direction == 2)
            {
                Debug.LogWarning($"Capsule Collider direction Z-AXIS is unsupported for 3D conversion. Capsule Collider radius, height and direction won't be converted.");
                return;
            }

            // Determine in what direction to convert the CapsuleCollider radius, height and direction.
            float angle;
            float diameter;
            float distance2D;
            switch(capsuleCollider2D.direction)
            {
                case CapsuleDirection2D.Vertical:
                    capsuleCollider.direction = 1;
                    angle = Vector3.Angle(capsuleCollider2D.transform.up, capsuleCollider.transform.up);
                    diameter = capsuleCollider2D.size.x;
                    distance2D = capsuleCollider2D.size.y - diameter;
                    break;
                case CapsuleDirection2D.Horizontal:
                    capsuleCollider.direction = 0;
                    angle = Vector3.Angle(capsuleCollider2D.transform.right, capsuleCollider.transform.right);
                    diameter = capsuleCollider2D.size.y;
                    distance2D = capsuleCollider2D.size.x - diameter;
                    break;
                default:
                    return;
            }

            // Calculate the 3D height based on the CapsuleCollider2Ds height and rotation.
            var height = diameter;
            if(!Mathf.Approximately(angle, 90f))
            {
                var opposite = Mathf.Tan(angle * Mathf.Deg2Rad) * distance2D;
                height += Mathf.Sqrt(distance2D * distance2D + opposite * opposite);
            }

            capsuleCollider.radius = diameter * 0.5f;
            capsuleCollider.height = height;
        }
        #endregion
    }
}

