#nullable enable
using System;
using UnityEngine;

namespace Unity2Dx.Physics
{
    [CreateAssetMenu(fileName = nameof(Outliner) , menuName = "2Dx/" + nameof(Outliner), order = 1)]
    [Serializable]
    public class Outliner : ScriptableObject
    {
        [Tooltip("The resolution used for the outliner. A higher resolution leads to more accuracy while a lower resolution increases outline performance.")] [Min(32)] public int resolution = 256;
        [Tooltip("How much difference in pixels in a straight line is considered a gap. This can help smooth out the outline a bit.")] [Min(1)] public uint gapLength = 3;
        [Tooltip("Product for optimizing the outline based on angle. 1 means no optimization. This value should be kept pretty high if you want to maintain round shapes. Note that some points (e.g. outer angles) are never optimized.")] [Range(0f, 1f)] public float product = 0.99f;
        [Tooltip("Tolerance for optimizing the outline based on distance. 0.03 to 0.05 is good for most cases.")] [Min(0f)] public float tolerance = 0.03f;
    }
}
