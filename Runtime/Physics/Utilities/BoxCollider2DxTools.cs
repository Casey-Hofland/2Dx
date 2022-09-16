#nullable enable
using System;
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static class BoxCollider2DxTools
    {
        /// <include file='../Documentation.xml' path='docs/BoxCollider2DxTools/IsBox/*'/>
        public static bool IsBoxCollider(this PolygonCollider2D polygonCollider2D, Quaternion rotation)
        {
            if(polygonCollider2D.pathCount == 1)
            {
                var pointCount = polygonCollider2D.GetTotalPointCount();

                if(pointCount == 4)
                {
                    return
                        polygonCollider2D.points[0].x >= 0
                        && polygonCollider2D.points[0].y >= 0
                        && polygonCollider2D.points[0] == -polygonCollider2D.points[2]
                        && polygonCollider2D.points[1] == -polygonCollider2D.points[3]
                        && polygonCollider2D.points[0].x == -polygonCollider2D.points[1].x
                        && polygonCollider2D.points[0].y == polygonCollider2D.points[1].y
                        && ((Vector2)(rotation * Vector3.right) == Vector2.zero
                            || (Vector2)(rotation * Vector3.up) == Vector2.zero
                            || (Vector2)(rotation * Vector3.forward) == Vector2.zero);
                }
                else if(pointCount == 6)
                {
                    if(polygonCollider2D.points[0] == -polygonCollider2D.points[3]
                        && polygonCollider2D.points[1] == -polygonCollider2D.points[4]
                        && polygonCollider2D.points[2] == -polygonCollider2D.points[5])
                    {
                        Vector2 right = rotation * Vector3.right;
                        Vector2 up = rotation * Vector3.up;
                        Vector2 forward = rotation * Vector3.forward;

                        var relativeRotation = polygonCollider2D.transform.rotation;
                        Vector2 relativeRight = relativeRotation * (polygonCollider2D.points[0] - polygonCollider2D.points[1]);
                        Vector2 relativeUp = relativeRotation * (polygonCollider2D.points[1] - polygonCollider2D.points[2]);
                        Vector2 relativeForward = relativeRotation * (polygonCollider2D.points[2] - polygonCollider2D.points[3]);

                        if(right == relativeRight)
                        {
                            if(up == relativeUp)
                            {
                                return forward == relativeForward;
                            }
                            else if(up == -relativeUp)
                            {
                                return forward == -relativeForward;
                            }
                        }
                        else if(right == -relativeRight)
                        {
                            if(up == relativeUp)
                            {
                                return forward == -relativeForward;
                            }
                            else if(up == -relativeUp)
                            {
                                return forward == relativeForward;
                            }
                        }
                    }
                }
            }

            return false;
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
