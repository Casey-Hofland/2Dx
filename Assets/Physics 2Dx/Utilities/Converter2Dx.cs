using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
            rigidbody2D.mass = Mathf.Min(rigidbody.mass, 1000000f);
            rigidbody2D.sleepMode = RigidbodySleepMode2D.StartAwake;
            rigidbody2D.useAutoMass = false;
            rigidbody2D.velocity = rigidbody.velocity;

            rigidbody2D.constraints = rigidbody.constraints.ToRigidbodyConstraints2D();
        }

        /// <include file='../Documentation.xml' path='docs/Convert2dx/Rigidbody2D/*'/>
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

        private static RigidbodyConstraints2D ToRigidbodyConstraints2D(this RigidbodyConstraints constraints)
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

        private static RigidbodyConstraints ToRigidbodyConstraints(this RigidbodyConstraints2D constraints2D, RigidbodyConstraints constraints)
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
        public static void ToCollider2D(this Collider collider, Collider2D collider2D) => collider.ToCollider2D(collider2D, default);
        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Collider/*'/>
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

                    break;
                case MeshCollider meshCollider when collider2D is PolygonCollider2D polygonCollider2D:
                    meshCollider.ToPolygonCollider2D(polygonCollider2D, conversionSettings.renderSize);
                    break;
                default:
                    collider.CopyGenericProperties(collider2D);
                    break;
            }
        }

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Collider2D/*'/>
        public static void ToCollider(this Collider2D collider2D, Collider collider) => collider2D.ToCollider(collider, default);
        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Collider2D/*'/>
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

                    break;
                case PolygonCollider2D polygonCollider2D when collider is MeshCollider meshCollider:
                    polygonCollider2D.ToMeshCollider(meshCollider, conversion2DSettings.polygonCollider2DConversionMethod);
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

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Box/*'/>
        //public static void ToPolygonCollider2D(this BoxCollider boxCollider, PolygonCollider2D polygonCollider2D)
        //{
        //    boxCollider.CopyGenericProperties(polygonCollider2D);

        //    polygonCollider2D.offset = CenterToOffset(boxCollider.transform, boxCollider.center, polygonCollider2D.transform);
        //}

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Box2D/*'/>
        //public static void ToBoxCollider(this PolygonCollider2D polygonCollider2D, BoxCollider boxCollider)
        //{
        //    polygonCollider2D.CopyGenericProperties(boxCollider);

        //    boxCollider.center = OffsetToCenter(polygonCollider2D.transform, polygonCollider2D.offset, boxCollider.transform, boxCollider.center);
        //}

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Mesh/*'/>
        public static void ToPolygonCollider2D(this MeshCollider meshCollider, PolygonCollider2D polygonCollider2D) => meshCollider.ToPolygonCollider2D(polygonCollider2D, default);
        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Mesh/*'/>
        public static void ToPolygonCollider2D(this MeshCollider meshCollider, PolygonCollider2D polygonCollider2D, MeshColliderConversionRenderSize renderSize)
        {
            meshCollider.CopyGenericProperties(polygonCollider2D);

            // Setup the renderer and camera for the render shot.
            if(!(renderFilter.sharedMesh = meshCollider.sharedMesh))
            {
                return;
            }
            renderFilter.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Inverse(polygonCollider2D.transform.rotation) * meshCollider.transform.rotation);

            var bounds = renderRenderer.bounds;
            renderFilter.transform.position = -bounds.center + Vector3.forward * bounds.extents.z;

            renderCamera.orthographicSize =
                bounds.extents.x > bounds.extents.y
                ? bounds.extents.x * renderCamera.aspect
                : bounds.extents.y;
            if(Mathf.Approximately(renderCamera.orthographicSize, 0))
            {
                return;
            }
            renderCamera.farClipPlane = Mathf.Max(bounds.size.z, 0.01f); // We assume the nearClipPlane to be 0, otherwise we would need to do renderCamera.nearClipPlane + 0.01f.

            // Render the texture and read it to a Texture2D.
            var activeRenderTexture = RenderTexture.active;
            var renderTexture = RenderTexture.active = renderCamera.targetTexture = renderTextures[(int)renderSize];

            renderCamera.gameObject.SetActive(true);
            renderCamera.Render();

            var texture2D = new Texture2D(renderTexture.width, renderTexture.height);
            var rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            texture2D.ReadPixels(rect, 0, 0, false);

            renderFilter.sharedMesh = null;
            renderCamera.gameObject.SetActive(false);
            RenderTexture.active = activeRenderTexture;

            // Create a sprite out of the texture2D and let it generate a physics shape.
            var pixelsPerUnit = texture2D.width * 0.5f / renderCamera.orthographicSize; // We assume the texture2D is a square, otherwise we would need to do Mathf.Max(texture2D.width, texture2D.height).
            Vector4 border = new Vector4(rect.xMin, rect.yMin, rect.xMax, rect.yMax);
            var sprite = Sprite.Create(texture2D, rect, centerPivot, pixelsPerUnit, 0, SpriteMeshType.FullRect, border, true);

            // Give the generated physics shape to the polygonCollider2D and offset it by the amount the renderer bounds and renderer transform were offset times half (not sure why times half).
            if((polygonCollider2D.pathCount = sprite.GetPhysicsShapeCount()) > 0)
            {
                for(int i = 0; i < sprite.GetPhysicsShapeCount(); i++)
                {
                    sprite.GetPhysicsShape(i, points);
                    polygonCollider2D.SetPath(i, points);
                }
            }
            polygonCollider2D.offset = (bounds.center - renderFilter.transform.position) * 0.5f;
        }

        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Polygon2D/*'/>
        public static void ToMeshCollider(this PolygonCollider2D polygonCollider2D, MeshCollider meshCollider) => polygonCollider2D.ToMeshCollider(meshCollider, default);
        /// <include file='../Documentation.xml' path='docs/Converter2Dx/Polygon2D/*'/>
        public static void ToMeshCollider(this PolygonCollider2D polygonCollider2D, MeshCollider meshCollider, PolygonCollider2DConversionMethod conversionMethod)
        {
            polygonCollider2D.CopyGenericProperties(meshCollider);

            if(polygonCollider2D.isActiveAndEnabled)
            {
                switch(conversionMethod)
                {
                    case PolygonCollider2DConversionMethod.CreateMeshAndDestroySharedMesh:
                        Object.Destroy(meshCollider.sharedMesh);
                        goto case PolygonCollider2DConversionMethod.CreateMesh;
                    case PolygonCollider2DConversionMethod.CreateMesh:
                        if(!polygonCollider2D.attachedRigidbody)
                        {
                            var rigidbody2D = polygonCollider2D.gameObject.AddComponent<Rigidbody2D>();
                            meshCollider.sharedMesh = polygonCollider2D.CreateMesh(false, false);
                            Object.DestroyImmediate(rigidbody2D);
                        }
                        else
                        {
                            meshCollider.sharedMesh = polygonCollider2D.CreateMesh(false, false);
                        }

                        if(meshCollider.transform.rotation != polygonCollider2D.transform.rotation)
                        {
                            // Rotate Vertices to the inverted position of the meshCollider's rotation so that they become flat.
                            var vertices = new List<Vector3>();
                            var inverse = Quaternion.Inverse(meshCollider.transform.rotation) * polygonCollider2D.transform.rotation;
                            meshCollider.sharedMesh.GetVertices(vertices);
                            for(int i = 0; i < vertices.Count; i++)
                            {
                                vertices[i] = inverse * vertices[i];
                            }
                            meshCollider.sharedMesh.SetVertices(vertices); // TODO: Optimize with advanced mesh functions.
                            meshCollider.sharedMesh.RecalculateBounds();
                        }

                        break;
                }
            }
        }

        private static Camera renderCamera;
        private static RenderTexture[] renderTextures;
        private static MeshFilter renderFilter;
        private static MeshRenderer renderRenderer;
        private static List<Vector2> points;
        private static readonly Vector2 centerPivot = new Vector2(0.5f, 0.5f);
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitColliderConversion()
        {
            var settings = Resources.Load<Physics2DxSettings>(nameof(Physics2DxSettings));

            renderTextures = new RenderTexture[8]
            {
                new RenderTexture(32, 32, 0),
                new RenderTexture(64, 64, 0),
                new RenderTexture(256, 256, 0),
                new RenderTexture(512, 512, 0),
                new RenderTexture(1024, 1024, 0),
                new RenderTexture(2048, 2048, 0),
                new RenderTexture(4096, 4096, 0),
                new RenderTexture(8192, 8192, 0),
            };

            // Create renderCamera GameObject.
            var renderCameraGO = new GameObject(nameof(renderCamera));
            renderCameraGO.SetActive(false);
            Object.DontDestroyOnLoad(renderCameraGO);
            if(Physics2Dx.slimHierarchy)
            {
                renderCameraGO.hideFlags |= HideFlags.HideInHierarchy;
            }

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

            // Create rendering Objects.
            renderFilter = renderingGO.AddComponent<MeshFilter>();

            renderRenderer = renderingGO.AddComponent<MeshRenderer>();
            renderRenderer.receiveShadows = false;
            renderRenderer.shadowCastingMode = ShadowCastingMode.Off;

            points = new List<Vector2>();
        }
        #endregion
    }
}

