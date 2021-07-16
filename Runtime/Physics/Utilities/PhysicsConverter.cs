using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity2Dx.Physics
{
    public static class PhysicsConverter
    {
        #region Vector
        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Vector/*'/>
        public static Vector2 ToVector2D(this Vector3 direction, Component component, Component component2D)
        {
            return Quaternion.Inverse(component2D.transform.rotation) * component.transform.rotation * direction;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Vector2D/*'/>
        public static Vector3 ToVector(this Vector2 direction, Component component2D, Component component, Vector3 original)
        {
            var relativeVector3 = component2D.transform.rotation * direction;
            relativeVector3.z = (component.transform.rotation * original).z;
            return Quaternion.Inverse(component.transform.rotation) * relativeVector3;
        }
        #endregion

        #region Colliders
        private static void GenericPropertiesToCollider2D(this Collider collider, Collider2D collider2D)
        {
            collider2D.enabled = collider.enabled;
            collider2D.hideFlags = collider.hideFlags;
            collider2D.sharedMaterial = collider.sharedMaterial.AsPhysicsMaterial2D();
        }

        private static void GenericPropertiesToCollider(this Collider2D collider2D, Collider collider)
        {
            collider.enabled = collider2D.enabled;
            collider.hideFlags = collider2D.hideFlags;
            collider.sharedMaterial = collider2D.sharedMaterial.AsPhysicMaterial();
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Collider/*'/>
        public static void ToCollider2D(this Collider collider, Collider2D collider2D) => collider.ToCollider2D(collider2D, default);
        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Collider/*'/>
        public static void ToCollider2D(this Collider collider, Collider2D collider2D, ConversionSettings conversionSettings)
        {
            switch(collider)
            {
                case SphereCollider sphereCollider when collider2D is CircleCollider2D circleCollider2D:
                    sphereCollider.ToCircleCollider2D(circleCollider2D);
                    break;
                case CapsuleCollider capsuleCollider when collider2D is CapsuleCollider2D capsuleCollider2D:
                    capsuleCollider.ToCapsuleCollider2D(capsuleCollider2D);
                    break;
                case BoxCollider boxCollider when collider2D is PolygonCollider2D polygonCollider2D:
                    boxCollider.ToPolygonCollider2D(polygonCollider2D);
                    break;
                case MeshCollider meshCollider when collider2D is PolygonCollider2D polygonCollider2D:
                    meshCollider.ToPolygonCollider2D(polygonCollider2D, conversionSettings.outliner);
                    break;
                default:
                    collider.GenericPropertiesToCollider2D(collider2D);
                    break;
            }
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Collider2D/*'/>
        public static void ToCollider(this Collider2D collider2D, Collider collider) => collider2D.ToCollider(collider, default);
        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Collider2D/*'/>
        public static void ToCollider(this Collider2D collider2D, Collider collider, Conversion2DSettings conversion2DSettings)
        {
            switch(collider2D)
            {
                case CircleCollider2D circleCollider2D when collider is SphereCollider sphereCollider:
                    circleCollider2D.ToSphereCollider(sphereCollider);
                    break;
                case CapsuleCollider2D capsuleCollider2D when collider is CapsuleCollider capsuleCollider:
                    capsuleCollider2D.ToCapsuleCollider(capsuleCollider);
                    break;
                case PolygonCollider2D polygonCollider2D when collider is BoxCollider boxCollider:
                    if(conversion2DSettings.toBoxColliderSkipSafetyCheck)
                    {
                        polygonCollider2D.ToBoxCollider(boxCollider);
                    }
                    else
                    {
                        polygonCollider2D.ToBoxColliderSafe(boxCollider);
                    }
                    break;
                case PolygonCollider2D polygonCollider2D when collider is MeshCollider meshCollider:
                    polygonCollider2D.ToMeshCollider(meshCollider, conversion2DSettings.polygonCollider2DConversionOptions);
                    break;
                default:
                    collider2D.GenericPropertiesToCollider(collider);
                    break;
            }
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Sphere/*'/>
        public static void ToCircleCollider2D(this SphereCollider sphereCollider, CircleCollider2D circleCollider2D)
        {
            sphereCollider.GenericPropertiesToCollider2D(circleCollider2D);

            // Set the CircleCollider2D settings.
            circleCollider2D.isTrigger = sphereCollider.isTrigger;
            circleCollider2D.offset = sphereCollider.center.ToVector2D(sphereCollider, circleCollider2D);
            circleCollider2D.radius = sphereCollider.radius;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Circle2D/*'/>
        public static void ToSphereCollider(this CircleCollider2D circleCollider2D, SphereCollider sphereCollider)
        {
            circleCollider2D.GenericPropertiesToCollider(sphereCollider);

            // Set the SphereCollider settings.
            sphereCollider.isTrigger = circleCollider2D.isTrigger;
            sphereCollider.center = circleCollider2D.offset.ToVector(circleCollider2D, sphereCollider, sphereCollider.center);
            sphereCollider.radius = circleCollider2D.radius;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Capsule/*'/>
        public static void ToCapsuleCollider2D(this CapsuleCollider capsuleCollider, CapsuleCollider2D capsuleCollider2D)
        {
            capsuleCollider.GenericPropertiesToCollider2D(capsuleCollider2D);

            // Set the CapsuleCollider2D settings.
            capsuleCollider2D.isTrigger = capsuleCollider.isTrigger;
            capsuleCollider2D.offset = capsuleCollider.center.ToVector2D(capsuleCollider, capsuleCollider2D);

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
#if UNITY_EDITOR
                    Debug.LogWarning($"Capsule Collider direction Z-AXIS is unsupported for 2D conversion. Capsule Collider 2D size and direction won't be converted.");
#endif
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

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Capsule2D/*'/>
        public static void ToCapsuleCollider(this CapsuleCollider2D capsuleCollider2D, CapsuleCollider capsuleCollider)
        {
            capsuleCollider2D.GenericPropertiesToCollider(capsuleCollider);

            // Set the CapsuleCollider settings.
            capsuleCollider.isTrigger = capsuleCollider2D.isTrigger;
            capsuleCollider.center = capsuleCollider2D.offset.ToVector(capsuleCollider2D, capsuleCollider, capsuleCollider.center);

            // Determine if the CapsuleCollider can be converted to.
            if(capsuleCollider.direction == 2)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Capsule Collider direction Z-AXIS is unsupported for 3D conversion. Capsule Collider radius, height and direction won't be converted.");
#endif
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

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Box/*'/>
        public static void ToPolygonCollider2D(this BoxCollider boxCollider, PolygonCollider2D polygonCollider2D)
        {
            boxCollider.GenericPropertiesToCollider2D(polygonCollider2D);

            polygonCollider2D.isTrigger = boxCollider.isTrigger;

            var relativeRotation = Quaternion.Inverse(polygonCollider2D.transform.rotation) * boxCollider.transform.rotation;
            polygonCollider2D.offset = relativeRotation * boxCollider.center;

            // Get the Vector2 equivelent distances the points need to move in to form a Box cutout.
            Vector2 relativeRight = relativeRotation * Vector3.right * Mathf.Abs(boxCollider.size.x);
            Vector2 relativeUp = relativeRotation * Vector3.up * Mathf.Abs(boxCollider.size.y);
            Vector2 relativeForward = relativeRotation * Vector3.forward * Mathf.Abs(boxCollider.size.z);

            polygonCollider2D.pathCount = 1;

            // Check if this Box is directly facing the front from any angle.
            if (relativeRight == Vector2.zero)
            {
                boxPoints4[0] = (relativeForward + relativeUp) * 0.5f;
                boxPoints4[1] = boxPoints4[0] - relativeForward;
                boxPoints4[2] = boxPoints4[1] - relativeUp;
                boxPoints4[3] = boxPoints4[2] + relativeForward;

                polygonCollider2D.SetPath(0, boxPoints4);
            }
            else if (relativeUp == Vector2.zero)
            {
                boxPoints4[0] = (relativeForward + relativeRight) * 0.5f;
                boxPoints4[1] = boxPoints4[0] - relativeForward;
                boxPoints4[2] = boxPoints4[1] - relativeRight;
                boxPoints4[3] = boxPoints4[2] + relativeForward;

                polygonCollider2D.SetPath(0, boxPoints4);
            }
            else if (relativeForward == Vector2.zero)
            {
                boxPoints4[0] = (relativeRight + relativeUp) * 0.5f;
                boxPoints4[1] = boxPoints4[0] - relativeRight;
                boxPoints4[2] = boxPoints4[1] - relativeUp;
                boxPoints4[3] = boxPoints4[2] + relativeRight;

                polygonCollider2D.SetPath(0, boxPoints4);
            }
            else
            {
                // Get 4 corner points that aren't next to each other (see optimization below).
                boxPoints4[0] = (relativeRight + relativeUp + relativeForward) * 0.5f;
                boxPoints4[1] = boxPoints4[0] - relativeRight - relativeUp;
                boxPoints4[2] = boxPoints4[0] - relativeRight - relativeForward;
                boxPoints4[3] = boxPoints4[0] - relativeUp - relativeForward;

                // Find the corner point that overlaps the cutout (not on the edge).
                var minSqrMagnitude = boxPoints4[0].sqrMagnitude;
                var overlappingPointIndex = 0;
                for(int i = 1; i < 4; i++)
                {
                    if(boxPoints4[i].sqrMagnitude < minSqrMagnitude)
                    {
                        minSqrMagnitude = boxPoints4[i].sqrMagnitude;
                        overlappingPointIndex = i;
                    }
                }

                // Rearrange the corner points based on which index is overlapping. We always want our cutout to have point 0 to 1 move in the x, point 1 to 2 move in the y, and point 2 to 3 move in the z direction.
                // Since no corner points are next to each other, we can fill the other half of our cutout by flipping each value to the other side (e.g. boxPoints[5] = -boxPoints[2]).
                if(overlappingPointIndex < 2)
                {
                    boxPoints6[3] = -(boxPoints6[0] = boxPoints4[overlappingPointIndex + 2]);
                    if(overlappingPointIndex % 2 == 0)
                    {
                        boxPoints6[5] = -(boxPoints6[2] = boxPoints4[overlappingPointIndex + 3]);
                        boxPoints6[1] = -(boxPoints6[4] = boxPoints4[overlappingPointIndex + 1]);
                    }
                    else
                    {
                        boxPoints6[5] = -(boxPoints6[2] = boxPoints4[overlappingPointIndex + 1]);
                        boxPoints6[1] = -(boxPoints6[4] = boxPoints4[overlappingPointIndex - 1]);
                    }
                }
                else
                {
                    boxPoints6[3] = -(boxPoints6[0] = boxPoints4[overlappingPointIndex - 2]);
                    if(overlappingPointIndex % 2 == 0)
                    {
                        boxPoints6[5] = -(boxPoints6[2] = boxPoints4[overlappingPointIndex - 1]);
                        boxPoints6[1] = -(boxPoints6[4] = boxPoints4[overlappingPointIndex + 1]);
                    }
                    else
                    {
                        boxPoints6[5] = -(boxPoints6[2] = boxPoints4[overlappingPointIndex - 3]);
                        boxPoints6[1] = -(boxPoints6[4] = boxPoints4[overlappingPointIndex - 1]);
                    }
                }

                polygonCollider2D.SetPath(0, boxPoints6);
            }
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Box2D/*'/>
        public static void ToBoxCollider(this PolygonCollider2D polygonCollider2D, BoxCollider boxCollider)
        {
            polygonCollider2D.GenericPropertiesToCollider(boxCollider);

            boxCollider.isTrigger = polygonCollider2D.isTrigger;
            boxCollider.center = polygonCollider2D.offset.ToVector(polygonCollider2D, boxCollider, boxCollider.center);

            Vector2 relativeRight = boxCollider.transform.right;
            Vector2 relativeUp = boxCollider.transform.up;
            Vector2 relativeForward = boxCollider.transform.forward;

            if(relativeRight == Vector2.zero)
            {
                var newSize = boxCollider.size;
                newSize.z = Mathf.Sqrt((polygonCollider2D.points[0] - polygonCollider2D.points[1]).sqrMagnitude / relativeForward.sqrMagnitude);
                newSize.y = Mathf.Sqrt((polygonCollider2D.points[1] - polygonCollider2D.points[2]).sqrMagnitude / relativeUp.sqrMagnitude);
                boxCollider.size = newSize;
            }
            else if(relativeUp == Vector2.zero)
            {
                var newSize = boxCollider.size;
                newSize.z = Mathf.Sqrt((polygonCollider2D.points[0] - polygonCollider2D.points[1]).sqrMagnitude / relativeForward.sqrMagnitude);
                newSize.x = Mathf.Sqrt((polygonCollider2D.points[1] - polygonCollider2D.points[2]).sqrMagnitude / relativeRight.sqrMagnitude);
                boxCollider.size = newSize;
            }
            else if(relativeForward == Vector2.zero)
            {
                var newSize = boxCollider.size;
                newSize.x = Mathf.Sqrt((polygonCollider2D.points[0] - polygonCollider2D.points[1]).sqrMagnitude / relativeRight.sqrMagnitude);
                newSize.y = Mathf.Sqrt((polygonCollider2D.points[1] - polygonCollider2D.points[2]).sqrMagnitude / relativeUp.sqrMagnitude);
                boxCollider.size = newSize;
            }
            else
            {
                boxCollider.size = new Vector3(
                    Mathf.Sqrt((polygonCollider2D.points[0] - polygonCollider2D.points[1]).sqrMagnitude / relativeRight.sqrMagnitude),
                    Mathf.Sqrt((polygonCollider2D.points[1] - polygonCollider2D.points[2]).sqrMagnitude / relativeUp.sqrMagnitude),
                    Mathf.Sqrt((polygonCollider2D.points[2] - polygonCollider2D.points[3]).sqrMagnitude / relativeForward.sqrMagnitude));
            }
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Box2DSafe/*'/>
        public static void ToBoxColliderSafe(this PolygonCollider2D polygonCollider2D, BoxCollider boxCollider)
        {
            if(polygonCollider2D.IsBoxCollider(boxCollider.transform.rotation))
            {
                polygonCollider2D.ToBoxCollider(boxCollider);
            }
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Mesh/*'/>
        public static void ToPolygonCollider2D(this MeshCollider meshCollider, PolygonCollider2D polygonCollider2D) => meshCollider.ToPolygonCollider2D(polygonCollider2D, default);

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Mesh/*'/>
        public static void ToPolygonCollider2D(this MeshCollider meshCollider, PolygonCollider2D polygonCollider2D, Outliner outliner)
        {
            meshCollider.GenericPropertiesToCollider2D(polygonCollider2D);
            polygonCollider2D.isTrigger = meshCollider.convex && meshCollider.isTrigger;

            // Setup the renderer and camera for the render shot.
            if(!(renderFilter.sharedMesh = meshCollider.sharedMesh))
            {
                polygonCollider2D.pathCount = 0;
                polygonCollider2D.offset = Vector2.zero;
                return;
            }

            // Rotate the transform so the bounds get updated correctly.
            renderFilter.transform.rotation = Quaternion.Inverse(polygonCollider2D.transform.rotation) * meshCollider.transform.rotation;
            //renderFilter.transform.localScale = meshCollider.transform.localScale;
            var bounds = renderRenderer.bounds;
            if(bounds.size.x == 0f || bounds.size.y == 0f)
            {
                return;
            }

            // Position the transform so the bounds' center is at (0, 0, 0).
            renderFilter.transform.position -= bounds.center - Vector3.forward * bounds.extents.z;

            outliner = outliner ? outliner : defaultOutliner;

            // Set the pixel and camera size.
            int pixelWidth, pixelHeight;
            if(bounds.size.x > bounds.size.y)
            {
                pixelHeight = Mathf.CeilToInt((pixelWidth = outliner.resolution) * bounds.size.y / bounds.size.x);
                renderCamera.orthographicSize = bounds.extents.x * pixelWidth / pixelHeight;
            }
            else
            {
                pixelWidth = Mathf.CeilToInt((pixelHeight = outliner.resolution) * bounds.size.x / bounds.size.y);
                renderCamera.orthographicSize = bounds.extents.y;
            }
            renderCamera.farClipPlane = Mathf.Max(bounds.size.z, 0.01f); // We assume the nearClipPlane to be 0, otherwise we would need to do renderCamera.nearClipPlane + 0.01f.

            // Render the camera and read it to a Texture2D.
            var renderTexture = RenderTexture.GetTemporary(pixelWidth, pixelHeight, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.Color);
            var activeRenderTexture = RenderTexture.active;
            RenderTexture.active = renderCamera.targetTexture = renderTexture;

            renderCamera.gameObject.SetActive(true);
            renderCamera.Render();

            texture2D.Resize(pixelWidth, pixelHeight);
            rect.width = pixelWidth;
            rect.height = pixelHeight;
            texture2D.ReadPixels(rect, 0, 0, false);

            renderCamera.gameObject.SetActive(false);
            renderCamera.targetTexture = null;
            RenderTexture.active = activeRenderTexture;
            RenderTexture.ReleaseTemporary(renderTexture);

            // Generate the paths and apply them to the polygonCollider2D.
            var pixelsPerUnit = pixelHeight * 0.5f / renderCamera.orthographicSize;

            var boundaryTracer = new ContourTracer();
            boundaryTracer.Trace(texture2D, centerPivot, pixelsPerUnit, outliner.gapLength, outliner.product);

            polygonCollider2D.pathCount = boundaryTracer.pathCount;
            for(int i = 0; i < polygonCollider2D.pathCount; i++)
            {
                boundaryTracer.GetPath(i, ref path);
                LineUtility.Simplify(path, outliner.tolerance, points);
                if(points.Count < 3)
                {
                    polygonCollider2D.pathCount--;
                    i--;
                }
                else
                {
                    polygonCollider2D.SetPath(i, points);
                }
            }

            polygonCollider2D.offset = -renderFilter.transform.position;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Polygon2D/*'/>
        public static void ToMeshCollider(this PolygonCollider2D polygonCollider2D, MeshCollider meshCollider) => polygonCollider2D.ToMeshCollider(meshCollider, default);
        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Polygon2D/*'/>
        public static void ToMeshCollider(this PolygonCollider2D polygonCollider2D, MeshCollider meshCollider, PolygonCollider2DConversionOptions conversionOptions)
        {
            polygonCollider2D.GenericPropertiesToCollider(meshCollider);

            if(meshCollider.convex)
            {
                meshCollider.isTrigger = polygonCollider2D.isTrigger;
            }

            if(conversionOptions.HasFlag(PolygonCollider2DConversionOptions.DestroySharedMesh))
            {
#if UNITY_EDITOR
                if(Application.isPlaying)
                {
                    Object.Destroy(meshCollider.sharedMesh);
                }
                else
                {
                    Object.DestroyImmediate(meshCollider.sharedMesh);
                }
#else
                Object.Destroy(meshCollider.sharedMesh);
#endif
            }

            if(conversionOptions.HasFlag(PolygonCollider2DConversionOptions.CreateMesh))
            {
                // Copy the polygonCollider2D paths to the polygonCollider2D dummy.
                var pathCount = polygonColliderMeshCreator.pathCount = polygonCollider2D.pathCount;
                for(int i = 0; i < pathCount; i++)
                {
                    polygonCollider2D.GetPath(i, points);
                    polygonColliderMeshCreator.SetPath(i, points);
                }
                polygonColliderMeshCreator.offset = polygonCollider2D.offset;

                // Let the polygonCollider2D dummy create a mesh, free from translation, rotation and scale.
                polygonColliderMeshCreator.gameObject.SetActive(true);
                var polygonMesh = polygonColliderMeshCreator.CreateMesh(false, false);
                polygonColliderMeshCreator.gameObject.SetActive(false);

                if(meshCollider.transform.rotation != polygonCollider2D.transform.rotation)
                {
                    // Rotate the vertices of the mesh to the inverted position of the meshCollider's rotation so that all look forward (0, 0, 1).
                    var vertices = new List<Vector3>();
                    var relativeRotation = Quaternion.Inverse(meshCollider.transform.rotation) * polygonCollider2D.transform.rotation;

                    polygonMesh.GetVertices(vertices);
                    for(int i = 0; i < vertices.Count; i++)
                    {
                        vertices[i] = relativeRotation * vertices[i];
                    }
                    polygonMesh.SetVertices(vertices);
                    polygonMesh.RecalculateBounds();
                }

                if(conversionOptions.HasFlag(PolygonCollider2DConversionOptions.CreateBackfaces))
                {
                    // Create a duplicate of the triangles in reverse order in order to create backfacing collision for the collider.
                    var triangles = polygonMesh.triangles;
                    var trianglesLength = triangles.Length;

                    var newTriangles = new int[trianglesLength * 2];
                    triangles.CopyTo(newTriangles, 0);
                    triangles.CopyTo(newTriangles, trianglesLength);
                    System.Array.Reverse(newTriangles, trianglesLength, trianglesLength);

                    polygonMesh.SetTriangles(newTriangles, 0, false);
                }

                meshCollider.sharedMesh = polygonMesh;
            }
        }

        private static Vector2[] boxPoints6 = new Vector2[6];
        private static Vector2[] boxPoints4 = new Vector2[4];
        private static List<Vector2> points = new List<Vector2>();
        private static List<Vector2> path = new List<Vector2>();

        private static Outliner defaultOutliner;
        private static Texture2D texture2D;
        private static Rect rect = new Rect();
        private static readonly Vector2 centerPivot = new Vector2(0.5f, 0.5f);

        private static Camera renderCamera;
        private static MeshFilter renderFilter;
        private static MeshRenderer renderRenderer;

        private static PolygonCollider2D polygonColliderMeshCreator;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitColliderConversion()
        {
            var settings = Settings.GetSettings;

            defaultOutliner = settings.defaultOutliner ? settings.defaultOutliner : ScriptableObject.CreateInstance<Outliner>();
            texture2D = new Texture2D(0, 0, TextureFormat.R8, false);

            // Create renderCamera GameObject.
            var renderCameraGO = new GameObject(nameof(renderCamera));
            renderCameraGO.SetActive(false);
            Object.DontDestroyOnLoad(renderCameraGO);

            // Create rendering GameObject.
            var renderingGO = new GameObject(nameof(renderFilter));
            renderingGO.transform.SetParent(renderCameraGO.transform, false);
            renderingGO.layer = 31;

            // Create renderCamera.
            renderCamera = renderCameraGO.AddComponent<Camera>();

            renderCamera.clearFlags = CameraClearFlags.SolidColor;
            renderCamera.backgroundColor = Color.clear;
            renderCamera.cullingMask = 1 << 31;
            renderCamera.orthographic = true;
            renderCamera.nearClipPlane = 0f;
            renderCamera.renderingPath = RenderingPath.Forward;
            renderCamera.useOcclusionCulling =
                renderCamera.allowHDR =
                renderCamera.allowMSAA =
                renderCamera.allowDynamicResolution = false;

#if HDRP_7_0_OR_NEWER
            var hdData = renderCameraGO.AddComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData>();
            hdData.volumeLayerMask = 0;
            hdData.probeLayerMask = 0;
            hdData.clearColorMode = UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData.ClearColorMode.Color;
            hdData.backgroundColorHDR = Color.clear;
#endif

            // Create rendering Objects.
            renderFilter = renderingGO.AddComponent<MeshFilter>();

            renderRenderer = renderingGO.AddComponent<MeshRenderer>();
            renderRenderer.receiveShadows = false;
            renderRenderer.shadowCastingMode = ShadowCastingMode.Off;
            renderRenderer.lightProbeUsage = LightProbeUsage.Off;
            renderRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            renderRenderer.allowOcclusionWhenDynamic = false;

            // Create the polygonCollider2D dummy.
            var polygonColliderMeshCreatorGO = new GameObject(nameof(polygonColliderMeshCreator));
            polygonColliderMeshCreatorGO.SetActive(false);
            Object.DontDestroyOnLoad(polygonColliderMeshCreatorGO);
            polygonColliderMeshCreator = polygonColliderMeshCreatorGO.AddComponent<PolygonCollider2D>();
        }
        #endregion

        #region PhysicMaterial
        private static Dictionary<PhysicMaterial, PhysicsMaterial2D> physicMaterialPairs = new Dictionary<PhysicMaterial, PhysicsMaterial2D>();
        private static Dictionary<PhysicsMaterial2D, PhysicMaterial> physicsMaterial2DPairs = new Dictionary<PhysicsMaterial2D, PhysicMaterial>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitPhysicMaterialPairs()
        {
            physicMaterialPairs = new Dictionary<PhysicMaterial, PhysicsMaterial2D>();
            physicsMaterial2DPairs = new Dictionary<PhysicsMaterial2D, PhysicMaterial>();
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/PhysicMaterial/*'/>
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

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/PhysicsMaterial2D/*'/>
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
        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Rigidbody/*'/>
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

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/Rigidbody2D/*'/>
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

        public static RigidbodyConstraints2D ToRigidbodyConstraints2D(this RigidbodyConstraints constraints)
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

        public static RigidbodyConstraints ToRigidbodyConstraints(this RigidbodyConstraints2D constraints2D, RigidbodyConstraints constraints)
        {
            if(constraints2D.HasFlag(RigidbodyConstraints2D.FreezePositionX))
            {
                constraints |= RigidbodyConstraints.FreezePositionX;
            }
            else
            {
                constraints &= ~RigidbodyConstraints.FreezePositionX;
            }

            if(constraints2D.HasFlag(RigidbodyConstraints2D.FreezePositionY))
            {
                constraints |= RigidbodyConstraints.FreezePositionY;
            }
            else
            {
                constraints &= ~RigidbodyConstraints.FreezePositionY;
            }

            if(constraints2D.HasFlag(RigidbodyConstraints2D.FreezeRotation))
            {
                constraints |= RigidbodyConstraints.FreezeRotationZ;
            }
            else
            {
                constraints &= ~RigidbodyConstraints.FreezeRotationZ;
            }

            return constraints;
        }
        #endregion

        #region ConstantForce
        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/ConstantForce/*'/>
        public static void ToConstantForce2D(this ConstantForce constantForce, ConstantForce2D constantForce2D)
        {
            constantForce2D.force = constantForce.force;
            constantForce2D.relativeForce = constantForce.relativeForce.ToVector2D(constantForce, constantForce2D);

            var normalizedRelativeTorque = constantForce.transform.rotation * constantForce.relativeTorque;
            constantForce2D.torque = (constantForce.torque.z + normalizedRelativeTorque.z) * Mathf.Rad2Deg;
        }

        /// <include file='../Documentation.xml' path='docs/PhysicsConverter/ConstantForce2D/*'/>
        public static void ToConstantForce(this ConstantForce2D constantForce2D, ConstantForce constantForce)
        {
            Vector3 force = constantForce2D.force;
            force.z = constantForce.force.z;
            
            constantForce.force = force;
            constantForce.relativeForce = constantForce2D.relativeForce.ToVector(constantForce2D, constantForce, constantForce.relativeForce);

            var normalizedRelativeTorque = constantForce.transform.rotation * constantForce.relativeTorque;
            var weightedTorque2D = (Mathf.Abs(normalizedRelativeTorque.z) + Mathf.Abs(constantForce.torque.z)) * constantForce2D.torque;
            if (weightedTorque2D != 0f)
            {
                weightedTorque2D = 1f / weightedTorque2D * Mathf.Deg2Rad;
            }

            Vector3 torque = constantForce.torque;
            torque.z *= weightedTorque2D;
            constantForce.torque = torque;

            normalizedRelativeTorque.z *= weightedTorque2D;
            constantForce.relativeTorque = Quaternion.Inverse(constantForce.transform.rotation) * normalizedRelativeTorque;
        }
        #endregion
    }
}