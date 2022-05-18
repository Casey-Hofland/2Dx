#nullable enable
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsCopy
    {
        private static void GenericPropertiesToJoint(this Joint joint, Joint other)
        {
            other.hideFlags = joint.hideFlags;

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
        }

        private static void GenericPropertiesToJoint2D(this Joint2D joint2D, Joint2D other)
        {
            other.enabled = joint2D.enabled;
            other.hideFlags = joint2D.hideFlags;

            other.breakForce = joint2D.breakForce;
            other.breakTorque = joint2D.breakTorque;
            other.connectedBody = joint2D.connectedBody;
            other.enableCollision = joint2D.enableCollision;
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
    }
}
