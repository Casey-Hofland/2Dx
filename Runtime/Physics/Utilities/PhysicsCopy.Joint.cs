#nullable enable
using UnityEngine;
using UnityExtras;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsCopy
    {
        private static void GenericPropertiesToJoint(this Joint joint, Joint other)
        {
            other.anchor = joint.anchor;
            other.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
            other.axis = joint.axis;
            other.breakForce = joint.breakForce;
            other.breakTorque = joint.breakTorque;
            other.connectedAnchor = joint.connectedAnchor;
            other.connectedArticulationBody = joint.connectedArticulationBody;
            other.connectedBody = joint.connectedBody;
            other.connectedMassScale = joint.connectedMassScale;
            other.enableCollision = joint.enableCollision;
            other.enablePreprocessing = joint.enablePreprocessing;
            other.massScale = joint.massScale;

            if (PhysicsConvert.connectedBodies.Remove(joint, out var connectedBody))
            {
                ConnectedBodyManager.Disconnect(connectedBody);
            }
        }

        private static void GenericPropertiesToJoint2D(this Joint2D joint2D, Joint2D other)
        {
            other.enabled = joint2D.enabled;

            other.breakForce = joint2D.breakForce;
            other.breakTorque = joint2D.breakTorque;
            other.connectedBody = joint2D.connectedBody;
            other.enableCollision = joint2D.enableCollision;

            if (PhysicsConvert.connectedBody2Ds.Remove(joint2D, out var connectedBody))
            {
                ConnectedBodyManager.Disconnect(connectedBody);
            }
        }

        private static void GenericPropertiesToJoint2D(this AnchoredJoint2D anchoredJoint2D, AnchoredJoint2D other)
        {
            anchoredJoint2D.GenericPropertiesToJoint2D((Joint2D)other);

            other.anchor = anchoredJoint2D.anchor;
            other.autoConfigureConnectedAnchor = anchoredJoint2D.autoConfigureConnectedAnchor;
            other.connectedAnchor = anchoredJoint2D.connectedAnchor;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/FixedJoint/*'/>
        public static void ToFixedJoint(this FixedJoint fixedJoint, FixedJoint other)
        {
            fixedJoint.GenericPropertiesToJoint(other);
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/FixedJoint2D/*'/>
        public static void ToFixedJoint2D(this FixedJoint2D fixedJoint2D, FixedJoint2D other)
        {
            fixedJoint2D.GenericPropertiesToJoint2D(other);

            other.dampingRatio = fixedJoint2D.dampingRatio;
            other.frequency = fixedJoint2D.frequency;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/SpringJoint/*'/>
        public static void ToSpringJoint(this SpringJoint springJoint, SpringJoint other)
        {
            springJoint.GenericPropertiesToJoint(other);

            other.damper = springJoint.damper;
            other.maxDistance = springJoint.maxDistance;
            other.minDistance = springJoint.minDistance;
            other.spring = springJoint.spring;
            other.tolerance = springJoint.tolerance;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/SpringJoint2D/*'/>
        public static void ToSpringJoint2D(this SpringJoint2D springJoint2D, SpringJoint2D other)
        {
            springJoint2D.GenericPropertiesToJoint2D(other);

            other.autoConfigureDistance = springJoint2D.autoConfigureDistance;
            other.dampingRatio = springJoint2D.dampingRatio;
            other.distance = springJoint2D.distance;
            other.frequency = springJoint2D.frequency;
        }

        public static void ToSliderJoint(this SliderJoint sliderJoint, SliderJoint other)
        {
            sliderJoint.configurableJoint.GenericPropertiesToJoint(other.configurableJoint);
            ((IAuthor)other).Serialize();

            other.angle = sliderJoint.angle;
            other.bounciness = sliderJoint.bounciness;
            other.contactDistance = sliderJoint.contactDistance;
            other.maxDistance = sliderJoint.maxDistance;
            other.minDistance = sliderJoint.minDistance;
            other.revolving = sliderJoint.revolving;
            other.useLimits = sliderJoint.useLimits;
        }

        public static void ToSliderJoint2D(this SliderJoint2D sliderJoint2D, SliderJoint2D other)
        {
            sliderJoint2D.GenericPropertiesToJoint2D(other);

            other.angle = sliderJoint2D.angle;
            other.autoConfigureAngle = sliderJoint2D.autoConfigureAngle;
            other.limits = sliderJoint2D.limits;
            other.motor = sliderJoint2D.motor;
            other.useLimits = sliderJoint2D.useLimits;
            other.useMotor = sliderJoint2D.useMotor;
        }

        public static void ToConfigurableJoint(this ConfigurableJoint configurableJoint, ConfigurableJoint other)
        {
            configurableJoint.GenericPropertiesToJoint(other);

            other.angularXDrive = configurableJoint.angularXDrive;
            other.angularXLimitSpring = configurableJoint.angularXLimitSpring;
            other.angularXMotion = configurableJoint.angularXMotion;
            other.angularYLimit = configurableJoint.angularYLimit;
            other.angularYMotion = configurableJoint.angularYMotion;
            other.angularYZDrive = configurableJoint.angularYZDrive;
            other.angularYZLimitSpring = configurableJoint.angularYZLimitSpring;
            other.angularZLimit = configurableJoint.angularZLimit;
            other.angularZMotion = configurableJoint.angularZMotion;

            other.configuredInWorldSpace = configurableJoint.configuredInWorldSpace;
            other.rotationDriveMode = configurableJoint.rotationDriveMode;
            other.secondaryAxis = configurableJoint.secondaryAxis;
            other.slerpDrive = configurableJoint.slerpDrive;
            other.swapBodies = configurableJoint.swapBodies;

            other.lowAngularXLimit = configurableJoint.lowAngularXLimit;
            other.highAngularXLimit = configurableJoint.highAngularXLimit;

            other.linearLimit = configurableJoint.linearLimit;
            other.linearLimitSpring = configurableJoint.linearLimitSpring;

            other.projectionAngle = configurableJoint.projectionAngle;
            other.projectionDistance = configurableJoint.projectionDistance;
            other.projectionMode = configurableJoint.projectionMode;

            other.targetAngularVelocity = configurableJoint.targetAngularVelocity;
            other.targetPosition = configurableJoint.targetPosition;
            other.targetRotation = configurableJoint.targetRotation;
            other.targetVelocity = configurableJoint.targetVelocity;

            other.xDrive = configurableJoint.xDrive;
            other.xMotion = configurableJoint.xMotion;
            other.yDrive = configurableJoint.yDrive;
            other.yMotion = configurableJoint.yMotion;
            other.zDrive = configurableJoint.zDrive;
            other.zMotion = configurableJoint.zMotion;
        }
    }
}
