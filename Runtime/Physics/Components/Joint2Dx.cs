#nullable enable
using System;
using UnityEngine;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Joint 2Dx")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2Dx))]
    public sealed class Joint2Dx : CopyConverter<Rigidbody2Dx, Joint, Joint2D>
    {
        protected override void ComponentToComponent(Joint component, Joint other)
        {
            switch (component)
            {
                case FixedJoint fixedJoint when other is FixedJoint otherFixedJoint:
                    fixedJoint.ToFixedJoint(otherFixedJoint);
                    break;
                case SpringJoint springJoint when other is SpringJoint otherSpringJoint:
                    springJoint.ToSpringJoint(otherSpringJoint);
                    break;
            }
        }

        protected override void Component2DToComponent2D(Joint2D component2D, Joint2D other)
        {
            switch (component2D)
            {
                case FixedJoint2D fixedJoint2D when other is FixedJoint2D otherFixedJoint2D:
                    fixedJoint2D.ToFixedJoint2D(otherFixedJoint2D);
                    break;
                case SpringJoint2D springJoint2D when other is SpringJoint2D otherSpringJoint2D:
                    springJoint2D.ToSpringJoint2D(otherSpringJoint2D);
                    break;
            }
        }

        protected override void ComponentToComponent2D(Joint component, Joint2D component2D)
        {
            //component.ToJoint2D(component2D);
        }

        protected override void Component2DToComponent(Joint2D component2D, Joint component)
        {
            //component2D.ToJoint(component);
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
