#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsCopy
    {
        private static void GenericPropertiesToCollider(this Collider collider, Collider other)
        {
            other.enabled = collider.enabled;

            other.contactOffset = collider.contactOffset;
            other.sharedMaterial = collider.sharedMaterial;
        }

        private static void GenericPropertiesToCollider2D(this Collider2D collider2D, Collider2D other)
        {
            other.enabled = collider2D.enabled;

            if (other.attachedRigidbody && other.attachedRigidbody.useAutoMass)
            {
                other.density = collider2D.density;
            }
            other.isTrigger = collider2D.isTrigger;
            other.offset = collider2D.offset;
            other.sharedMaterial = collider2D.sharedMaterial;
            //other.usedByComposite = collider2D.usedByComposite;   // DON'T add used by composite: it has special checks that throws errors when this value is set on certain colliders.
            other.usedByEffector = collider2D.usedByEffector;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Sphere/*'/>
        public static void ToSphereCollider(this SphereCollider sphereCollider, SphereCollider other)
        {
            sphereCollider.GenericPropertiesToCollider(other);

            other.isTrigger = sphereCollider.isTrigger;
            other.center = sphereCollider.center;
            other.radius = sphereCollider.radius;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Circle2D/*'/>
        public static void ToCircleCollider2D(this CircleCollider2D circleCollider2D, CircleCollider2D other)
        {
            circleCollider2D.GenericPropertiesToCollider2D(other);

            other.radius = circleCollider2D.radius;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Capsule/*'/>
        public static void ToCapsuleCollider(this CapsuleCollider capsuleCollider, CapsuleCollider other)
        {
            capsuleCollider.GenericPropertiesToCollider(other);

            other.isTrigger = capsuleCollider.isTrigger;
            other.center = capsuleCollider.center;
            other.radius = capsuleCollider.radius;
            other.height = capsuleCollider.height;
            other.direction = capsuleCollider.direction;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Capsule2D/*'/>
        public static void ToCapsuleCollider2D(this CapsuleCollider2D capsuleCollider2D, CapsuleCollider2D other)
        {
            capsuleCollider2D.GenericPropertiesToCollider2D(other);

            other.size = capsuleCollider2D.size;
            other.direction = capsuleCollider2D.direction;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Box/*'/>
        public static void ToBoxCollider(this BoxCollider boxCollider, BoxCollider other)
        {
            boxCollider.GenericPropertiesToCollider(other);

            other.isTrigger = boxCollider.isTrigger;
            other.center = boxCollider.center;
            other.size = boxCollider.size;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Box2D/*'/>
        public static void ToBoxCollider2D(this BoxCollider2D boxCollider2D, BoxCollider2D other)
        {
            boxCollider2D.GenericPropertiesToCollider2D(other);

            other.usedByComposite = boxCollider2D.usedByComposite;
            other.autoTiling = boxCollider2D.autoTiling;
            other.size = boxCollider2D.size;
            other.edgeRadius = boxCollider2D.edgeRadius;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Mesh/*'/>
        public static void ToMeshCollider(this MeshCollider meshCollider, MeshCollider other)
        {
            meshCollider.GenericPropertiesToCollider(other);

            if (other.convex = meshCollider.convex)
            {
                other.isTrigger = meshCollider.isTrigger;
            }
            other.cookingOptions = meshCollider.cookingOptions;
            other.sharedMesh = meshCollider.sharedMesh;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/Polygon2D/*'/>
        public static void ToPolygonCollider2D(this PolygonCollider2D polygonCollider2D, PolygonCollider2D other)
        {
            polygonCollider2D.GenericPropertiesToCollider2D(other);

            other.usedByComposite = polygonCollider2D.usedByComposite;
            other.autoTiling = polygonCollider2D.autoTiling;
            other.pathCount = polygonCollider2D.pathCount;
            for (int i = 0; i < other.pathCount; i++)
            {
                polygonCollider2D.GetPath(i, points);
                polygonCollider2D.SetPath(i, points);
            }
        }

        private static List<Vector2> points = new List<Vector2>();
    }
}
