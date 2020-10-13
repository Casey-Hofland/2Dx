using DimensionConverter.Utilities;
using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Capsule Converter")]
    [DisallowMultipleComponent]
    public sealed class CapsuleConverter : ColliderConverter<CapsuleCollider, CapsuleCollider2D>
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
