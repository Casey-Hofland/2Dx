#nullable enable
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsCopy
    {
        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/ConstantForce/*'/>
        public static void ToConstantForce(this ConstantForce constantForce, ConstantForce other)
        {
            other.enabled = constantForce.enabled;
            other.hideFlags = constantForce.hideFlags;

            other.force = constantForce.force;
            other.relativeForce = constantForce.relativeForce;
            other.torque = constantForce.torque;
            other.relativeTorque = constantForce.relativeTorque;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/ConstantForce2D/*'/>
        public static void ToConstantForce2D(this ConstantForce2D constantForce2D, ConstantForce2D other)
        {
            other.enabled = constantForce2D.enabled;
            other.hideFlags = constantForce2D.hideFlags;

            other.force = constantForce2D.force;
            other.relativeForce = constantForce2D.relativeForce;
            other.torque = constantForce2D.torque;
        }
    }
}
