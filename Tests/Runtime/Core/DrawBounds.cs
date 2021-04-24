using UnityEngine;

namespace Unity2Dx.Tests
{
    public class DrawBounds : MonoBehaviour
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        private Renderer _renderer;
        private Renderer renderer => _renderer ? _renderer : (_renderer = GetComponent<Renderer>());
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;

            if(renderer)
            {
                var bounds = renderer.bounds;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }
}
