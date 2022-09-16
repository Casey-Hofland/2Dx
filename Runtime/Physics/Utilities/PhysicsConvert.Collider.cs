#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsConvert
    {
        private static void GenericPropertiesToCollider2D(this Collider collider, Collider2D collider2D)
        {
            collider2D.enabled = collider.enabled;

            collider2D.sharedMaterial = collider.sharedMaterial.AsPhysicsMaterial2D();
        }

        private static void GenericPropertiesToCollider(this Collider2D collider2D, Collider collider)
        {
            collider.enabled = collider2D.enabled;

            collider.sharedMaterial = collider2D.sharedMaterial.AsPhysicMaterial();
        }

        #region SphereCollider & CircleCollider2D
        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Sphere/*'/>
        public static void ToCircleCollider2D(this SphereCollider sphereCollider, CircleCollider2D circleCollider2D)
        {
            sphereCollider.GenericPropertiesToCollider2D(circleCollider2D);

            // Set the CircleCollider2D settings.
            circleCollider2D.isTrigger = sphereCollider.isTrigger;
            circleCollider2D.offset = sphereCollider.center.ToVector2D(sphereCollider, circleCollider2D);
            circleCollider2D.radius = sphereCollider.radius;
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Circle2D/*'/>
        public static void ToSphereCollider(this CircleCollider2D circleCollider2D, SphereCollider sphereCollider)
        {
            circleCollider2D.GenericPropertiesToCollider(sphereCollider);

            // Set the SphereCollider settings.
            sphereCollider.isTrigger = circleCollider2D.isTrigger;
            sphereCollider.center = circleCollider2D.offset.ToVector(circleCollider2D, sphereCollider, sphereCollider.center);
            sphereCollider.radius = circleCollider2D.radius;
        }
        #endregion

        #region CapsuleCollider & CapsuleCollider2D
        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Capsule/*'/>
        public static void ToCapsuleCollider2D(this CapsuleCollider capsuleCollider, CapsuleCollider2D capsuleCollider2D)
        {
            capsuleCollider.GenericPropertiesToCollider2D(capsuleCollider2D);

            // Set the CapsuleCollider2D settings.
            capsuleCollider2D.isTrigger = capsuleCollider.isTrigger;
            capsuleCollider2D.offset = capsuleCollider.center.ToVector2D(capsuleCollider, capsuleCollider2D);

            // Determine if and in what direction to convert the CapsuleCollider2D size and direction.
            Vector3 direction;
            switch (capsuleCollider.direction)
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
            switch (capsuleCollider2D.direction)
            {
                case CapsuleDirection2D.Vertical:
                    capsuleCollider2D.size = new Vector2(diameter, diameter + distance);
                    break;
                case CapsuleDirection2D.Horizontal:
                    capsuleCollider2D.size = new Vector2(diameter + distance, diameter);
                    break;
            }
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Capsule2D/*'/>
        public static void ToCapsuleCollider(this CapsuleCollider2D capsuleCollider2D, CapsuleCollider capsuleCollider)
        {
            capsuleCollider2D.GenericPropertiesToCollider(capsuleCollider);

            // Set the CapsuleCollider settings.
            capsuleCollider.isTrigger = capsuleCollider2D.isTrigger;
            capsuleCollider.center = capsuleCollider2D.offset.ToVector(capsuleCollider2D, capsuleCollider, capsuleCollider.center);

            // Determine if the CapsuleCollider can be converted to.
            if (capsuleCollider.direction == 2)
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
            switch (capsuleCollider2D.direction)
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
            if (!Mathf.Approximately(angle, 90f))
            {
                var opposite = Mathf.Tan(angle * Mathf.Deg2Rad) * distance2D;
                height += Mathf.Sqrt(distance2D * distance2D + opposite * opposite);
            }

            capsuleCollider.radius = diameter * 0.5f;
            capsuleCollider.height = height;
        }
        #endregion

        #region BoxCollider & PolygonCollider2D
        private static Vector2[] boxPoints6 = new Vector2[6];
        private static Vector2[] boxPoints4 = new Vector2[4];

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Box/*'/>
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
                for (int i = 1; i < 4; i++)
                {
                    if (boxPoints4[i].sqrMagnitude < minSqrMagnitude)
                    {
                        minSqrMagnitude = boxPoints4[i].sqrMagnitude;
                        overlappingPointIndex = i;
                    }
                }

                // Rearrange the corner points based on which index is overlapping. We always want our cutout to have point 0 to 1 move in the x, point 1 to 2 move in the y, and point 2 to 3 move in the z direction.
                // Since no corner points are next to each other, we can fill the other half of our cutout by flipping each value to the other side (e.g. boxPoints[5] = -boxPoints[2]).
                if (overlappingPointIndex < 2)
                {
                    boxPoints6[3] = -(boxPoints6[0] = boxPoints4[overlappingPointIndex + 2]);
                    if (overlappingPointIndex % 2 == 0)
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
                    if (overlappingPointIndex % 2 == 0)
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

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Box2D/*'/>
        public static void ToBoxCollider(this PolygonCollider2D polygonCollider2D, BoxCollider boxCollider)
        {
            polygonCollider2D.GenericPropertiesToCollider(boxCollider);

            boxCollider.isTrigger = polygonCollider2D.isTrigger;
            boxCollider.center = polygonCollider2D.offset.ToVector(polygonCollider2D, boxCollider, boxCollider.center);

            Vector2 relativeRight = boxCollider.transform.right;
            Vector2 relativeUp = boxCollider.transform.up;
            Vector2 relativeForward = boxCollider.transform.forward;

            if (relativeRight == Vector2.zero)
            {
                var newSize = boxCollider.size;
                newSize.z = Mathf.Sqrt((polygonCollider2D.points[0] - polygonCollider2D.points[1]).sqrMagnitude / relativeForward.sqrMagnitude);
                newSize.y = Mathf.Sqrt((polygonCollider2D.points[1] - polygonCollider2D.points[2]).sqrMagnitude / relativeUp.sqrMagnitude);
                boxCollider.size = newSize;
            }
            else if (relativeUp == Vector2.zero)
            {
                var newSize = boxCollider.size;
                newSize.z = Mathf.Sqrt((polygonCollider2D.points[0] - polygonCollider2D.points[1]).sqrMagnitude / relativeForward.sqrMagnitude);
                newSize.x = Mathf.Sqrt((polygonCollider2D.points[1] - polygonCollider2D.points[2]).sqrMagnitude / relativeRight.sqrMagnitude);
                boxCollider.size = newSize;
            }
            else if (relativeForward == Vector2.zero)
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

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Box2DSafe/*'/>
        public static void ToBoxColliderSafe(this PolygonCollider2D polygonCollider2D, BoxCollider boxCollider)
        {
            if (polygonCollider2D.IsBoxCollider(/*boxCollider.transform.rotation*/))
            {
                polygonCollider2D.ToBoxCollider(boxCollider);
            }
        }
        #endregion

        #region MeshCollider & PolygonCollider2D
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterOutliner()
        {
            points = new();
            path = new();

            defaultOutliner = ScriptableObject.CreateInstance<Outliner>();
            rect = new();
        }

        private static List<Vector2> points = new();
        private static List<Vector2> path = new();

        private static Outliner defaultOutliner = ScriptableObject.CreateInstance<Outliner>();
        private static Rect rect = new();
        private static readonly Vector2 centerPivot = new(0.5f, 0.5f);

        private static Camera? _renderCamera;
        private static Camera renderCamera
        {
            get
            {
                if (_renderCamera == null)
                {
                    var renderCameraGO = CreateDummy(nameof(renderCamera));

                    // Create renderCamera and set values.
                    _renderCamera = renderCameraGO.AddComponent<Camera>();

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
                }

                return _renderCamera;
            }
        }

        private static GameObject? _renderingGO;
        private static GameObject renderingGO
        {
            get
            {
                if (_renderingGO == null)
                {
                    _renderingGO = new GameObject(nameof(renderingGO));
                    _renderingGO.transform.SetParent(renderCamera.transform, false);
                    _renderingGO.layer = 31;
                }

                return _renderingGO;
            }
        }

        private static MeshFilter? _renderFilter;
        private static MeshFilter renderFilter
        {
            get
            {
                if (_renderFilter == null)
                {
                    _renderFilter = renderingGO.AddComponent<MeshFilter>();
                }

                return _renderFilter;
            }
        }

        private static MeshRenderer? _renderRenderer;
        private static MeshRenderer renderRenderer
        {
            get
            {
                if (_renderRenderer == null)
                {
                    _renderRenderer = renderingGO.AddComponent<MeshRenderer>();

                    _renderRenderer.receiveShadows = false;
                    _renderRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    _renderRenderer.lightProbeUsage = LightProbeUsage.Off;
                    _renderRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                    _renderRenderer.allowOcclusionWhenDynamic = false;
                }
                
                return _renderRenderer;
            }
        }

        private static PolygonCollider2D? _polygonColliderMeshCreator;
        private static PolygonCollider2D polygonColliderMeshCreator
        {
            get
            {
                if (_polygonColliderMeshCreator == null)
                {
                    var polygonColliderMeshCreatorGO = CreateDummy(nameof(polygonColliderMeshCreator));
                    _polygonColliderMeshCreator = polygonColliderMeshCreatorGO.AddComponent<PolygonCollider2D>();
                }

                return _polygonColliderMeshCreator;
            }
        }

        private static GameObject CreateDummy(string name)
        {
            var go = new GameObject(name);
            go.SetActive(false);

#if UNITY_EDITOR
            if (!Application.IsPlaying(go))
            {
                UnityEditor.EditorApplication.delayCall += () => Object.DestroyImmediate(go);
            }
            else
#endif
            {
                Object.DontDestroyOnLoad(go);
            }

            return go;
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Mesh/*'/>
        public static void ToPolygonCollider2D(this MeshCollider meshCollider, PolygonCollider2D polygonCollider2D) => meshCollider.ToPolygonCollider2D(polygonCollider2D, default);

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Mesh/*'/>
        public static void ToPolygonCollider2D(this MeshCollider meshCollider, PolygonCollider2D polygonCollider2D, Outliner? outliner)
        {
            meshCollider.GenericPropertiesToCollider2D(polygonCollider2D);
            polygonCollider2D.isTrigger = meshCollider.convex && meshCollider.isTrigger;

            // Setup the renderer and camera for the render shot.
            if (!(renderFilter.sharedMesh = meshCollider.sharedMesh))
            {
                polygonCollider2D.pathCount = 0;
                polygonCollider2D.offset = Vector2.zero;
                return;
            }

            // Rotate the transform so the bounds get updated correctly.
            renderFilter.transform.rotation = Quaternion.Inverse(polygonCollider2D.transform.rotation) * meshCollider.transform.rotation;
            //renderFilter.transform.localScale = meshCollider.transform.localScale;
            var bounds = renderRenderer.bounds;
            if (bounds.size.x == 0f || bounds.size.y == 0f)
            {
                return;
            }

            // Position the transform so the bounds' center is at (0, 0, 0).
            renderFilter.transform.position -= bounds.center - Vector3.forward * bounds.extents.z;

            outliner = outliner != null ? outliner : defaultOutliner;

            // Set the pixel and camera size.
            int pixelWidth, pixelHeight;
            if (bounds.size.x > bounds.size.y)
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
            //var renderTexture = RenderTexture.GetTemporary(pixelWidth, pixelHeight, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.Color);
            var renderTexture = RenderTexture.GetTemporary(pixelWidth, pixelHeight, 16, RenderTextureFormat.R8, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.Color | RenderTextureMemoryless.Depth);
            var activeRenderTexture = RenderTexture.active;
            RenderTexture.active = renderCamera.targetTexture = renderTexture;

            renderCamera.gameObject.SetActive(true);
            renderCamera.Render();

            var texture2D = new Texture2D(pixelWidth, pixelHeight, TextureFormat.R8, false);
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
            for (int i = 0; i < polygonCollider2D.pathCount; i++)
            {
                boundaryTracer.GetPath(i, ref path);
                LineUtility.Simplify(path, outliner.tolerance, points);
                if (points.Count < 3)
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

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Polygon2D/*'/>
        public static void ToMeshCollider(this PolygonCollider2D polygonCollider2D, MeshCollider meshCollider) => polygonCollider2D.ToMeshCollider(meshCollider, default);
        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/Polygon2D/*'/>
        public static void ToMeshCollider(this PolygonCollider2D polygonCollider2D, MeshCollider meshCollider, PolygonCollider2DConversionOptions conversionOptions)
        {
            polygonCollider2D.GenericPropertiesToCollider(meshCollider);

            if (meshCollider.convex)
            {
                meshCollider.isTrigger = polygonCollider2D.isTrigger;
            }

            if (conversionOptions.HasFlag(PolygonCollider2DConversionOptions.DestroySharedMesh))
            {
#if UNITY_EDITOR
                if (!Application.IsPlaying(meshCollider.sharedMesh))
                {
                    Object.DestroyImmediate(meshCollider.sharedMesh);
                }
                else
#endif
                {
                    Object.Destroy(meshCollider.sharedMesh);
                }
            }

            if (conversionOptions.HasFlag(PolygonCollider2DConversionOptions.CreateMesh))
            {
                // Copy the polygonCollider2D paths to the polygonCollider2D dummy.
                var pathCount = polygonColliderMeshCreator.pathCount = polygonCollider2D.pathCount;
                for (int i = 0; i < pathCount; i++)
                {
                    polygonCollider2D.GetPath(i, points);
                    polygonColliderMeshCreator.SetPath(i, points);
                }
                polygonColliderMeshCreator.offset = polygonCollider2D.offset;

                // Let the polygonCollider2D dummy create a mesh, free from translation, rotation and scale.
                polygonColliderMeshCreator.gameObject.SetActive(true);
                var polygonMesh = polygonColliderMeshCreator.CreateMesh(false, false);
                polygonColliderMeshCreator.gameObject.SetActive(false);

                if (meshCollider.transform.rotation != polygonCollider2D.transform.rotation)
                {
                    // Rotate the vertices of the mesh to the inverted position of the meshCollider's rotation so that all look forward (0, 0, 1).
                    var vertices = new List<Vector3>();
                    var relativeRotation = Quaternion.Inverse(meshCollider.transform.rotation) * polygonCollider2D.transform.rotation;

                    polygonMesh.GetVertices(vertices);
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        vertices[i] = relativeRotation * vertices[i];
                    }
                    polygonMesh.SetVertices(vertices);
                    polygonMesh.RecalculateBounds();
                }

                if (conversionOptions.HasFlag(PolygonCollider2DConversionOptions.CreateBackfaces))
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
        #endregion
    }
}
