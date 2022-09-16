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
            characterController.GenericPropertiesToCollider2D(characterController2D);
            ((IAuthor)characterController2D).Serialize();

            characterController2D.enabled = characterController.enabled;

            characterController2D.center = characterController.center;
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

            Vector3 center = characterController2D.center;
            center.z = characterController.center.z;
            characterController.center = characterController2D.center;
            characterController.detectCollisions = characterController2D.detectCollisions;
            characterController.enableOverlapRecovery = characterController2D.enableOverlapRecovery;
            characterController.height = characterController2D.height;
            characterController.minMoveDistance = characterController2D.minMoveDistance;
            characterController.radius = characterController2D.radius;
            characterController.skinWidth = characterController2D.skinWidth;
            characterController.slopeLimit = characterController2D.slopeLimit;
            characterController.stepOffset = characterController2D.stepOffset;
        }

        public static void ToCharacterMover2D(this CharacterMover characterMover, CharacterMover2D characterMover2D)
        {
            characterMover2D.enabled = characterMover.enabled;

            characterMover2D.coyoteTime = characterMover.coyoteTime;
            characterMover2D.fastFallRatio = characterMover.fastFallRatio;
            characterMover2D.gravityScale = characterMover.gravityScale;
            characterMover2D.jumpBuffer = characterMover.jumpBuffer;
            characterMover2D.jumpHeight = characterMover.jumpHeight;
            characterMover2D.motion = characterMover.motion;
            characterMover2D.moveSpeed = characterMover.moveSpeed;
            characterMover2D.peakTime = characterMover.peakTime;
            characterMover2D.speedChangeRate = characterMover.speedChangeRate;
            characterMover2D.sprintBoost = characterMover.sprintBoost;
            characterMover2D.targetMotion = characterMover.targetMotion;
        }

        public static void ToCharacterMover(this CharacterMover2D characterMover2D, CharacterMover characterMover)
        {
            characterMover.enabled = characterMover2D.enabled;

            characterMover.coyoteTime = characterMover2D.coyoteTime;
            characterMover.fastFallRatio = characterMover2D.fastFallRatio;
            characterMover.gravityScale = characterMover2D.gravityScale;
            characterMover.jumpBuffer = characterMover2D.jumpBuffer;
            characterMover.jumpHeight = characterMover2D.jumpHeight;
            characterMover.motion = characterMover2D.motion;
            characterMover.moveSpeed = characterMover2D.moveSpeed;
            characterMover.peakTime = characterMover2D.peakTime;
            characterMover.speedChangeRate = characterMover2D.speedChangeRate;
            characterMover.sprintBoost = characterMover2D.sprintBoost;
            characterMover.targetMotion = characterMover2D.targetMotion;
        }
    }
}
