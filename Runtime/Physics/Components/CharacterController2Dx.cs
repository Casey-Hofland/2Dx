#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityExtras;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Character Controller 2Dx")]
    [DisallowMultipleComponent]
    public sealed class CharacterController2Dx : CopyConvertible<CharacterController, CharacterController2D>
    {
        [field: SerializeField] public bool ignoreOverlap { get; set; }

        protected override void ComponentToComponent(CharacterController component, CharacterController other)
        {
            component.ToCharacterController(other);
        }

        protected override void Component2DToComponent2D(CharacterController2D component2D, CharacterController2D other)
        {
            component2D.ToCharacterController2D(other);
            DestroyImmediate(component2D.capsuleCollider2D);
        }

        protected override void ComponentToComponent2D(CharacterController component, CharacterController2D component2D)
        {
            component.ToCharacterController2D(component2D);
        }

        protected override void Component2DToComponent(CharacterController2D component2D, CharacterController component)
        {
            component2D.ToCharacterController(component);
        }

#if UNITY_2020_1_OR_NEWER
        private void FixedUpdate()
        {
            if (is2DNot3D && Physics2D.simulationMode == SimulationMode2D.FixedUpdate)
            {
                UpdateOverlap();
            }
        }

        private void Update()
        {
            if (is2DNot3D && Physics2D.simulationMode == SimulationMode2D.Update)
            {
                UpdateOverlap();
            }
        }
#else
        private void FixedUpdate()
        {
            if (is2DNot3D)
            {
                UpdateOverlap();
            }
        }
#endif

        protected override void OnConverted(bool convertTo2DNot3D)
        {
            base.OnConverted(convertTo2DNot3D);

            if (convertTo2DNot3D && ignoreOverlap)
            {
                IgnoreOverlap();
            }
        }

        #region Overlap
        private new Collider2D? collider;
        private List<Collider2D> overlapColliders = new List<Collider2D>();
        private List<Collider2D> trackedColliders = new List<Collider2D>();

        public void IgnoreOverlap()
        {
            if (component2Ds.Count == 0 || !component2Ds[0])
            {
                return;
            }
            collider = component2Ds[0].capsuleCollider2D;

            var trackedColliderCount = collider.OverlapCollider(default, trackedColliders);
            if (trackedColliderCount == 0)
            {
                return;
            }

            var ignoreOverlapDistance = Physics2D.defaultContactOffset * -2f - Vector2.kEpsilon;
            for (int i = 0; i < trackedColliderCount; i++)
            {
                var trackedCollider = trackedColliders[i];

                var colliderDistance = collider.Distance(trackedCollider);
                if (colliderDistance.distance <= ignoreOverlapDistance)
                {
                    Physics2D.IgnoreCollision(trackedCollider, collider);
                }
            }
        }

        public void ClearOverlap()
        {
            if (collider != null)
            {
                for (int i = 0; i < trackedColliders.Count; i++)
                {
                    var trackedCollider = trackedColliders[i];
                    Physics2D.IgnoreCollision(trackedCollider, collider, false);
                }
            }

            collider = null;
            overlapColliders.Clear();
            trackedColliders.Clear();
        }

        public void UpdateOverlap()
        {
            if (component2Ds.Count == 0 || !component2Ds[0])
            {
                return;
            }
            collider = component2Ds[0].capsuleCollider2D;

            collider.OverlapCollider(default, overlapColliders);

            var trackedCollidersCount = trackedColliders.Count;
            if (trackedCollidersCount > 0)
            {
                Collider2D trackedCollider;
                for (int i = trackedCollidersCount - 1; i >= 0; i--)
                {
                    trackedCollider = trackedColliders[i];
                    if (!trackedCollider)
                    {
                        trackedColliders.RemoveAt(i);
                        continue;
                    }

                    if (!overlapColliders.Contains(trackedCollider))
                    {
                        Physics2D.IgnoreCollision(collider, trackedCollider, false);
                        trackedColliders.RemoveAt(i);
                    }
                }
            }
        }
        #endregion
    }
}
