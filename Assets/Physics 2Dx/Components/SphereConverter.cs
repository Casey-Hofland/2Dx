﻿using Physics2DxSystem.Utilities;
using UnityEngine;

namespace Physics2DxSystem
{
    [AddComponentMenu(Physics2Dx.componentMenu + "Sphere Converter")]
    [DisallowMultipleComponent]
    public sealed class SphereConverter : ColliderModule2Dx<SphereCollider, CircleCollider2D>
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
