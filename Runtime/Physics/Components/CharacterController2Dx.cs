#nullable enable
using UnityEngine;
using UnityExtras;

namespace Unity2Dx.Physics
{
    public sealed class CharacterController2Dx : CopyConvertible<CharacterController, CharacterController2D>
    {
        protected override void ComponentToComponent(CharacterController component, CharacterController other)
        {
            component.ToCharacterController(other);
        }

        protected override void Component2DToComponent2D(CharacterController2D component2D, CharacterController2D other)
        {
            component2D.ToCharacterController2D(other);

            // Destroy the capsule collider 2D immediately after we have copied our CharacterController2D. This can't be done from the CharacterController2D itself unfortunately.
            var capsuleCollider2D = component2D.capsuleCollider2D;
            DestroyImmediate(component2D);
            DestroyImmediate(capsuleCollider2D);
        }

        protected override void ComponentToComponent2D(CharacterController component, CharacterController2D component2D)
        {
            component.ToCharacterController2D(component2D);
        }

        protected override void Component2DToComponent(CharacterController2D component2D, CharacterController component)
        {
            component2D.ToCharacterController(component);
        }
    }
}
