#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static class PhysicsCopier
    {
        #region Colliders
        private static void GenericPropertiesToCollider(this Collider collider, Collider other)
        {
            other.enabled = collider.enabled;
            other.hideFlags = collider.hideFlags;
            other.contactOffset = collider.contactOffset;
            other.sharedMaterial = collider.sharedMaterial;
        }

        private static void GenericPropertiesToCollider2D(this Collider2D collider2D, Collider2D other)
        {
            other.enabled = collider2D.enabled;
            other.hideFlags = collider2D.hideFlags;
            if(other.attachedRigidbody && other.attachedRigidbody.useAutoMass)
            {
                other.density = collider2D.density;
            }
            other.sharedMaterial = collider2D.sharedMaterial;
            other.isTrigger = collider2D.isTrigger;
            other.usedByEffector = collider2D.usedByEffector;
            other.offset = collider2D.offset;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Collider/*'/>
        public static void ToCollider(this Collider collider, Collider other)
        {
            switch(collider)
            {
                case SphereCollider sphereCollider when other is SphereCollider sphere:
                    sphereCollider.ToSphereCollider(sphere);
                    break;
                case CapsuleCollider capsuleCollider when other is CapsuleCollider capsule:
                    capsuleCollider.ToCapsuleCollider(capsule);
                    break;
                case BoxCollider boxCollider when other is BoxCollider box:
                    boxCollider.ToBoxCollider(box);
                    break;
                case MeshCollider meshCollider when other is MeshCollider mesh:
                    meshCollider.ToMeshCollider(mesh);
                    break;
                default:
                    collider.GenericPropertiesToCollider(other);
                    break;
            }
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Collider2D/*'/>
        public static void ToCollider2D(this Collider2D collider2D, Collider2D other)
        {
            switch(collider2D)
            {
                case CircleCollider2D circleCollider2D when other is CircleCollider2D circle:
                    circleCollider2D.ToCircleCollider2D(circle);
                    break;
                case CapsuleCollider2D capsuleCollider2D when other is CapsuleCollider2D capsule:
                    capsuleCollider2D.ToCapsuleCollider2D(capsule);
                    break;
                case BoxCollider2D boxCollider2D when other is BoxCollider2D box:
                    boxCollider2D.ToBoxCollider2D(box);
                    break;
                case PolygonCollider2D polygonCollider2D when other is PolygonCollider2D polygon:
                    polygonCollider2D.ToPolygonCollider2D(polygon);
                    break;
                default:
                    collider2D.GenericPropertiesToCollider2D(other);
                    break;
            }
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Sphere/*'/>
        public static void ToSphereCollider(this SphereCollider sphereCollider, SphereCollider other)
        {
            sphereCollider.GenericPropertiesToCollider(other);

            other.isTrigger = sphereCollider.isTrigger;
            other.center = sphereCollider.center;
            other.radius = sphereCollider.radius;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Circle2D/*'/>
        public static void ToCircleCollider2D(this CircleCollider2D circleCollider2D, CircleCollider2D other)
        {
            circleCollider2D.GenericPropertiesToCollider2D(other);

            other.radius = circleCollider2D.radius;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Capsule/*'/>
        public static void ToCapsuleCollider(this CapsuleCollider capsuleCollider, CapsuleCollider other)
        {
            capsuleCollider.GenericPropertiesToCollider(other);

            other.isTrigger = capsuleCollider.isTrigger;
            other.center = capsuleCollider.center;
            other.radius = capsuleCollider.radius;
            other.height = capsuleCollider.height;
            other.direction = capsuleCollider.direction;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Capsule2D/*'/>
        public static void ToCapsuleCollider2D(this CapsuleCollider2D capsuleCollider2D, CapsuleCollider2D other)
        {
            capsuleCollider2D.GenericPropertiesToCollider2D(other);

            other.size = capsuleCollider2D.size;
            other.direction = capsuleCollider2D.direction;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Box/*'/>
        public static void ToBoxCollider(this BoxCollider boxCollider, BoxCollider other)
        {
            boxCollider.GenericPropertiesToCollider(other);

            other.isTrigger = boxCollider.isTrigger;
            other.center = boxCollider.center;
            other.size = boxCollider.size;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Box2D/*'/>
        public static void ToBoxCollider2D(this BoxCollider2D boxCollider2D, BoxCollider2D other)
        {
            boxCollider2D.GenericPropertiesToCollider2D(other);

            other.usedByComposite = boxCollider2D.usedByComposite;
            other.autoTiling = boxCollider2D.autoTiling;
            other.size = boxCollider2D.size;
            other.edgeRadius = boxCollider2D.edgeRadius;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Mesh/*'/>
        public static void ToMeshCollider(this MeshCollider meshCollider, MeshCollider other)
        {
            meshCollider.GenericPropertiesToCollider(other);

            if(other.convex = meshCollider.convex)
            {
                other.isTrigger = meshCollider.isTrigger;
            }
            other.cookingOptions = meshCollider.cookingOptions;
            other.sharedMesh = meshCollider.sharedMesh;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Polygon2D/*'/>
        public static void ToPolygonCollider2D(this PolygonCollider2D polygonCollider2D, PolygonCollider2D other)
        {
            polygonCollider2D.GenericPropertiesToCollider2D(other);

            other.usedByComposite = polygonCollider2D.usedByComposite;
            other.autoTiling = polygonCollider2D.autoTiling;
            other.pathCount = polygonCollider2D.pathCount;
            for(int i = 0; i < other.pathCount; i++)
            {
                polygonCollider2D.GetPath(i, points);
                polygonCollider2D.SetPath(i, points);
            }
        }

        private static List<Vector2> points = new List<Vector2>();
        #endregion

        #region Rigidbody
        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Rigidbody/*'/>
        public static void ToRigidbody(this Rigidbody rigidbody, Rigidbody other) => rigidbody.ToRigidbody(other, false);
        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Rigidbody/*'/>
        public static void ToRigidbody(this Rigidbody rigidbody, Rigidbody other, bool setPosition) => rigidbody.ToRigidbody(other, setPosition, false);
        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Rigidbody/*'/>
        public static void ToRigidbody(this Rigidbody rigidbody, Rigidbody other, bool setPosition, bool setRotation)
        {
            other.hideFlags = rigidbody.hideFlags;

            other.angularDrag = rigidbody.angularDrag;
            other.angularVelocity = rigidbody.angularVelocity;
            other.centerOfMass = rigidbody.centerOfMass;
            other.collisionDetectionMode = rigidbody.collisionDetectionMode;
            other.constraints = rigidbody.constraints;
            other.detectCollisions = rigidbody.detectCollisions;
            other.drag = rigidbody.drag;
            other.inertiaTensor = rigidbody.inertiaTensor;
            other.inertiaTensorRotation = rigidbody.inertiaTensorRotation;
            other.interpolation = rigidbody.interpolation;
            other.isKinematic = rigidbody.isKinematic;
            other.mass = rigidbody.mass;
            other.maxAngularVelocity = rigidbody.maxAngularVelocity;
            other.maxDepenetrationVelocity = rigidbody.maxDepenetrationVelocity;
            if(setPosition)
            {
                other.position = rigidbody.position;
            }
            if(setRotation)
            {
                other.rotation = rigidbody.rotation;
            }
            other.sleepThreshold = rigidbody.sleepThreshold;
            other.solverIterations = rigidbody.solverIterations;
            other.useGravity = rigidbody.useGravity;
            other.velocity = rigidbody.velocity;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Rigidbody2D/*'/>
        public static void ToRigidbody2D(this Rigidbody2D rigidbody2D, Rigidbody2D other) => rigidbody2D.ToRigidbody2D(other, false);
        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Rigidbody2D/*'/>
        public static void ToRigidbody2D(this Rigidbody2D rigidbody2D, Rigidbody2D other, bool setPosition) => rigidbody2D.ToRigidbody2D(other, setPosition, false);
        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/Rigidbody2D/*'/>
        public static void ToRigidbody2D(this Rigidbody2D rigidbody2D, Rigidbody2D other, bool setPosition, bool setRotation)
        {
            other.hideFlags = rigidbody2D.hideFlags;

            other.angularDrag = rigidbody2D.angularDrag;
            other.angularVelocity = rigidbody2D.angularVelocity;
            other.bodyType = rigidbody2D.bodyType;
            other.centerOfMass = rigidbody2D.centerOfMass;
            other.collisionDetectionMode = rigidbody2D.collisionDetectionMode;
            other.constraints = rigidbody2D.constraints;
            other.drag = rigidbody2D.drag;
            other.gravityScale = rigidbody2D.gravityScale;
            other.inertia = rigidbody2D.inertia;
            other.interpolation = rigidbody2D.interpolation;
            other.isKinematic = rigidbody2D.isKinematic;
            if(!(other.useAutoMass = rigidbody2D.useAutoMass))
            {
                other.mass = rigidbody2D.mass;
            }
            if(setPosition)
            {
                other.position = rigidbody2D.position;
            }
            if(setRotation)
            {
                other.rotation = rigidbody2D.rotation;
            }
            other.sharedMaterial = rigidbody2D.sharedMaterial;
            other.simulated = rigidbody2D.simulated;
            other.sleepMode = rigidbody2D.sleepMode;
            other.useFullKinematicContacts = rigidbody2D.useFullKinematicContacts;
            other.velocity = rigidbody2D.velocity;
        }
        #endregion

        #region ConstantForce
        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/ConstantForce/*'/>
        public static void ToConstantForce(this ConstantForce constantForce, ConstantForce other)
        {
            other.hideFlags = constantForce.hideFlags;

            other.force = constantForce.force;
            other.relativeForce = constantForce.relativeForce;
            other.torque = constantForce.torque;
            other.relativeTorque = constantForce.relativeTorque;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsCopier/ConstantForce2D/*'/>
        public static void ToConstantForce2D(this ConstantForce2D constantForce2D, ConstantForce2D other)
        {
            other.hideFlags = constantForce2D.hideFlags;

            other.force = constantForce2D.force;
            other.relativeForce = constantForce2D.relativeForce;
            other.torque = constantForce2D.torque;
        }
        #endregion

        #region Joints
        private static void GenericPropertiesToJoint(this Joint joint, Joint other)
        {
            other.hideFlags = joint.hideFlags;

            other.anchor = joint.anchor;
            other.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            other.axis = joint.axis;
            other.breakForce = joint.breakForce;
            other.breakTorque = joint.breakTorque;
            other.connectedAnchor = joint.connectedAnchor;
            other.connectedArticulationBody = joint.connectedArticulationBody;
            other.connectedBody = joint.connectedBody;
            other.connectedMassScale = joint.connectedMassScale;
            other.enableCollision = joint.enableCollision;
            other.enablePreprocessing = joint.enablePreprocessing;
            other.massScale = joint.massScale;
        }

        private static void GenericPropertiesToJoint2D(this Joint2D joint2D, Joint2D other)
        {
            other.enabled = joint2D.enabled;
            other.hideFlags = joint2D.hideFlags;

            other.breakForce = joint2D.breakForce;
            other.breakTorque = joint2D.breakTorque;
            other.connectedBody = joint2D.connectedBody;
            other.enableCollision = joint2D.enableCollision;
        }

        private static void GenericPropertiesToAnchoredJoint2D(this AnchoredJoint2D anchoredJoint2D, AnchoredJoint2D other)
        {
            anchoredJoint2D.GenericPropertiesToJoint2D(other);

            other.anchor = anchoredJoint2D.anchor;
            other.autoConfigureConnectedAnchor = anchoredJoint2D.autoConfigureConnectedAnchor;
            other.connectedAnchor = anchoredJoint2D.connectedAnchor;
        }

        public static void ToJoint(this Joint joint, Joint other)
        {
            switch (joint)
            {
                case FixedJoint fixedJoint when other is FixedJoint @fixed:
                    fixedJoint.ToFixedJoint(@fixed);
                    break;
                case SpringJoint springJoint when other is SpringJoint spring:
                    springJoint.ToSpringJoint(spring);
                    break;
                default:
                    joint.GenericPropertiesToJoint(other);
                    break;
            }
        }

        public static void ToJoint2D(this Joint2D joint2D, Joint2D other)
        {
            switch (joint2D)
            {
                case FixedJoint2D fixedJoint2D when other is FixedJoint2D @fixed:
                    fixedJoint2D.ToFixedJoint2D(@fixed);
                    break;
                case SpringJoint2D springJoint2D when other is SpringJoint2D spring:
                    springJoint2D.ToSpringJoint2D(spring);
                    break;
                case AnchoredJoint2D anchoredJoint2D when other is AnchoredJoint2D anchored:
                    anchoredJoint2D.GenericPropertiesToAnchoredJoint2D(anchored);
                    break;
                default:
                    joint2D.GenericPropertiesToJoint2D(other);
                    break;
            }
        }

        public static void ToFixedJoint(this FixedJoint fixedJoint, FixedJoint other)
        {
            fixedJoint.GenericPropertiesToJoint(other);
        }

        public static void ToFixedJoint2D(this FixedJoint2D fixedJoint2D, FixedJoint2D other)
        {
            fixedJoint2D.GenericPropertiesToAnchoredJoint2D(other);

            other.dampingRatio = fixedJoint2D.dampingRatio;
            other.frequency = fixedJoint2D.frequency;
        }

        public static void ToSpringJoint(this SpringJoint springJoint, SpringJoint other)
        {
            springJoint.GenericPropertiesToJoint(other);

            other.damper = springJoint.damper;
            other.maxDistance = springJoint.maxDistance;
            other.minDistance = springJoint.minDistance;
            other.spring = springJoint.spring;
            other.tolerance = springJoint.tolerance;
        }

        public static void ToSpringJoint2D(this SpringJoint2D springJoint2D, SpringJoint2D other)
        {
            springJoint2D.GenericPropertiesToAnchoredJoint2D(other);

            other.autoConfigureDistance = springJoint2D.autoConfigureDistance;
            other.dampingRatio = springJoint2D.dampingRatio;
            other.distance = springJoint2D.distance;
            other.frequency = springJoint2D.frequency;
        }
        #endregion
    }
}

