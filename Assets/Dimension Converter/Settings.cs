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
        [Tooltip("Returns if Physics2Dx is currently operating in 2D instead of 3D.")] 
        public bool is2DNot3D;

        /// <include file='./Documentation.xml' path='docs/Dimension/conversionTime/*' />
        [Tooltip("The time it takes for the Physics2Dx mode to convert.")] 
        public float conversionTime;

        /// <include file='./Documentation.xml' path='docs/Dimension/splitConversionOverMultipleFrames/*' />
        [Tooltip("This batches Physics Conversion over multiple frames. Enable this to improve performance on conversion, but when retrieving data from a Module2Dx make sure to wait after the conversion has finished. If the conversion takes longer than the Conversion Time the splitting will halt and the full conversion will execute before moving on. If you are still experiencing performance issues, try decreasing the Batch Size on heavy Module2Dx's or increasing your Conversion Time depending on the issue.")] 
        public bool batchConversion;

        [Tooltip("The order in which Converters are converted and in how large of a batch.")]
        public ConverterSettings[] convertersSettings;

        [Tooltip("This will hide every gameObject created by Physics2Dx that is used as a cache or otherwise background activity. Disable this to get a better understanding of how Physics2Dx operates and debug issues.")] 
        public bool slimHierarchy;
        
        [Tooltip("CameraConverters will take control over their camera's settings so updates to the camera2Dx are directly visualized. The camera's projection view will depend on the is 2D Not 3D setting.")]
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

