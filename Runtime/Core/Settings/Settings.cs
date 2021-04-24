using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("CaseyDeCoder.2Dx.Core.Editor")]
namespace Unity2Dx
{
    internal sealed class Settings : ScriptableObject
    {
        [Tooltip("Returns if the dimension is currently 2D instead of 3D.")] 
        public bool is2DNot3D = true;

        [Tooltip("The time it takes for the dimension to convert.")] 
        public float conversionTime = 0.6666667f;

        [Tooltip("The time scale when the dimension is converting.")]
        public float conversionTimeScale = 0f;

        [Tooltip("If enabled, the conversion is batched and converted over multiple frames, but never longer than the conversionTime.")] 
        public bool batchConversion = true;

        [Tooltip("The order in which Converters are converted and in how large of a batch.")]
        public ConverterSettings[] convertersSettings = Array.Empty<ConverterSettings>();

        private static Settings _settings;
        public static Settings GetSettings => _settings || (_settings = Resources.Load<Settings>(nameof(Settings))) ? _settings : (_settings = CreateInstance<Settings>());
    }
}

