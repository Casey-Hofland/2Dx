#nullable enable
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsConvert
    {
        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Rigidbody/*'/>
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
            rigidbody2D.mass = Mathf.Min(rigidbody.mass, 1000000f);
            rigidbody2D.sleepMode = RigidbodySleepMode2D.StartAwake;
            rigidbody2D.useAutoMass = false;
            rigidbody2D.velocity = rigidbody.velocity;

            rigidbody2D.constraints = rigidbody.constraints.ToRigidbodyConstraints2D();
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Rigidbody2D/*'/>
        public static void ToRigidbody(this Rigidbody2D rigidbody2D, Rigidbody rigidbody)
        {
            // Carry over the Rigidbody2D settings to its 3D equivalent.
            rigidbody.angularDrag = rigidbody2D.angularDrag;
            rigidbody.angularVelocity = Vector3.forward * rigidbody2D.angularVelocity * Mathf.Deg2Rad;
            rigidbody.drag = rigidbody2D.drag;
            rigidbody.hideFlags = rigidbody2D.hideFlags;
            rigidbody.interpolation = (RigidbodyInterpolation)rigidbody2D.interpolation;
            rigidbody.isKinematic = rigidbody2D.bodyType != RigidbodyType2D.Dynamic;
            rigidbody.mass = rigidbody2D.mass;
            rigidbody.useGravity = rigidbody2D.gravityScale > float.Epsilon;
            rigidbody.velocity = rigidbody2D.velocity;

            rigidbody.constraints = rigidbody2D.constraints.ToRigidbodyConstraints(rigidbody.constraints);
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/RigidbodyConstraints/*'/>
        public static RigidbodyConstraints2D ToRigidbodyConstraints2D(this RigidbodyConstraints constraints)
        {
            RigidbodyConstraints2D constraints2D = RigidbodyConstraints2D.None;

            if (constraints.HasFlag(RigidbodyConstraints.FreezePositionX))
            {
                constraints2D |= RigidbodyConstraints2D.FreezePositionX;
            }

            if (constraints.HasFlag(RigidbodyConstraints.FreezePositionY))
            {
                constraints2D |= RigidbodyConstraints2D.FreezePositionY;
            }

            if (constraints.HasFlag(RigidbodyConstraints.FreezeRotationZ))
            {
                constraints2D |= RigidbodyConstraints2D.FreezeRotation;
            }

            return constraints2D;
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/RigidbodyConstraints2D/*'/>
        public static RigidbodyConstraints ToRigidbodyConstraints(this RigidbodyConstraints2D constraints2D, RigidbodyConstraints constraints)
        {
            if (constraints2D.HasFlag(RigidbodyConstraints2D.FreezePositionX))
            {
                constraints |= RigidbodyConstraints.FreezePositionX;
            }
            else
            {
                constraints &= ~RigidbodyConstraints.FreezePositionX;
            }

            if (constraints2D.HasFlag(RigidbodyConstraints2D.FreezePositionY))
            {
                constraints |= RigidbodyConstraints.FreezePositionY;
            }
            else
            {
                constraints &= ~RigidbodyConstraints.FreezePositionY;
            }

            if (constraints2D.HasFlag(RigidbodyConstraints2D.FreezeRotation))
            {
                constraints |= RigidbodyConstraints.FreezeRotationZ;
            }
            else
            {
                constraints &= ~RigidbodyConstraints.FreezeRotationZ;
            }

            return constraints;
        }
    }
}
