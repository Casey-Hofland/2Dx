#nullable enable
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsCopy
    {
        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Rigidbody/*'/>
        public static void ToRigidbody(this Rigidbody rigidbody, Rigidbody other) => rigidbody.ToRigidbody(other, false);
        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Rigidbody/*'/>
        public static void ToRigidbody(this Rigidbody rigidbody, Rigidbody other, bool setPosition) => rigidbody.ToRigidbody(other, setPosition, false);
        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Rigidbody/*'/>
        public static void ToRigidbody(this Rigidbody rigidbody, Rigidbody other, bool setPosition, bool setRotation)
        {
            other.angularDrag = rigidbody.angularDrag;
            other.angularVelocity = rigidbody.angularVelocity;
            other.centerOfMass = rigidbody.centerOfMass;
            other.collisionDetectionMode = rigidbody.collisionDetectionMode;
            other.constraints = rigidbody.constraints;
            other.detectCollisions = rigidbody.detectCollisions;
            other.drag = rigidbody.drag;
            //other.inertiaTensor = rigidbody.inertiaTensor;
            //other.inertiaTensorRotation = rigidbody.inertiaTensorRotation;
            //other.ResetInertiaTensor();
            other.interpolation = rigidbody.interpolation;
            other.isKinematic = rigidbody.isKinematic;
            other.mass = rigidbody.mass;
            other.maxAngularVelocity = rigidbody.maxAngularVelocity;
            other.maxDepenetrationVelocity = rigidbody.maxDepenetrationVelocity;
            if (setPosition)
            {
                other.position = rigidbody.position;
            }
            if (setRotation)
            {
                other.rotation = rigidbody.rotation;
            }
            other.sleepThreshold = rigidbody.sleepThreshold;
            other.solverIterations = rigidbody.solverIterations;
            other.useGravity = rigidbody.useGravity;
            other.velocity = rigidbody.velocity;

        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Rigidbody2D/*'/>
        public static void ToRigidbody2D(this Rigidbody2D rigidbody2D, Rigidbody2D other) => rigidbody2D.ToRigidbody2D(other, false);
        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Rigidbody2D/*'/>
        public static void ToRigidbody2D(this Rigidbody2D rigidbody2D, Rigidbody2D other, bool setPosition) => rigidbody2D.ToRigidbody2D(other, setPosition, false);
        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Rigidbody2D/*'/>
        public static void ToRigidbody2D(this Rigidbody2D rigidbody2D, Rigidbody2D other, bool setPosition, bool setRotation)
        {
            other.angularDrag = rigidbody2D.angularDrag;
            other.angularVelocity = rigidbody2D.angularVelocity;
            other.bodyType = rigidbody2D.bodyType;
            other.centerOfMass = rigidbody2D.centerOfMass;
            other.collisionDetectionMode = rigidbody2D.collisionDetectionMode;
            other.constraints = rigidbody2D.constraints;
            other.drag = rigidbody2D.drag;
            other.gravityScale = rigidbody2D.gravityScale;
            //other.inertia = rigidbody2D.inertia;
            other.interpolation = rigidbody2D.interpolation;
            other.isKinematic = rigidbody2D.isKinematic;
            if (!(other.useAutoMass = rigidbody2D.useAutoMass))
            {
                other.mass = rigidbody2D.mass;
            }
            if (setPosition)
            {
                other.position = rigidbody2D.position;
            }
            if (setRotation)
            {
                other.rotation = rigidbody2D.rotation;
            }
            other.sharedMaterial = rigidbody2D.sharedMaterial;
            other.simulated = rigidbody2D.simulated;
            other.sleepMode = rigidbody2D.sleepMode;
            other.useFullKinematicContacts = rigidbody2D.useFullKinematicContacts;
            other.velocity = rigidbody2D.velocity;
        }
    }
}
