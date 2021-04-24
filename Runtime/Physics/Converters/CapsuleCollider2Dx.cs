using UnityEngine;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Capsule Collider 2Dx")]
    [DisallowMultipleComponent]
    public sealed class CapsuleCollider2Dx : ColliderConverter<CapsuleCollider, CapsuleCollider2D>
    {
        protected override void ColliderToCollider(CapsuleCollider collider, CapsuleCollider other)
        {
            collider.ToCapsuleCollider(other);
        }

        protected override void Collider2DToCollider2D(CapsuleCollider2D collider2D, CapsuleCollider2D other)
        {
            collider2D.ToCapsuleCollider2D(other);
        }

        protected override void ColliderToCollider2D(CapsuleCollider collider, CapsuleCollider2D collider2D)
        {
            collider.ToCapsuleCollider2D(collider2D);
        }

        protected override void Collider2DToCollider(CapsuleCollider2D collider2D, CapsuleCollider collider)
        {
            collider2D.ToCapsuleCollider(collider);
        }
    }
}
