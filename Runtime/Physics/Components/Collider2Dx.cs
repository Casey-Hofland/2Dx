#nullable enable
using System;
using UnityEngine;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Collider 2Dx")]
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
            switch (component)
            {
                case SphereCollider sphereCollider when component2D is CircleCollider2D circleCollider2D:
                    sphereCollider.ToCircleCollider2D(circleCollider2D);
                    break;
                case CapsuleCollider capsuleCollider when component2D is CapsuleCollider2D capsuleCollider2D:
                    capsuleCollider.ToCapsuleCollider2D(capsuleCollider2D);
                    break;
                case BoxCollider boxCollider when component2D is PolygonCollider2D polygonCollider2D:
                    boxCollider.ToPolygonCollider2D(polygonCollider2D);
                    break;
                case MeshCollider meshCollider when component2D is PolygonCollider2D polygonCollider2D:
                    meshCollider.ToPolygonCollider2D(polygonCollider2D, outliner);
                    break;
            }
        }

        protected override void Component2DToComponent(Collider2D component2D, Collider component)
        {
            switch (component2D)
            {
                case CircleCollider2D circleCollider2D when component is SphereCollider sphereCollider:
                    circleCollider2D.ToSphereCollider(sphereCollider);
                    break;
                case CapsuleCollider2D capsuleCollider2D when component is CapsuleCollider capsuleCollider:
                    capsuleCollider2D.ToCapsuleCollider(capsuleCollider);
                    break;
                case PolygonCollider2D polygonCollider2D when component is BoxCollider boxCollider:
                    polygonCollider2D.ToBoxCollider(boxCollider);
                    break;
                case PolygonCollider2D polygonCollider2D when component is MeshCollider meshCollider:
                    polygonCollider2D.ToMeshCollider(meshCollider, polygonCollider2DConversionOptions);
                    break;
            }
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
