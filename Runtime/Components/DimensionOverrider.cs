using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Dimension Overrider")]
    [DisallowMultipleComponent]
    public class DimensionOverrider : MonoBehaviour
    {
        [Header("Overridden on Awake")]
        [SerializeField] private bool _is2DNot3D;
        [SerializeField] private bool _ignoreConversionTime;

        public bool is2DNot3D
        {
            get => _is2DNot3D;
            set => _is2DNot3D = value;
        }

        public bool ignoreConversionTime
        {
            get => _ignoreConversionTime;
            set => _ignoreConversionTime = value;
        }

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
