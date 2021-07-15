using UnityEngine;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Box Collider 2Dx")]
    [DisallowMultipleComponent]
    public sealed class BoxCollider2Dx : ColliderConverter<BoxCollider, PolygonCollider2D>
    {
        #region Properties
        [Header("Convert to 3D")]
        [Tooltip("If enabled, converting from PolygonCollider2D to BoxCollider skips a safety check to make sure the PolygonCollider2D is in an appropriate BoxCollider shape.")] [SerializeField] private bool _skipSafetyCheck;

        public bool skipSafetyCheck
        {
            get => _skipSafetyCheck;
            set => _skipSafetyCheck = value;
        }
        #endregion

        #region BoxCollider2D Conversion
        /// <include file='../Documentation.xml' path='docs/BoxCollider2Dx/AddBoxCollider2D/*' />
        public PolygonCollider2D AddBoxCollider2D()
        {
            var polygonCollider2D = AddCollider2D();
            polygonCollider2D.CreateBoxCollider();

            return polygonCollider2D;
        }

        /// <include file='../Documentation.xml' path='docs/BoxCollider2Dx/AddBoxCollider2D/*' />
        public PolygonCollider2D AddBoxCollider2D(BoxCollider2D copyOf)
        {
            var polygonCollider2D = AddCollider2D();

            copyOf.ToPolygonCollider2D(polygonCollider2D);
            if(!Dimension.is2DNot3D)
            {
                var collider = GetColliderAt(collidersCount - 1);
                Collider2DToCollider(polygonCollider2D, collider);
            }

            return polygonCollider2D;
        }

        /// <include file='../Documentation.xml' path='docs/BoxCollider2Dx/CacheCollider2Ds/*' />
        public override void CacheCollider2Ds()
        {
            foreach(var boxCollider2D in transform2Dx.gameObject2D.GetComponents<BoxCollider2D>())
            {
                AddBoxCollider2D(boxCollider2D);
                DestroyImmediate(boxCollider2D);
            }

            foreach(var boxCollider2D in GetComponents<BoxCollider2D>())
            {
                AddBoxCollider2D(boxCollider2D);
                DestroyImmediate(boxCollider2D);
            }

            base.CacheCollider2Ds();
        }
        #endregion

        #region overrides
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
            if(skipSafetyCheck)
            {
                collider2D.ToBoxCollider(collider);
            }
            else 
            {
                collider2D.ToBoxColliderSafe(collider);
            }
        }
        #endregion
    }
}
