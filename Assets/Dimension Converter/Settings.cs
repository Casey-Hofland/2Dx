using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("DimensionConverter.Editor")]
namespace DimensionConverter
{
    internal sealed class Settings : ScriptableObject
    {
        internal const string componentMenu = "Dimension Converter/";

        /// <include file='./Documentation.xml' path='docs/Dimension/is2Dnot3D/*' />
        [Tooltip("Returns if the dimension is currently 2D instead of 3D.")] 
        public bool is2DNot3D;

        /// <include file='./Documentation.xml' path='docs/Dimension/conversionTime/*' />
        [Tooltip("The time it takes for the dimension to convert.")] 
        public float conversionTime;

        /// <include file='./Documentation.xml' path='docs/Dimension/batchConversion/*' />
        [Tooltip("If enabled, the conversion is batched and converted over multiple frames, but never longer than the conversionTime.")] 
        public bool batchConversion;

        [Tooltip("The order in which Converters are converted and in how large of a batch.")]
        public ConverterSettings[] convertersSettings;

        [Tooltip("This will hide certain gameObjects that are used as a cache or otherwise background activity. Disable this to get a better understanding of how Dimension Converter operates and debug issues.")] 
        public bool slimHierarchy;
        
        [Tooltip("CameraConverters will take control over their camera's settings so updates to the CameraConverter are directly visualized. The camera's projection view will depend on the is 2D Not 3D setting.")]
        public bool cameraConverterLiveUpdate;

        public void Reset()
        {
            is2DNot3D = true;
            conversionTime = 0.6666667f;
            batchConversion = true;
            convertersSettings = Array.Empty<ConverterSettings>();
            slimHierarchy = true;
            cameraConverterLiveUpdate = true;
        }
    }
}

