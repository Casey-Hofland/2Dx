#nullable enable
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsConvert
    {
        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/ConstantForce/*'/>
        public static void ToConstantForce2D(this ConstantForce constantForce, ConstantForce2D constantForce2D)
        {
            constantForce2D.enabled = constantForce.enabled;

            constantForce2D.force = constantForce.force;
            constantForce2D.relativeForce = constantForce.relativeForce.ToVector2D(constantForce, constantForce2D, RotationSource.Rigidbody2D);

            var normalizedRelativeTorque = constantForce.transform.rotation * constantForce.relativeTorque;
            constantForce2D.torque = (constantForce.torque.z + normalizedRelativeTorque.z) * Mathf.Rad2Deg;
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/ConstantForce2D/*'/>
        public static void ToConstantForce(this ConstantForce2D constantForce2D, ConstantForce constantForce)
        {
            constantForce2D.enabled = constantForce.enabled;

            Vector3 force = constantForce2D.force;
            force.z = constantForce.force.z;

            constantForce.force = force;
            constantForce.relativeForce = constantForce2D.relativeForce.ToVector(constantForce2D, constantForce, constantForce.relativeForce, RotationSource.Rigidbody2D);

            var normalizedRelativeTorque = constantForce.transform.rotation * constantForce.relativeTorque;

            var torqueWeight = Mathf.Abs(constantForce.torque.z) + Mathf.Abs(normalizedRelativeTorque.z);
            if (torqueWeight != 0f)
            {
                torqueWeight = Mathf.Abs(constantForce.torque.z) / torqueWeight;
            }

            var torque2D = constantForce2D.torque * Mathf.Deg2Rad;

            var torque = constantForce.torque;
            torque.z = torque2D * torqueWeight;
            constantForce.torque = torque;

            normalizedRelativeTorque.z = torque2D * (1f - torqueWeight);
            constantForce.relativeTorque = Quaternion.Inverse(constantForce.transform.rotation) * normalizedRelativeTorque;
        }
    }
}
