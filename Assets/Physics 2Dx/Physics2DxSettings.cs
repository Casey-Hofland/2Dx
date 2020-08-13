using UnityEngine;

namespace Physics2DxSystem
{
    public class Physics2DxSettings : ScriptableObject
    {
        /// <include file='./Documentation.xml' path='docs/Physics2Dx/is2Dnot3D/*' />
        [Tooltip("Returns if Physics2Dx is currently operating in 2D instead of 3D.")] public bool is2Dnot3D = true;
        /// <include file='./Documentation.xml' path='docs/Physics2Dx/conversionTime/*' />
        [Tooltip("The time it takes for the Physics2Dx mode to convert.")] public float conversionTime = 0.6666667f;
        /// <include file='./Documentation.xml' path='docs/Physics2Dx/splitConversionOverMultipleFrames/*' />
        [Tooltip("This splits 2Dx conversion over multiple frames. Enable this to improve performance on conversion, but when retrieving data from a Module2Dx make sure to wait after the conversion has finished. If the conversion takes longer than the Conversion Time the splitting will halt and the full conversion will execute before moving on. If you are still experiencing performance issues, try decreasing the Batch Size on heavy Module2Dx's or increasing your Conversion Time depending on the problem.")] public bool splitConversion = true;
        [Tooltip("This will hide every gameObject created by Physics2Dx that is used as a cache or otherwise background activity. Disable this to get a better understanding of how Physics2Dx operates.")] public bool slimHierarchy = true;
        [Tooltip("The order in which Module2Dxes are converted and in how large of a batch.")]
        public Module2DxSettings[] module2DxesSettings = new Module2DxSettings[]
        {
            new Module2DxSettings { order = -1000, type = typeof(Transform2Dx), batchSize3D = 1000, batchSize2D = 1000 },
            new Module2DxSettings { order = -100, type = typeof(SphereCollider2Dx), batchSize3D = 100, batchSize2D = 100 },
        };

        public void Reset()
        {
            is2Dnot3D = true;
            conversionTime = 0.6666667f;
            splitConversion = true;
            slimHierarchy = true;
            module2DxesSettings = new Module2DxSettings[]
            {
                new Module2DxSettings { order = -1000, type = typeof(Transform2Dx), batchSize3D = 1000, batchSize2D = 1000 },
                new Module2DxSettings { order = -100, type = typeof(SphereCollider2Dx), batchSize3D = 100, batchSize2D = 100 },
            };
        }
    }
}

