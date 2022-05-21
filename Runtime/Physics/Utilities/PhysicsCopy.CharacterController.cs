#nullable enable
using UnityEngine;
using UnityExtras;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsCopy
    {
        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/CharacterController/*'/>
        public static void ToCharacterController(this CharacterController characterController, CharacterController other)
        {
            characterController.GenericPropertiesToCollider(other);

            other.detectCollisions = characterController.detectCollisions;
            other.enableOverlapRecovery = characterController.enableOverlapRecovery;
            other.height = characterController.height;
            other.minMoveDistance = characterController.minMoveDistance;
            other.radius = characterController.radius;
            other.skinWidth = characterController.skinWidth;
            other.slopeLimit = characterController.slopeLimit;
            other.stepOffset = characterController.stepOffset;
        }

        /// <include file='./PhysicsCopy.xml' path='docs/PhysicsCopy/CharacterController2D/*'/>
        public static void ToCharacterController2D(this CharacterController2D characterController2D, CharacterController2D other)
        {
            characterController2D.capsuleCollider2D.GenericPropertiesToCollider2D(other);

            other.hideFlags = characterController2D.hideFlags;
            other.enabled = characterController2D.enabled;

            other.detectCollisions = characterController2D.detectCollisions;
            other.enableOverlapRecovery = characterController2D.enableOverlapRecovery;
            other.height = characterController2D.height;
            other.minMoveDistance = characterController2D.minMoveDistance;
            other.radius = characterController2D.radius;
            other.skinWidth = characterController2D.skinWidth;
            other.slopeLimit = characterController2D.slopeLimit;
            other.stepOffset = characterController2D.stepOffset;
        }
    }
}
