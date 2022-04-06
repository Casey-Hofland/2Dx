#nullable enable
using System;
using UnityEngine;

namespace Unity2Dx.Physics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2Dx))]
    public sealed class Joint2Dx : CopyConverter<Rigidbody2Dx, Joint, Joint2D>
    {
        protected override void ComponentToComponent(Joint component, Joint other)
        {
            component.ToJoint(other);
        }

        protected override void Component2DToComponent2D(Joint2D component2D, Joint2D other)
        {
            component2D.ToJoint2D(other);
        }

        protected override void ComponentToComponent2D(Joint component, Joint2D component2D)
        {
            component.ToJoint2D(component2D);
        }

        protected override void Component2DToComponent(Joint2D component2D, Joint component)
        {
            component2D.ToJoint(component);
        }

        public override Type? GetConversionType(Component component)
        {
            return component switch
            {
                FixedJoint _ => typeof(FixedJoint2D),
                FixedJoint2D _ => typeof(FixedJoint),
                SpringJoint _ => typeof(SpringJoint2D),
                SpringJoint2D _ => typeof(SpringJoint),
                _ => base.GetConversionType(component),
            };
        }
    }
}
