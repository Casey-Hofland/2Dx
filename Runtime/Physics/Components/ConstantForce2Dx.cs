#nullable enable
using UnityEngine;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Constant Force 2Dx")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2Dx))]
    public sealed class ConstantForce2Dx : CopyConverter<Rigidbody2Dx, ConstantForce, ConstantForce2D>
    {
        protected override void ComponentToComponent(ConstantForce component, ConstantForce other)
        {
            component.ToConstantForce(other);
        }

        protected override void Component2DToComponent2D(ConstantForce2D component2D, ConstantForce2D other)
        {
            component2D.ToConstantForce2D(other);
        }

        protected override void ComponentToComponent2D(ConstantForce component, ConstantForce2D component2D)
        {
            component.ToConstantForce2D(component2D);
        }

        protected override void Component2DToComponent(ConstantForce2D component2D, ConstantForce component)
        {
            component2D.ToConstantForce(component);
        }
    }
}
