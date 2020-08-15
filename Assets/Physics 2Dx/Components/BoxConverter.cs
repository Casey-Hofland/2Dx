using Physics2DxSystem.Utilities;
using System;
using UnityEngine;

namespace Physics2DxSystem
{
    [AddComponentMenu(Physics2Dx.componentMenu + "Box Converter")]
    [DisallowMultipleComponent]
    public sealed class BoxConverter : ColliderModule2Dx<BoxCollider, PolygonCollider2D>
    {
        private void OnValidate()
        {
            Debug.LogWarning($"Note that BoxCollider2Dx is still untested!");
        }

        [Tooltip("The BoxCollider2Ds to ignore for conversion.")] public BoxCollider2D[] ignoredBoxCollider2Ds;

        [Header("Convert to 3D")]
        [Tooltip("If enabled, PolygonCollider2D to BoxCollider will first go through a safety check to make sure the PolygonCollider2D is in an appropriate BoxCollider shape.")] public bool toBoxColliderSafe = true;

        public bool IgnoreBoxCollider2D(BoxCollider2D boxCollider2D) => Array.IndexOf(ignoredBoxCollider2Ds, boxCollider2D) != -1;

        public PolygonCollider2D AddBoxCollider2D()
        {
            var polygonCollider2D = AddCollider2D();
            polygonCollider2D.CreateBoxCollider();

            return polygonCollider2D;
        }

        public PolygonCollider2D AddBoxCollider2D(BoxCollider2D toAdd)
        {
            var polygonCollider2D = AddCollider2D();

            toAdd.ToPolygonCollider2D(polygonCollider2D);
            if(!Physics2Dx.is2Dnot3D)
            {
                var collider = GetColliderAt(collidersCount - 1);
                Collider2DToCollider(polygonCollider2D, collider);
            }

            return polygonCollider2D;
        }

        public override void CacheCollider2Ds()
        {
            foreach(var boxCollider2D in transform2Dx.gameObject2D.GetComponents<BoxCollider2D>())
            {
                if(!IgnoreBoxCollider2D(boxCollider2D))
                {
                    AddBoxCollider2D(boxCollider2D);
                    DestroyImmediate(boxCollider2D);
                }
            }

            foreach(var boxCollider2D in GetComponents<BoxCollider2D>())
            {
                if(!IgnoreBoxCollider2D(boxCollider2D))
                {
                    AddBoxCollider2D(boxCollider2D);
                    DestroyImmediate(boxCollider2D);
                }
            }

            base.CacheCollider2Ds();
        }

        protected override void ColliderToCollider(BoxCollider collider, BoxCollider other)
        {
            collider.ToBoxCollider(other);
        }

        protected override void Collider2DToCollider2D(PolygonCollider2D collider2D, PolygonCollider2D other)
        {
            collider2D.ToPolygonCollider2D(other);
        }

        protected override void ColliderToCollider2D(BoxCollider collider, PolygonCollider2D collider2D)
        {
            collider.ToPolygonCollider2D(collider2D);
        }

        protected override void Collider2DToCollider(PolygonCollider2D collider2D, BoxCollider collider)
        {
            if(toBoxColliderSafe)
            {
                collider2D.ToBoxColliderSafe(collider);
            }
            else 
            {
                collider2D.ToBoxCollider(collider);
            }
        }
    }
}
