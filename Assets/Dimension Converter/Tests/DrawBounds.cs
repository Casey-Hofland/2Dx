using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBounds : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        var bounds = GetComponent<MeshRenderer>().bounds;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
