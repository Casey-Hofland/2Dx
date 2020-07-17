using Physics2DxSystem.Utilities;
using UnityEngine;

namespace Physics2DxSystem
{
    [AddComponentMenu(Physics2Dx.componentMenu + "Rigidbody 2Dx")]
    [DisallowMultipleComponent]
    public sealed class Rigidbody2Dx : Module2Dx
    {
        public override int conversionOrder => -50;
        public override uint batchSize => 64;

        private Rigidbody rigidbodyCache;
        private Rigidbody2D rigidbody2DCache;

        protected override void Awake()
        {
            base.Awake();

            rigidbodyCache = new GameObject(nameof(rigidbodyCache)).AddComponent<Rigidbody>();
            rigidbodyCache.transform.SetParent(transform, false);
            rigidbodyCache.gameObject.SetActive(false);

            rigidbody2DCache = new GameObject(nameof(rigidbody2DCache)).AddComponent<Rigidbody2D>();
            rigidbody2DCache.transform.SetParent(transform, false);
            rigidbody2DCache.gameObject.SetActive(false);

            if(Physics2Dx.slimHierarchy)
            {
                rigidbodyCache.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                rigidbody2DCache.gameObject.hideFlags |= HideFlags.HideInHierarchy;
            }
        }

        public override void ConvertTo2D()
        {
            if(TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody2DCache.gameObject.SetActive(true);
                rigidbody.ToRigidbody2D(rigidbody2DCache);

                DestroyImmediate(rigidbody);
                gameObject.AddComponent(rigidbody2DCache);
                rigidbody2DCache.gameObject.SetActive(false);
            }
        }

        public override void ConvertTo3D()
        {
            if(TryGetComponent<Rigidbody2D>(out var rigidbody2D))
            {
                rigidbodyCache.gameObject.SetActive(true);
                rigidbody2D.ToRigidbody(rigidbodyCache);

                DestroyImmediate(rigidbody2D);
                gameObject.AddComponent(rigidbodyCache, "inertiaTensor");
                rigidbodyCache.gameObject.SetActive(false);
            }
        }
    }
}

