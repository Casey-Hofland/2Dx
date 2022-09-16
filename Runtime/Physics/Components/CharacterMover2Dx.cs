#nullable enable
using UnityEngine;
using UnityExtras;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Character Mover 2Dx")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController2Dx))]
    public sealed class CharacterMover2Dx : CopyConverter<CharacterController2Dx, CharacterMover, CharacterMover2D>
    {
        protected override void ComponentToComponent(CharacterMover component, CharacterMover other)
        {
            component.ToCharacterMover(other);
        }

        protected override void Component2DToComponent2D(CharacterMover2D component2D, CharacterMover2D other)
        {
            component2D.ToCharacterMover2D(other);
        }

        protected override void ComponentToComponent2D(CharacterMover component, CharacterMover2D component2D)
        {
            component.ToCharacterMover2D(component2D);
        }

        protected override void Component2DToComponent(CharacterMover2D component2D, CharacterMover component)
        {
            component2D.ToCharacterMover(component);
        }
    }
}
