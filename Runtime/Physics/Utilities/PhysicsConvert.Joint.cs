#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityExtras;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsConvert
    {
        internal static Dictionary<Joint, Rigidbody> connectedBodies = new();
        internal static Dictionary<Joint2D, Rigidbody2D> connectedBody2Ds = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterConnectedBodies()
        {
            connectedBodies = new();
            connectedBody2Ds = new();
        }

        private static void GenericPropertiesToJoint2D(this Joint joint, Joint2D joint2D)
        {
            joint2D.enabled = true;

            joint2D.breakForce = joint.breakForce;
            joint2D.breakTorque = joint.breakTorque;
            joint2D.enableCollision = joint.enableCollision;

            if (joint.connectedBody)
            {
                connectedBodies[joint] = joint.connectedBody;
                ConnectedBodyManager.Connect(joint.connectedBody, rigidbody2D => { if (joint2D) joint2D.connectedBody = rigidbody2D; });
            }
        }

        private static void GenericPropertiesToJoint2D(this Joint joint, AnchoredJoint2D anchoredJoint2D)
        {
            joint.GenericPropertiesToJoint2D((Joint2D)anchoredJoint2D);

            anchoredJoint2D.anchor = joint.anchor.ToVector2D(joint, anchoredJoint2D, RotationSource.ZOnly);
            anchoredJoint2D.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;

            Debug.LogWarning($"TODO: in the special case that either only {nameof(joint)} or {nameof(anchoredJoint2D)} has a connected body, the single connected body is used to calculate the connected anchor. This is a hotfix for 2Dx conversion but, for all intends and purposes, is an issue, for example in the case that a {nameof(Joint)} is converted to a {nameof(AnchoredJoint2D)} that is not (about to be) part of the same game object.");
            Component connectedBody = joint.connectedBody ? joint.connectedBody : anchoredJoint2D.connectedBody;
            Component connectedBody2D = anchoredJoint2D.connectedBody ? anchoredJoint2D.connectedBody : joint.connectedBody;
            if (connectedBody && connectedBody2D)
            {
                anchoredJoint2D.connectedAnchor = joint.connectedAnchor.ToVector2D(connectedBody, connectedBody2D, RotationSource.ZOnly);
            }
            else
            {
                anchoredJoint2D.connectedAnchor = joint.connectedAnchor;
            }
        }

        private static void GenericPropertiesToJoint(this Joint2D joint2D, Joint joint)
        {
            joint.breakForce = joint2D.breakForce;
            joint.breakTorque = joint2D.breakTorque;
            joint.enableCollision = joint2D.enableCollision;

            if (joint2D.connectedBody)
            {
                connectedBody2Ds[joint2D] = joint2D.connectedBody;
                ConnectedBodyManager.Connect(joint2D.connectedBody, rigidbody => joint.connectedBody = rigidbody);
            }
        }

        private static void GenericPropertiesToJoint(this AnchoredJoint2D anchoredJoint2D, Joint joint)
        {
            ((Joint2D)anchoredJoint2D).GenericPropertiesToJoint(joint);

            joint.anchor = anchoredJoint2D.anchor.ToVector(anchoredJoint2D, joint, joint.anchor, RotationSource.ZOnly);
            joint.autoConfigureConnectedAnchor = anchoredJoint2D.autoConfigureConnectedAnchor;

            Debug.LogWarning($"TODO: in the special case that either only {nameof(joint)} or {nameof(anchoredJoint2D)} has a connected body, the single connected body is used to calculate the connected anchor. This is a hotfix for 2Dx conversion but, for all intends and purposes, is an issue, for example in the case that a {nameof(Joint)} is converted to a {nameof(AnchoredJoint2D)} that is not (about to be) part of the same game object.");
            Component connectedBody2D = anchoredJoint2D.connectedBody ? anchoredJoint2D.connectedBody : joint.connectedBody;
            Component connectedBody = joint.connectedBody ? joint.connectedBody : anchoredJoint2D.connectedBody;
            if (connectedBody2D && connectedBody)
            {
                joint.connectedAnchor = anchoredJoint2D.connectedAnchor.ToVector(connectedBody2D, connectedBody, joint.connectedAnchor, RotationSource.ZOnly);
            }
            else
            {
                Vector3 connectedAnchor = anchoredJoint2D.connectedAnchor;
                connectedAnchor.z = joint.connectedAnchor.z;
                joint.connectedAnchor = connectedAnchor;
            }
        }

        #region SliderJoint & SliderJoint2D
        public static void ToSliderJoint2D(this SliderJoint sliderJoint, SliderJoint2D sliderJoint2D)
        {
            sliderJoint.configurableJoint.GenericPropertiesToJoint2D(sliderJoint2D);

            sliderJoint2D.autoConfigureAngle = false;
            sliderJoint2D.angle = sliderJoint.angle.eulerAngles.z;
            var limits = sliderJoint2D.limits;
            limits.max = -sliderJoint.minDistance;
            limits.min = -sliderJoint.maxDistance;
            sliderJoint2D.limits = limits;
            sliderJoint2D.useLimits = sliderJoint.useLimits;
        }

        public static void ToSliderJoint(this SliderJoint2D sliderJoint2D, SliderJoint sliderJoint)
        {
            sliderJoint2D.GenericPropertiesToJoint(sliderJoint.configurableJoint);
            ((IAuthor)sliderJoint).Serialize();

            sliderJoint.angle = Quaternion.Euler(0f, 0f, sliderJoint2D.angle);
            ((IAuthor)sliderJoint).Serialize();

            sliderJoint.maxDistance = -sliderJoint2D.limits.min;
            sliderJoint.minDistance = -sliderJoint2D.limits.max;
            sliderJoint.useLimits = sliderJoint2D.useLimits;
        }
        #endregion
    }
}
