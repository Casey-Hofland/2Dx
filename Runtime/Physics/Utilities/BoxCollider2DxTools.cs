#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static class BoxCollider2DxTools
    {
        /// <include file='../Documentation.xml' path='docs/BoxCollider2DxTools/IsBox/*'/>
        public static bool IsBoxCollider(this PolygonCollider2D polygonCollider2D/*, out Quaternion rotation*/)
        {
            // Box consists of 1 path.
            if (polygonCollider2D.pathCount != 1)
            {
                return false;
            }

            // Ensure the polygon collider 2D is active and enabled so we can get the shape count.
            var enabled = polygonCollider2D.enabled;
            polygonCollider2D.enabled = true;

            var gameObject = polygonCollider2D.gameObject;
            Queue<bool> actives = new();
            while (!polygonCollider2D.isActiveAndEnabled)
            {
                actives.Enqueue(gameObject.activeInHierarchy);
                gameObject.SetActive(true);

                if (gameObject.transform.parent == null)
                {
                    break;
                }
                gameObject = gameObject.transform.parent.gameObject;
            }
            var shapeCount = polygonCollider2D.shapeCount;

            gameObject = polygonCollider2D.gameObject;
            while (actives.TryDequeue(out bool active))
            {
                gameObject.SetActive(active);

                if (gameObject.transform.parent == null)
                {
                    break;
                }
                gameObject = gameObject.transform.parent.gameObject;
            }

            polygonCollider2D.enabled = enabled;

            // Boxes consist of 1 shape.
            if (shapeCount != 1)
            {
                return false;
            }

            // Boxes consist of either 4 points for a 2D representation, or 6 points for a 3D representation.
            var pointCount = polygonCollider2D.GetTotalPointCount();
            if (pointCount != 4 && pointCount != 6)
            {
                return false;
            }

            // Boxes have opposite vertexes from their center.
            for (int i = 0, j = pointCount / 2; i < j; i++)
            {
                if (polygonCollider2D.points[i] != -polygonCollider2D.points[i + j])
                {
                    return false;
                }
            }

            // If there are 6 points in the polygon collider we implicitely know it is a box shape because:
            //  - The first 3 points are opposite the last 3 points AND
            //  - The collider creates only 1 shape.
            // The first points proves the shape is parallel.
            // The second point proves no angles exceed 180 degrees.
            // These facts combined prove that the shape must be a 3D representation of a Box.
            if (pointCount == 6)
            {
                return true;
            }

            // If there are 4 points, check if the polygon collider is right angled.
            // We can skip a Dot operation by checking (1x, 1y) == -(0x, 2y) instead.
            return polygonCollider2D.points[1].x == -polygonCollider2D.points[0].x
                && polygonCollider2D.points[1].y == -polygonCollider2D.points[2].y;
        }

        /// <include file='../Documentation.xml' path='docs/BoxCollider2DxTools/CreateBox/*'/>
        public static void CreateBoxCollider(this PolygonCollider2D polygonCollider2D) => polygonCollider2D.CreateBoxCollider(Vector2.one);
        /// <include file='../Documentation.xml' path='docs/BoxCollider2DxTools/CreateBox/*'/>
        public static void CreateBoxCollider(this PolygonCollider2D polygonCollider2D, Vector2 size)
        {
            var points = new Vector2[4];
            points[2] = -(points[0] = size * 0.5f);
            points[3] = -(points[1] = new Vector2(-size.x, size.y) * 0.5f);

            polygonCollider2D.pathCount = 1;
            polygonCollider2D.SetPath(0, points);
        }

        /// <include file='../Documentation.xml' path='docs/BoxCollider2DxTools/CreateBox/*'/>
        public static void CreateBoxCollider(this PolygonCollider2D polygonCollider2D, Quaternion rotation) => polygonCollider2D.CreateBoxCollider(rotation, Vector3.one);
        /// <include file='../Documentation.xml' path='docs/BoxCollider2DxTools/CreateBox/*'/>
        public static void CreateBoxCollider(this PolygonCollider2D polygonCollider2D, Quaternion rotation, Vector3 size)
        {
            throw new NotImplementedException();
        }

        public static void RotateBoxCollider(this PolygonCollider2D polygonCollider2D, Quaternion rotation)
        {
            throw new NotImplementedException();
        }

        public static void ResizeBoxCollider(this PolygonCollider2D polygonCollider2D, Vector3 size)
        {
            throw new NotImplementedException();
        }

        /// <include file='../Documentation.xml' path='docs/BoxCollider2DxTools/Box2D/*'/>
        public static void ToPolygonCollider2D(this BoxCollider2D boxCollider2D, PolygonCollider2D polygonCollider2D)
        {
            polygonCollider2D.enabled = boxCollider2D.enabled;
            if(polygonCollider2D.attachedRigidbody && polygonCollider2D.attachedRigidbody.useAutoMass)
            {
                polygonCollider2D.density = boxCollider2D.density;
            }
            polygonCollider2D.sharedMaterial = boxCollider2D.sharedMaterial;
            polygonCollider2D.isTrigger = boxCollider2D.isTrigger;
            polygonCollider2D.usedByEffector = boxCollider2D.usedByEffector;
            polygonCollider2D.usedByComposite = boxCollider2D.usedByComposite;
            polygonCollider2D.autoTiling = boxCollider2D.autoTiling;
            polygonCollider2D.offset = boxCollider2D.offset;

            polygonCollider2D.CreateBoxCollider(boxCollider2D.size);
        }
    }
}
