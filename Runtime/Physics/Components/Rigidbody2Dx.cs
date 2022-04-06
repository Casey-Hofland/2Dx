#nullable enable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Unity2Dx.Physics
{
    [DisallowMultipleComponent]
    public sealed class Rigidbody2Dx : CopyConvertible<Rigidbody, Rigidbody2D>
    {
        [field: Tooltip("When converting to 2D, ignore collisions of overlapping Collider2Ds until they aren't overlapping anymore.")]
        [field: SerializeField]
        public bool ignoreOverlap { get; set; }

        protected override void ComponentToComponent(Rigidbody component, Rigidbody other)
        {
            component.ToRigidbody(other);
        }

        protected override void Component2DToComponent2D(Rigidbody2D component2D, Rigidbody2D other)
        {
            component2D.ToRigidbody2D(other);
        }

        protected override void ComponentToComponent2D(Rigidbody component, Rigidbody2D component2D)
        {
            component.ToRigidbody2D(component2D);
        }

        protected override void Component2DToComponent(Rigidbody2D component2D, Rigidbody component)
        {
            component2D.ToRigidbody(component);
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
        private List<Collider2D> attachedColliders = new List<Collider2D>();
        private List<Collider2D> overlapColliders = new List<Collider2D>();
        private List<Collider2D> trackedColliders = new List<Collider2D>();

        public ReadOnlyCollection<Collider2D> ignoredColliders => trackedColliders.AsReadOnly();

        /// <include file='../Documentation.xml' path='docs/Rigidbody2Dx/IgnoreOverlap/*' />
        public void IgnoreOverlap()
        {
            if (component2Ds.Count == 0 || !component2Ds[0])
            {
                return;
            }
            var rigidbody2D = component2Ds[0];

            if (rigidbody2D.attachedColliderCount == 0)
            {
                return;
            }

            var trackedColliderCount = rigidbody2D.OverlapCollider(default, trackedColliders);
            if (trackedColliderCount == 0)
            {
                return;
            }

            var attachedColliderCount = rigidbody2D.GetAttachedColliders(attachedColliders);
            var ignoreOverlapDistance = Physics2D.defaultContactOffset * -2f - Vector2.kEpsilon;
            for (int i = 0; i < trackedColliderCount; i++)
            {
                var trackedCollider = trackedColliders[i];

                for (int i2 = 0; i2 < attachedColliderCount; i2++)
                {
                    var attachedCollider = attachedColliders[i2];

                    var colliderDistance = attachedCollider.Distance(trackedCollider);
                    if (colliderDistance.distance <= ignoreOverlapDistance)
                    {
                        for (int i3 = 0; i3 < attachedColliderCount; i3++)
                        {
                            attachedCollider = attachedColliders[i3];

                            Physics2D.IgnoreCollision(trackedCollider, attachedCollider);
                        }
                        break;
                    }
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/Rigidbody2Dx/ClearOverlap/*' />
        public void ClearOverlap()
        {
            for (int i = 0; i < trackedColliders.Count; i++)
            {
                var trackedCollider = trackedColliders[i];

                for (int i2 = 0; i2 < attachedColliders.Count; i2++)
                {
                    var attachedCollider = attachedColliders[i2];

                    Physics2D.IgnoreCollision(trackedCollider, attachedCollider, false);
                }
            }

            attachedColliders.Clear();
            overlapColliders.Clear();
            trackedColliders.Clear();
        }

        /// <include file='../Documentation.xml' path='docs/Rigidbody2Dx/UpdateOverlap/*' />
        public void UpdateOverlap()
        {
            if (component2Ds.Count == 0 || !component2Ds[0])
            {
                return;
            }
            var rigidbody2D = component2Ds[0];

            var trackedCollidersCount = trackedColliders.Count;
            if (trackedCollidersCount > 0)
            {
                Collider2D trackedCollider;
                int attachedCollidersCount = 0;
                for (int i = trackedCollidersCount - 1; i >= 0; i--)
                {
                    trackedCollider = trackedColliders[i];
                    if (!trackedCollider)
                    {
                        trackedColliders.RemoveAt(i);
                        continue;
                    }

                    if (attachedCollidersCount == 0)
                    {
                        attachedColliders.RemoveAll(collider => !collider);
                        if ((attachedCollidersCount = attachedColliders.Count) == 0)
                        {
                            ClearOverlap();
                            break;
                        }
                        rigidbody2D.OverlapCollider(default, overlapColliders);
                    }

                    if (!overlapColliders.Contains(trackedCollider))
                    {
                        for (int a = attachedCollidersCount - 1; a >= 0; a--)
                        {
                            Physics2D.IgnoreCollision(attachedColliders[a], trackedCollider, false);
                            trackedColliders.RemoveAt(i);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
