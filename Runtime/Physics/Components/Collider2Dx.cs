#nullable enable
using System;
using UnityEngine;


namespace Unity2Dx.Physics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform2Dx))]
    public sealed class Collider2Dx : Converter<Transform2Dx, Collider, Collider2D>
    {
        private Transform2Dx? _transform2Dx;
        public Transform2Dx transform2Dx => _transform2Dx ? _transform2Dx! : (_transform2Dx = GetComponent<Transform2Dx>());

        [field: SerializeField] public Outliner? outliner { get; set; }
        [field: SerializeField] public PolygonCollider2DConversionOptions polygonCollider2DConversionOptions { get; set; }

        protected override void ComponentToComponent2D(Collider component, Collider2D component2D)
        {
            component.ToCollider2D(component2D, outliner);
        }

        protected override void Component2DToComponent(Collider2D component2D, Collider component)
        {
            component2D.ToCollider(component, true, polygonCollider2DConversionOptions);
        }

        public override Type? GetConversionType(Component component)
        {
            return component switch
            {
                SphereCollider _ => typeof(CircleCollider2D),
                CircleCollider2D _ => typeof(SphereCollider),
                CapsuleCollider _ => typeof(CapsuleCollider2D),
                CapsuleCollider2D _ => typeof(CapsuleCollider),
                BoxCollider _ => typeof(PolygonCollider2D),
                MeshCollider _ => typeof(PolygonCollider2D),
                PolygonCollider2D polygonCollider2D => polygonCollider2D.IsBoxCollider(transform2Dx.transform3D.rotation) ? typeof(BoxCollider) : typeof(MeshCollider),
                _ => base.GetConversionType(component),
            };
        }
    }
}
