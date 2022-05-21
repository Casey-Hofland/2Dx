#nullable enable
using UnityEngine;
using UnityExtras;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsConvert
    {
        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/CharacterController/*'/>
        public static void ToCharacterController2D(this CharacterController characterController, CharacterController2D characterController2D)
        {
            var capsuleCollider2DEnabled = characterController2D.capsuleCollider2D.enabled;
            var capsuleCollider2DHideFlags = characterController2D.capsuleCollider2D.hideFlags;
            characterController.GenericPropertiesToCollider2D(characterController2D);
            characterController2D.capsuleCollider2D.enabled = capsuleCollider2DEnabled;
            characterController2D.capsuleCollider2D.hideFlags = capsuleCollider2DHideFlags;

            characterController2D.enabled = characterController.enabled;
            characterController2D.hideFlags = characterController.hideFlags;

            characterController2D.detectCollisions = characterController.detectCollisions;
            characterController2D.enableOverlapRecovery = characterController.enableOverlapRecovery;
            characterController2D.height = characterController.height;
            characterController2D.minMoveDistance = characterController.minMoveDistance;
            characterController2D.radius = characterController.radius;
            characterController2D.skinWidth = characterController.skinWidth;
            characterController2D.slopeLimit = characterController.slopeLimit;
            characterController2D.stepOffset = characterController.stepOffset;
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/CharacterController2D/*'/>
        public static void ToCharacterController(this CharacterController2D characterController2D, CharacterController characterController)
        {
            characterController2D.capsuleCollider2D.GenericPropertiesToCollider(characterController);

            characterController.enabled = characterController2D.enabled;
            characterController.hideFlags = characterController2D.hideFlags;

            characterController.detectCollisions = characterController2D.detectCollisions;
            characterController.enableOverlapRecovery = characterController2D.enableOverlapRecovery;
            characterController.height = characterController2D.height;
            characterController.minMoveDistance = characterController2D.minMoveDistance;
            characterController.radius = characterController2D.radius;
            characterController.skinWidth = characterController2D.skinWidth;
            characterController.slopeLimit = characterController2D.slopeLimit;
            characterController.stepOffset = characterController2D.stepOffset;
        }
    }
}
