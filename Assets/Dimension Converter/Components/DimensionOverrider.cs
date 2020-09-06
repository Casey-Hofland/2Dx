using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Dimension Setter")]
    [DisallowMultipleComponent]
    public class DimensionOverrider : MonoBehaviour
    {
        [Header("Overridden on Awake")]
        public bool is2DNot3D;
        public bool ignoreConversionTime;

        private void Awake()
        {
            if(ignoreConversionTime)
            {
                var conversionTime = Dimension.conversionTime;
                Dimension.conversionTime = 0f;

                Dimension.is2DNot3D = is2DNot3D;

                Dimension.conversionTime = conversionTime;
            }
            else
            {
                Dimension.is2DNot3D = is2DNot3D;
            }
        }
    }
}
