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

            other.center = characterController.center;
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
            ((IAuthor)other).Serialize();

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

        public static void ToCharacterMover(this CharacterMover characterMover, CharacterMover other)
        {
            other.enabled = characterMover.enabled;

            other.coyoteTime = characterMover.coyoteTime;
            other.fastFallRatio = characterMover.fastFallRatio;
            other.gravityScale = characterMover.gravityScale;
            other.jumpBuffer = characterMover.jumpBuffer;
            other.jumpHeight = characterMover.jumpHeight;
            other.motion = characterMover.motion;
            other.moveSpeed = characterMover.moveSpeed;
            other.peakTime = characterMover.peakTime;
            other.rotationSpeed = characterMover.rotationSpeed;
            other.speedChangeRate = characterMover.speedChangeRate;
            other.sprintBoost = characterMover.sprintBoost;
            other.targetMotion = characterMover.targetMotion;
        }

        public static void ToCharacterMover2D(this CharacterMover2D characterMover2D, CharacterMover2D other)
        {
            other.enabled = characterMover2D.enabled;

            other.coyoteTime = characterMover2D.coyoteTime;
            other.fastFallRatio = characterMover2D.fastFallRatio;
            other.gravityScale = characterMover2D.gravityScale;
            other.jumpBuffer = characterMover2D.jumpBuffer;
            other.jumpHeight = characterMover2D.jumpHeight;
            other.motion = characterMover2D.motion;
            other.moveSpeed = characterMover2D.moveSpeed;
            other.peakTime = characterMover2D.peakTime;
            other.speedChangeRate = characterMover2D.speedChangeRate;
            other.sprintBoost = characterMover2D.sprintBoost;
            other.targetMotion = characterMover2D.targetMotion;
        }

        public static void ToFirstPersonCharacter(this FirstPersonCharacter firstPersonCharacter, FirstPersonCharacter other)
        {
            other.bottomClamp = firstPersonCharacter.bottomClamp;
            other.topClamp = firstPersonCharacter.topClamp;
            other.lookTransform = firstPersonCharacter.lookTransform;

            other.enabled = false;

            other.moveReaction = firstPersonCharacter.moveReaction;
            other.lookReaction = firstPersonCharacter.lookReaction;
            other.sprintReaction = firstPersonCharacter.sprintReaction;
            other.jumpReaction = firstPersonCharacter.jumpReaction;

            other.enabled = firstPersonCharacter.enabled;
        }

        public static void ToThirdPersonCharacter2D(this ThirdPersonCharacter2D thirdPersonCharacter2D, ThirdPersonCharacter2D other)
        {
            other.enabled = false;

            other.moveReaction = thirdPersonCharacter2D.moveReaction;
            other.sprintReaction = thirdPersonCharacter2D.sprintReaction;
            other.jumpReaction = thirdPersonCharacter2D.jumpReaction;

            other.enabled = thirdPersonCharacter2D.enabled;
        }
    }
}
