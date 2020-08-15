﻿using System.Collections.Generic;
using UnityEngine;

namespace Physics2DxSystem.Utilities
{
    public static class Copier2Dx
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

        public static void ToSphereCollider(this SphereCollider sphereCollider, SphereCollider other)
        {
            sphereCollider.GenericPropertiesToCollider(other);

            other.isTrigger = sphereCollider.isTrigger;
            other.center = sphereCollider.center;
            other.radius = sphereCollider.radius;
        }

        public static void ToCircleCollider2D(this CircleCollider2D circleCollider2D, CircleCollider2D other)
        {
            circleCollider2D.GenericPropertiesToCollider2D(other);

            other.radius = circleCollider2D.radius;
        }

        public static void ToCapsuleCollider(this CapsuleCollider capsuleCollider, CapsuleCollider other)
        {
            capsuleCollider.GenericPropertiesToCollider(other);

            other.isTrigger = capsuleCollider.isTrigger;
            other.center = capsuleCollider.center;
            other.radius = capsuleCollider.radius;
            other.height = capsuleCollider.height;
            other.direction = capsuleCollider.direction;
        }

        public static void ToCapsuleCollider2D(this CapsuleCollider2D capsuleCollider2D, CapsuleCollider2D other)
        {
            capsuleCollider2D.GenericPropertiesToCollider2D(other);

            other.size = capsuleCollider2D.size;
            other.direction = capsuleCollider2D.direction;
        }

        public static void ToBoxCollider(this BoxCollider boxCollider, BoxCollider other)
        {
            boxCollider.GenericPropertiesToCollider(other);

            other.isTrigger = boxCollider.isTrigger;
            other.center = boxCollider.center;
            other.size = boxCollider.size;
        }

        public static void ToBoxCollider2D(this BoxCollider2D boxCollider2D, BoxCollider2D other)
        {
            boxCollider2D.GenericPropertiesToCollider2D(other);

            other.usedByComposite = boxCollider2D.usedByComposite;
            other.autoTiling = boxCollider2D.autoTiling;
            other.size = boxCollider2D.size;
            other.edgeRadius = boxCollider2D.edgeRadius;
        }

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
    }
}

