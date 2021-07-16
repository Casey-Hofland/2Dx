using UnityEngine;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Constant Force 2Dx")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform2Dx))]
    public sealed class ConstantForce2Dx : RequireRigidbodyConverter<ConstantForce, ConstantForce2D>
    {
        #region Required Components
        private Transform2Dx _transform2Dx;
        public Transform2Dx transform2Dx => _transform2Dx ? _transform2Dx : (_transform2Dx = GetComponent<Transform2Dx>());
        #endregion

        #region overrides
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
            component2D.transform.rotation = transform2Dx.transform2D.rotation;
            component.ToConstantForce2D(component2D);
        }

        protected override void Component2DToComponent(ConstantForce2D component2D, ConstantForce component)
        {
            component.transform.rotation = transform2Dx.transform3D.rotation;

            var component2DRotation = component2D.transform.rotation;
            component2D.transform.rotation = transform2Dx.transform2D.rotation;
            component2D.ToConstantForce(component);
            component2D.transform.rotation = component2DRotation;
        }
        #endregion
    }
}
