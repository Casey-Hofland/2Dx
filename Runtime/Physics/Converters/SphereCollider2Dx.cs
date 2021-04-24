using UnityEngine;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Sphere Collider 2Dx")]
    [DisallowMultipleComponent]
    public sealed class SphereCollider2Dx : ColliderConverter<SphereCollider, CircleCollider2D>
    {
        protected override void ColliderToCollider(SphereCollider collider, SphereCollider other)
        {
            collider.ToSphereCollider(other);
        }

        protected override void Collider2DToCollider2D(CircleCollider2D collider2D, CircleCollider2D other)
        {
            collider2D.ToCircleCollider2D(other);
        }

        protected override void ColliderToCollider2D(SphereCollider collider, CircleCollider2D collider2D)
        {
            collider.ToCircleCollider2D(collider2D);
        }

        protected override void Collider2DToCollider(CircleCollider2D collider2D, SphereCollider collider)
        {
            collider2D.ToSphereCollider(collider);
        }
    }
}
