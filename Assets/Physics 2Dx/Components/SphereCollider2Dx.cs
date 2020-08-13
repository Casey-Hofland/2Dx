using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics2DxSystem
{
    public sealed class SphereCollider2Dx : ColliderModule2Dx
    {
        public override void ConvertTo2D()
        {
            Debug.Log("My Sphere Collider 2Dx To 2D");
        }

        public override void ConvertTo3D()
        {
            Debug.Log("My Sphere Collider 2Dx To 3D");
        }
    }
}
