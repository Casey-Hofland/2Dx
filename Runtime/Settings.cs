using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("CaseyDeCoder.DimensionConverter.Editor")]
namespace DimensionConverter
{
    internal sealed class Settings : ScriptableObject
    {
        internal const string componentMenu = "Dimension Converter/";

        [Tooltip("Returns if the dimension is currently 2D instead of 3D.")] 
        public bool is2DNot3D = true;

        [Tooltip("The default outliner to use on a MeshConverter if no outliner is specified on it.")] 
        public Outliner defaultOutliner;

        [Tooltip("The time it takes for the dimension to convert.")] 
        public float conversionTime = 0.6666667f;

        [Tooltip("If enabled, the conversion is batched and converted over multiple frames, but never longer than the conversionTime.")] 
        public bool batchConversion = true;

        [Tooltip("The order in which Converters are converted and in how large of a batch.")]
        public ConverterSettings[] convertersSettings = Array.Empty<ConverterSettings>();

        [Obsolete("Replace with Create Instance in SettingsProvider instead", false)]
        public void Reset()
        {
            is2DNot3D = true;
            defaultOutliner = null;
            conversionTime = 0.6666667f;
            batchConversion = true;
            convertersSettings = Array.Empty<ConverterSettings>();
        }

        private static Settings _settings;
        public static Settings GetSettings => _settings || (_settings = Resources.Load<Settings>(nameof(Settings))) ? _settings : (_settings = CreateInstance<Settings>());
    }
}

