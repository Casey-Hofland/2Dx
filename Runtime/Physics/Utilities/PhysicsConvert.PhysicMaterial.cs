#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace Unity2Dx.Physics
{
    public static partial class PhysicsConvert
    {
        private static Dictionary<PhysicMaterial, PhysicsMaterial2D> physicMaterialPairs = new Dictionary<PhysicMaterial, PhysicsMaterial2D>();
        private static Dictionary<PhysicsMaterial2D, PhysicMaterial> physicsMaterial2DPairs = new Dictionary<PhysicsMaterial2D, PhysicMaterial>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitPhysicMaterialPairs()
        {
            physicMaterialPairs = new Dictionary<PhysicMaterial, PhysicsMaterial2D>();
            physicsMaterial2DPairs = new Dictionary<PhysicsMaterial2D, PhysicMaterial>();
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/PhysicMaterial/*'/>
        public static PhysicsMaterial2D? AsPhysicsMaterial2D(this PhysicMaterial physicMaterial)
        {
            if (!physicMaterial)
            {
                return null;
            }

            // If there is no 2D equivalent of the physicMaterial, create a new one.
            if (!physicMaterialPairs.TryGetValue(physicMaterial, out var physicsMaterial2D))
            {
                physicsMaterial2D = new PhysicsMaterial2D();

                physicMaterialPairs.Add(physicMaterial, physicsMaterial2D);
                physicsMaterial2DPairs.Add(physicsMaterial2D, physicMaterial);
            }

            // Carry over the physicMaterial settings to its 2D equivalent.
            physicsMaterial2D.name = physicMaterial.name;
            physicsMaterial2D.bounciness = physicMaterial.bounciness;
            physicsMaterial2D.friction = physicMaterial.dynamicFriction;

            return physicsMaterial2D;
        }

        /// <include file='./PhysicsConvert.xml' path='docs/PhysicsConvert/PhysicsMaterial2D/*'/>
        public static PhysicMaterial? AsPhysicMaterial(this PhysicsMaterial2D physicsMaterial2D)
        {
            if (!physicsMaterial2D)
            {
                return null;
            }

            // If there is no 3D equivalent of the physicsMaterial2D, create a new one.
            if (!physicsMaterial2DPairs.TryGetValue(physicsMaterial2D, out var physicMaterial))
            {
                physicMaterial = new PhysicMaterial();

                physicsMaterial2DPairs.Add(physicsMaterial2D, physicMaterial);
                physicMaterialPairs.Add(physicMaterial, physicsMaterial2D);
            }

            // Carry over the physicsMaterial2D settings to its 3D equivalent.
            physicMaterial.name = physicsMaterial2D.name;
            physicMaterial.bounciness = physicsMaterial2D.bounciness;
            physicMaterial.dynamicFriction = physicsMaterial2D.friction;

            return physicMaterial;
        }
    }
}
