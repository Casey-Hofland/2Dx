#nullable enable
using UnityEngine;
using UnityExtras;
using UnityExtras.InputSystem;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/First Person Character 2Dx")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterMover2Dx))]
    public sealed class FirstPersonCharacter2Dx : CopyConverter<CharacterMover2Dx, FirstPersonCharacter, ThirdPersonCharacter2D>
    {
        [field: SerializeField] public InputReaction moveReaction { get; set; }
        [field: SerializeField] public InputReaction move2DReaction { get; set; }
        [field: SerializeField] public InputReaction lookReaction { get; set; }
        [field: SerializeField] public InputReaction sprintReaction { get; set; }
        [field: SerializeField] public InputReaction jumpReaction { get; set; }

        protected override void ComponentToComponent(FirstPersonCharacter component, FirstPersonCharacter other)
        {
            component.ToFirstPersonCharacter(other);
        }

        protected override void Component2DToComponent2D(ThirdPersonCharacter2D component2D, ThirdPersonCharacter2D other)
        {
            component2D.ToThirdPersonCharacter2D(other);
        }

        protected override void ComponentToComponent2D(FirstPersonCharacter component, ThirdPersonCharacter2D component2D)
        {
            component2D.enabled = false;

            moveReaction.input.action?.Disable();
            lookReaction.input.action?.Disable();

            move2DReaction.input.action?.Enable();
            sprintReaction.input.action?.Enable();
            jumpReaction.input.action?.Enable();

            component2D.moveReaction = move2DReaction;
            component2D.sprintReaction = sprintReaction;
            component2D.jumpReaction = jumpReaction;

            component2D.enabled = component.enabled;
        }

        protected override void Component2DToComponent(ThirdPersonCharacter2D component2D, FirstPersonCharacter component)
        {
            component.enabled = false;

            move2DReaction.input.action?.Disable();

            moveReaction.input.action?.Enable();
            lookReaction.input.action?.Enable();
            sprintReaction.input.action?.Enable();
            jumpReaction.input.action?.Enable();

            component.moveReaction = moveReaction;
            component.lookReaction = lookReaction;
            component.sprintReaction = sprintReaction;
            component.jumpReaction = jumpReaction;

            component.enabled = component2D.enabled;
        }
    }
}
