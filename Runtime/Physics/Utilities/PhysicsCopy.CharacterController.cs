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

        public static void ToCharacterMoverBase(this CharacterMoverBase characterMoverBase, CharacterMoverBase other)
        {
            other.enabled = characterMoverBase.enabled;

            other.allowPerpetualJump = characterMoverBase.allowPerpetualJump;
            other.coyoteTime = characterMoverBase.coyoteTime;
            other.fastFallRatio = characterMoverBase.fastFallRatio;
            other.gravityScale = characterMoverBase.gravityScale;
            other.jumpBuffer = characterMoverBase.jumpBuffer;
            other.jumpHeight = characterMoverBase.jumpHeight;
            other.motion = characterMoverBase.motion;
            other.moveSpeed = characterMoverBase.moveSpeed;
            other.peakTime = characterMoverBase.peakTime;
            other.speedChangeRate = characterMoverBase.speedChangeRate;
            other.sprintBoost = characterMoverBase.sprintBoost;
            other.targetMotion = characterMoverBase.targetMotion;
        }

        public static void ToCharacterMover(this CharacterMover characterMover, CharacterMover other)
        {
            characterMover.ToCharacterMoverBase(other);

            other.rotationSpeed = characterMover.rotationSpeed;
        }

        public static void ToCharacterMover2D(this CharacterMover2D characterMover2D, CharacterMover2D other)
        {
            characterMover2D.ToCharacterMoverBase(other);
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
