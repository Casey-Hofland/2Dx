#nullable enable
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsConvert
    {
        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/ConstantForce/*'/>
        public static void ToConstantForce2D(this ConstantForce constantForce, ConstantForce2D constantForce2D)
        {
            constantForce2D.force = constantForce.force;
            constantForce2D.relativeForce = constantForce.relativeForce.ToVector2D(constantForce, constantForce2D);

            var normalizedRelativeTorque = constantForce.transform.rotation * constantForce.relativeTorque;
            constantForce2D.torque = (constantForce.torque.z + normalizedRelativeTorque.z) * Mathf.Rad2Deg;
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/ConstantForce2D/*'/>
        public static void ToConstantForce(this ConstantForce2D constantForce2D, ConstantForce constantForce)
        {
            Vector3 force = constantForce2D.force;
            force.z = constantForce.force.z;

            constantForce.force = force;
            constantForce.relativeForce = constantForce2D.relativeForce.ToVector(constantForce2D, constantForce, constantForce.relativeForce);

            var normalizedRelativeTorque = constantForce.transform.rotation * constantForce.relativeTorque;
            var weightedTorque2D = (Mathf.Abs(normalizedRelativeTorque.z) + Mathf.Abs(constantForce.torque.z)) * constantForce2D.torque;
            if (weightedTorque2D != 0f)
            {
                weightedTorque2D = 1f / weightedTorque2D * Mathf.Deg2Rad;
            }

            Vector3 torque = constantForce.torque;
            torque.z *= weightedTorque2D;
            constantForce.torque = torque;

            normalizedRelativeTorque.z *= weightedTorque2D;
            constantForce.relativeTorque = Quaternion.Inverse(constantForce.transform.rotation) * normalizedRelativeTorque;
        }
    }
}
