using Physics2DxSystem.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Physics2DxSystem
{
    [AddComponentMenu(Physics2Dx.componentMenu + "Collider 2Dx")]
    [DisallowMultipleComponent]
    [Obsolete("This class is inheriting the deprecated Module2DxOld class.", false)]
    public sealed class Collider2Dx : Module2DxOld
    {
        private static readonly Quaternion zRotation90Deg = new Quaternion(0f, 0f, 0.7071068f, 0.7071068f);

        public override int conversionOrder => -100;
        public override uint batchSize => 256;

        #region Colliders Collections
        private Dictionary<Collider, Collider2D> colliderPairs = new Dictionary<Collider, Collider2D>();
        private Dictionary<Collider2D, Collider> collider2DPairs = new Dictionary<Collider2D, Collider>();

        // Add a Collider Collider2D pair to the dictionaries.
        private void AddPair(Collider collider, Collider2D collider2D)
        {
            colliderPairs.Add(collider, collider2D);
            collider2DPairs.Add(collider2D, collider);
            collidersChanged = collider2DsChanged = true;
        }

        // Remove a Collider Collider2D pair from the dictionaries.
        private void RemovePair(Collider collider, Collider2D collider2D)
        {
            colliderPairs.Remove(collider);
            collider2DPairs.Remove(collider2D);
            collidersChanged = collider2DsChanged = true;
        }

        private bool collidersChanged = false;
        private bool collider2DsChanged = false;

        private ReadOnlyCollection<Collider> _colliders = Array.AsReadOnly(Array.Empty<Collider>());
        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/colliders/*'/>
        public ReadOnlyCollection<Collider> colliders
        {
            get
            {
                if(collidersChanged)
                {
                    var array = new Collider[colliderPairs.Count];
                    colliderPairs.Keys.CopyTo(array, 0);

                    _colliders = Array.AsReadOnly(array);
                    collidersChanged = false;
                }

                return _colliders;
            }
        }

        private ReadOnlyCollection<Collider2D> _collider2Ds = Array.AsReadOnly(Array.Empty<Collider2D>());
        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/collider2Ds/*'/>
        public ReadOnlyCollection<Collider2D> collider2Ds
        {
            get
            {
                if(collider2DsChanged)
                {
                    var array = new Collider2D[collider2DPairs.Count];
                    collider2DPairs.Keys.CopyTo(array, 0);

                    _collider2Ds = Array.AsReadOnly(array);
                    collider2DsChanged = false;
                }

                return _collider2Ds;
            }
        }
        #endregion

#pragma warning disable CS0649
        [Tooltip("Automatically updates the collider caches by managing dynamically added and removed colliders. Turning this off improves performance on conversion but the caches won't be updated. Use the Component2Dx's methods to add and destroy colliders instead.")] public bool autoUpdate = false;
        [Tooltip("Determines which Capsule2D direction the Converter2Dx supports.")] public CapsuleDirection2D capsuleDirection2D;
        [Header("Conversion Settings")]
        public ConversionSettings conversionSettings = new ConversionSettings { renderSize = MeshColliderConversionRenderSize._256, tolerance = 0.05f };
        public Conversion2DSettings conversion2DSettings = new Conversion2DSettings { };
        [Header("Ignore Conversion")]
        [Tooltip("The Colliders to ignore for conversion. Bypassed by AddCollider.")] public Collider[] ignoreConversion;
        [Tooltip("The Collider2Ds to ignore for conversion. Bypassed by AddCollider.")] public Collider2D[] ignoreConversion2D;
        [Header("Caches")]
        [SerializeField] [Tooltip("The cache to store all the object's Colliders in.")] private Transform collidersCache;
        [SerializeField] [Tooltip("The cache to store all the object's Collider2Ds in.")] private Transform collider2DsCache;
#pragma warning restore CS0649

        private Vector3 upwardDirection
        {
            get
            {
                switch(capsuleDirection2D)
                {
                    case CapsuleDirection2D.Vertical:
                        return transform.up;
                    case CapsuleDirection2D.Horizontal:
                        return zRotation90Deg * transform.right;
                    default:
                        return default;
                }
            }
        }
        
        #region Validation
        private void OnValidate()
        {
            // Check if the cache transforms are valid.
            if(collidersCache && collidersCache.parent != transform)
            {
                collidersCache = null;
                Debug.LogError($"{nameof(collidersCache)} parent has to be the {nameof(Collider2Dx)}'s transform.");
            }
            if(collider2DsCache && collider2DsCache.parent != transform)
            {
                collider2DsCache = null;
                Debug.LogError($"{nameof(collider2DsCache)} parent has to be the {nameof(Collider2Dx)}'s transform.");
            }
            if(collidersCache && collider2DsCache && collidersCache == collider2DsCache)
            {
                collidersCache = null;
                collider2DsCache = null;
                Debug.LogError($"{nameof(collidersCache)} and {nameof(collider2DsCache)} can't be the same.");
            }
        }

        private IEnumerator OnTransformChildrenChanged()
        {
            yield return null;

            // Check if either caches still exist and are still parented to this object.
            if(!collidersCache || collidersCache.parent != transform)
            {
                throw new MissingReferenceException($"Required child {collidersCache.name} was removed from {name}.");
            }
            if(!collider2DsCache || collider2DsCache.parent != transform)
            {
                throw new MissingReferenceException($"Required child {collider2DsCache.name} was removed from {name}.");
            }
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();

            // Create the Colliders Cache if it is not created yet.
            if(!collidersCache)
            {
                collidersCache = new GameObject(nameof(collidersCache)).transform;
                collidersCache.SetParent(transform, false);
                collidersCache.gameObject.layer = gameObject.layer;
                collidersCache.tag = tag;
            }
            else
            {
                // Search the collidersCache for any unmapped Colliders and map them.
                foreach(var collider in collidersCache.GetComponents<Collider>())
                {
                    if(Array.IndexOf(ignoreConversion, collider) == -1)
                    {
                        if(conversion2DSettings.polygonCollider2DConversionOptions.HasFlag(PolygonCollider2DConversionOptions.DestroySharedMesh) && collider is MeshCollider meshCollider)
                        {
                            meshCollider.sharedMesh = Instantiate(meshCollider.sharedMesh);
                        }

                        var newCollider2D = (Collider2D)collider2DsCache.gameObject.AddComponent(Collider2DType(collider));
                        collider.ToCollider2D(newCollider2D, conversionSettings);
                        AddPair(collider, newCollider2D);
                    }
                }
            }

            // Create the Collider2Ds Cache if it is not created yet.
            if(!collider2DsCache)
            {
                collider2DsCache = new GameObject(nameof(collider2DsCache)).transform;
                collider2DsCache.SetParent(transform, false);
                Update2DTransform();
                collider2DsCache.gameObject.layer = gameObject.layer;
                collider2DsCache.tag = tag;
            }
            else
            {
                // Search the collider2DsCache for any unmapped Collider2Ds and map them.
                foreach(var collider2D in collider2DsCache.GetComponents<Collider2D>())
                {
                    if(!collider2DPairs.ContainsKey(collider2D) && Array.IndexOf(ignoreConversion2D, collider2D) == -1)
                    {
                        if(collider2D is BoxCollider2D boxCollider2D)
                        {
                            var polygonCollider2D = collider2DsCache.gameObject.AddComponent<PolygonCollider2D>();
                            boxCollider2D.ToPolygonCollider2D(polygonCollider2D);
                            DestroyImmediate(boxCollider2D);

                            var boxCollider = collidersCache.gameObject.AddComponent<BoxCollider>();
                            polygonCollider2D.ToBoxCollider(boxCollider);
                            AddPair(boxCollider, polygonCollider2D);
                        }
                        else
                        {
                            var newCollider = (Collider)collidersCache.gameObject.AddComponent(ColliderType(collider2D));
                            collider2D.ToCollider(newCollider, conversion2DSettings);
                            AddPair(newCollider, collider2D);
                        }
                    }
                }
            }

            // Add Collider2Ds from the Component2Dx to its caches and destroy them on the object itself.
            string[] _ignore = null;
            foreach(var collider2D in GetComponents<Collider2D>())
            {
                if(_ignore == null)
                {
                    _ignore = !collider2D.attachedRigidbody || !collider2D.attachedRigidbody.useAutoMass
                        ? new[] { "density", "usedByComposite" } : new[] { "usedByComposite" };
                }

                if(Array.IndexOf(ignoreConversion2D, collider2D) == -1)
                {
                    var ignore = collider2D is BoxCollider2D || collider2D is PolygonCollider2D
                    ? new ArraySegment<string>(_ignore, 0, _ignore.Length - 1).Array
                    : _ignore;

                    if(collider2D is BoxCollider2D boxCollider2D)
                    {
                        var polygonCollider2D = collider2DsCache.gameObject.AddComponent<PolygonCollider2D>();
                        boxCollider2D.ToPolygonCollider2D(polygonCollider2D);
                        DestroyImmediate(boxCollider2D);

                        var boxCollider = collidersCache.gameObject.AddComponent<BoxCollider>();
                        polygonCollider2D.ToBoxCollider(boxCollider);
                        AddPair(boxCollider, polygonCollider2D);
                    }
                    else
                    {
                        var newCollider2D = collider2DsCache.gameObject.AddComponent(collider2D, ignore);
                        var newCollider = (Collider)collidersCache.gameObject.AddComponent(ColliderType(newCollider2D));
                        newCollider2D.ToCollider(newCollider, conversion2DSettings);
                        AddPair(newCollider, newCollider2D);

                        DestroyImmediate(collider2D);
                    }
                }
            }

            // If there was any Collider2D on the object, it will be disallowed to have Colliders and we can skip searching for them.
            if(_ignore == null)
            {
                // Add Colliders from the Component2Dx to its caches and destroy them on the object itself.
                foreach(var collider in GetComponents<Collider>())
                {
                    if(Array.IndexOf(ignoreConversion, collider) == -1)
                    {
                        if(conversion2DSettings.polygonCollider2DConversionOptions.HasFlag(PolygonCollider2DConversionOptions.DestroySharedMesh) && collider is MeshCollider meshCollider)
                        {
                            meshCollider.sharedMesh = Instantiate(meshCollider.sharedMesh);
                        }

                        var newCollider = collidersCache.gameObject.AddComponent(collider);
                        var newCollider2D = (Collider2D)collider2DsCache.gameObject.AddComponent(Collider2DType(newCollider));
                        newCollider.ToCollider2D(newCollider2D, conversionSettings);
                        AddPair(newCollider, newCollider2D);

                        DestroyImmediate(collider);
                    }
                }
            }

            // Deactivate the cache that is currently unused.
            if(Physics2Dx.is2Dnot3D)
            {
                collidersCache.gameObject.SetActive(false);
            }
            else
            {
                collider2DsCache.gameObject.SetActive(false);
            }
        }

        // Make sure the Caches are always at the same position, (2D) rotation and scale as the parent transform.
        private void LateUpdate()
        {
            // TODO: Check to see if this gets called if the Collider2Dx transform changes or not.
            if(collidersCache.hasChanged && collidersCache.gameObject.activeInHierarchy)
            {
                collidersCache.SetPositionAndRotation(transform.position, transform.rotation);
                collidersCache.localScale = Vector3.one;
                collidersCache.hasChanged = false;
            }

            if(collider2DsCache.hasChanged && collider2DsCache.gameObject.activeInHierarchy)
            {
                var newRotation = Quaternion.LookRotation(Vector3.forward, upwardDirection);
                collider2DsCache.SetPositionAndRotation(transform.position, newRotation);
                collider2DsCache.localScale = Vector3.one;
                collider2DsCache.hasChanged = false;
            }
        }

        #region Collider Accessors
        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/AddCollider/*'/>
        public (T collider, Collider2D collider2D) AddCollider<T>() where T : Collider
        {
            var newCollider = collidersCache.gameObject.AddComponent<T>();
            var newCollider2D = (Collider2D)collider2DsCache.gameObject.AddComponent(Collider2DType(newCollider));
            AddPair(newCollider, newCollider2D);

            return (newCollider, newCollider2D);
        }

        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/AddCollider/*'/>
        public (T collider, Collider2D collider2D) AddCollider<T>(T toAdd, params string[] ignore) where T : Collider
        {
            var newCollider = (T)collidersCache.gameObject.AddComponent((Collider)toAdd, ignore);

            if(conversion2DSettings.polygonCollider2DConversionOptions.HasFlag(PolygonCollider2DConversionOptions.DestroySharedMesh) && newCollider is MeshCollider meshCollider)
            {
                meshCollider.sharedMesh = Instantiate(meshCollider.sharedMesh);
            }

            var newCollider2D = (Collider2D)collider2DsCache.gameObject.AddComponent(Collider2DType(newCollider));
            if(Physics2Dx.is2Dnot3D)
            {
                newCollider.ToCollider2D(newCollider2D, conversionSettings);
            }
            AddPair(newCollider, newCollider2D);

            return (newCollider, newCollider2D);
        }

        public Collider GetColliderAt(int index) => colliders.Count > index ? colliders[index] : null;
        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/DestroyCollider/*'/>
        public bool DestroyColliderAt(int index) => DestroyCollider(GetColliderAt(index));
        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/DestroyCollider/*'/>
        public bool DestroyCollider(Collider collider)
        {
            if(!collider || !colliderPairs.ContainsKey(collider))
            {
                return false;
            }

            var collider2D = colliderPairs[collider];
            RemovePair(collider, collider2D);

            Destroy(collider);
            Destroy(collider2D);

            return true;
        }

        // Get the associated Collider2D Type from a Collider.
        private Type Collider2DType(Collider collider)
        {
            switch(collider)
            {
                case SphereCollider _:
                    return typeof(CircleCollider2D);
                case CapsuleCollider _:
                    return typeof(CapsuleCollider2D);
                case BoxCollider _:
                case MeshCollider _:
                    return typeof(PolygonCollider2D);
                default:
                    return typeof(Collider2D);
            }
        }
        #endregion

        #region Collider2D Accessors
        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/AddCollider2D/*'/>
        public (Collider2D collider2D, Collider collider) AddCollider2D<T>() where T : Collider2D
        {
            if(typeof(T) == typeof(BoxCollider2D))
            {
                var polygonCollider2D = collider2DsCache.gameObject.AddComponent<PolygonCollider2D>();
                polygonCollider2D.CreateBoxCollider();
                var boxCollider = collidersCache.gameObject.AddComponent<BoxCollider>();

                AddPair(boxCollider, polygonCollider2D);
                return (polygonCollider2D, boxCollider);
            }
            else
            {
                var newCollider2D = collider2DsCache.gameObject.AddComponent<T>();
                var newCollider = (Collider)collidersCache.gameObject.AddComponent(ColliderType(newCollider2D));

                AddPair(newCollider, newCollider2D);
                return (newCollider2D, newCollider);
            }
        }

        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/AddCollider2D/*'/>
        public (Collider2D collider2D, Collider collider) AddCollider2D<T>(T toAdd, params string[] ignore) where T : Collider2D
        {
            var attachedRigidbody = collider2DsCache.GetComponentInParent<Rigidbody2D>();
            if(!attachedRigidbody || !attachedRigidbody.useAutoMass)
            {
                Array.Resize(ref ignore, ignore.Length + 1);
                ignore[ignore.Length - 1] = "density";
            }
            if(!(toAdd is BoxCollider2D) && !(toAdd is PolygonCollider2D))
            {
                Array.Resize(ref ignore, ignore.Length + 1);
                ignore[ignore.Length - 1] = "usedByComposite";
            }

            if(toAdd is BoxCollider2D boxCollider2D)
            {
                var polygonCollider2D = collider2DsCache.gameObject.AddComponent<PolygonCollider2D>();
                boxCollider2D.ToPolygonCollider2D(polygonCollider2D);
                var boxCollider = collidersCache.gameObject.AddComponent<BoxCollider>();

                if(!Physics2Dx.is2Dnot3D)
                {
                    polygonCollider2D.ToBoxCollider(boxCollider);
                }

                AddPair(boxCollider, polygonCollider2D);
                return (polygonCollider2D, boxCollider);
            }
            else
            {
                var newCollider2D = collider2DsCache.gameObject.AddComponent((Collider2D)toAdd, ignore);
                var newCollider = (Collider)collidersCache.gameObject.AddComponent(ColliderType(newCollider2D));

                if(!Physics2Dx.is2Dnot3D)
                {
                    newCollider2D.ToCollider(newCollider, conversion2DSettings);
                }

                AddPair(newCollider, newCollider2D);
                return (newCollider2D, newCollider);
            }
        }

        public Collider2D GetCollider2DAt(int index) => collider2Ds.Count > index ? collider2Ds[index] : null;
        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/DestroyCollider2D/*'/>
        public bool DestroyCollider2DAt(int index) => DestroyCollider2D(GetCollider2DAt(index));
        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/DestroyCollider2D/*'/>
        public bool DestroyCollider2D(Collider2D collider2D)
        {
            if(!collider2D || !collider2DPairs.ContainsKey(collider2D))
            {
                return false;
            }

            var collider = collider2DPairs[collider2D];
            RemovePair(collider, collider2D);

            Destroy(collider);
            Destroy(collider2D);

            return true;
        }

        // Get the associated Collider Type from a Collider2D.
        private Type ColliderType(Collider2D collider2D)
        {
            switch(collider2D)
            {
                case CircleCollider2D _:
                    return typeof(SphereCollider);
                case CapsuleCollider2D _:
                    return typeof(CapsuleCollider);
                case PolygonCollider2D polygonCollider2D:
                    return polygonCollider2D.IsBoxCollider(collidersCache.rotation) ? typeof(BoxCollider) : typeof(MeshCollider);
                default:
                    return typeof(Collider);
            }
        }
        #endregion

        #region To 2D
        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/Update2DTransform/*'/>
        [ContextMenu(nameof(Update2DTransform))]
        public void Update2DTransform()
        {
#if UNITY_EDITOR
            if(!Application.isPlaying)
            {
                if(!collider2DsCache)
                {
                    Debug.LogWarning($"There is no {nameof(collider2DsCache)} assigned.");
                    return;
                }
                UnityEditor.Undo.RecordObject(collider2DsCache, nameof(Update2DTransform));
            }
#endif
            collider2DsCache.rotation = Quaternion.LookRotation(Vector3.forward, upwardDirection);
            collider2DsCache.hasChanged = false;
        }

        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/UpdateMappedColliders/*'/>
        public void UpdateMappedColliders()
        {
            Collider2D collider2D;
            foreach(var collider in colliders)
            {
                collider2D = colliderPairs[collider];

                // If the key is missing, remove it and destroy the associated value.
                if(!collider)
                {
                    RemovePair(collider, collider2D);
                    Destroy(collider2D);
                }
                // If the value is missing, create a new one and assign it to the key.
                else if(!collider2D)
                {
                    collider2DPairs.Remove(collider2D);

                    var newCollider2D = (Collider2D)collider2DsCache.gameObject.AddComponent(Collider2DType(collider));
                    collider2DPairs.Add(newCollider2D, collider);
                    colliderPairs[collider] = newCollider2D;
                    collider2DsChanged = true;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/CacheColliders/*'/>
        public void CacheColliders()
        {
            // Search the collidersCache for any unmapped Colliders and map them.
            foreach(var collider in collidersCache.GetComponents<Collider>())
            {
                if(!colliderPairs.ContainsKey(collider) && Array.IndexOf(ignoreConversion, collider) == -1)
                {
                    if(conversion2DSettings.polygonCollider2DConversionOptions.HasFlag(PolygonCollider2DConversionOptions.DestroySharedMesh) && collider is MeshCollider meshCollider)
                    {
                        meshCollider.sharedMesh = Instantiate(meshCollider.sharedMesh);
                    }

                    var newCollider2D = (Collider2D)collider2DsCache.gameObject.AddComponent(Collider2DType(collider));
                    if(Physics2Dx.is2Dnot3D)
                    {
                        collider.ToCollider2D(newCollider2D, conversionSettings);
                    }
                    AddPair(collider, newCollider2D);
                }
            }

            // Add Colliders from the Component2Dx to its caches and destroy them on the object itself.
            foreach(var collider in GetComponents<Collider>())
            {
                AddCollider(collider);
                Destroy(collider);
            }
        }

        public override void ConvertTo2D()
        {
            // Update the Collider2Dx to be ready to convert to 2D.
            if(transform.hasChanged)
            {
                Update2DTransform();
                transform.hasChanged = false;
            }

            if(autoUpdate)
            {
                UpdateMappedColliders();
                CacheColliders();
            }

            // Convert all Colliders to Collider2Ds.
            foreach(var colliderPair in colliderPairs)
            {
                colliderPair.Key.ToCollider2D(colliderPair.Value, conversionSettings);
            }

            collidersCache.gameObject.SetActive(false);
            collider2DsCache.gameObject.SetActive(true);
        }
        #endregion

        #region To 3D
        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/UpdateMappedCollider2Ds/*'/>
        public void UpdateMappedCollider2Ds()
        {
            Collider collider;
            foreach(var collider2D in collider2Ds)
            {
                collider = collider2DPairs[collider2D];

                // If the key is missing, remove it and destroy the associated value.
                if(!collider2D)
                {
                    RemovePair(collider, collider2D);
                    Destroy(collider);
                }
                // If the value is missing, create a new one and assign it to the key.
                else if(!collider)
                {
                    colliderPairs.Remove(collider);

                    var newCollider = (Collider)collidersCache.gameObject.AddComponent(ColliderType(collider2D));
                    colliderPairs.Add(newCollider, collider2D);
                    collider2DPairs[collider2D] = newCollider;
                    collidersChanged = true;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/Modules/Collider2Dx/CacheCollider2Ds/*'/>
        public void CacheCollider2Ds()
        {
            // Search the collider2DsCache for any unmapped Collider2Ds and map them.
            foreach(var collider2D in collider2DsCache.GetComponents<Collider2D>())
            {
                if(!collider2DPairs.ContainsKey(collider2D) && Array.IndexOf(ignoreConversion2D, collider2D) == -1)
                {
                    if(collider2D is BoxCollider2D boxCollider2D)
                    {
                        var polygonCollider2D = collider2DsCache.gameObject.AddComponent<PolygonCollider2D>();
                        boxCollider2D.ToPolygonCollider2D(polygonCollider2D);
                        DestroyImmediate(boxCollider2D);

                        var boxCollider = collidersCache.gameObject.AddComponent<BoxCollider>();
                        if(!Physics2Dx.is2Dnot3D)
                        {
                            polygonCollider2D.ToBoxCollider(boxCollider);
                        }
                        AddPair(boxCollider, polygonCollider2D);
                    }
                    else
                    {
                        var newCollider = (Collider)collidersCache.gameObject.AddComponent(ColliderType(collider2D));
                        if(!Physics2Dx.is2Dnot3D)
                        {
                            collider2D.ToCollider(newCollider, conversion2DSettings);
                        }
                        AddPair(newCollider, collider2D);
                    }
                }
            }

            // Add Collider2Ds from the Component2Dx to its caches and destroy them on the object itself.
            foreach(var collider2D in GetComponents<Collider2D>())
            {
                AddCollider2D(collider2D);
                Destroy(collider2D);
            }
        }

        public override void ConvertTo3D()
        {
            // Update the Collider2Dx to be ready to convert to 3D.
            if(autoUpdate)
            {
                UpdateMappedCollider2Ds();
                CacheCollider2Ds();
            }

            // Convert all Collider2Ds to Colliders.
            foreach(var collider2DPair in collider2DPairs)
            {
                collider2DPair.Key.ToCollider(collider2DPair.Value, conversion2DSettings);
            }

            collider2DsCache.gameObject.SetActive(false);
            collidersCache.gameObject.SetActive(true);
        }
        #endregion
    }
}
