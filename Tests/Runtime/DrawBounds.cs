using UnityEngine;

namespace DimensionConverter.Tests
{
    public class DrawBounds : MonoBehaviour
    {
        private Renderer _renderer;
        private new Renderer renderer => _renderer ? _renderer : (_renderer = GetComponent<Renderer>()); 

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
