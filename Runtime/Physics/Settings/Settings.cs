using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("CaseyDeCoder.2Dx.Core.Editor")]
namespace Unity2Dx.Physics
{
    internal sealed class Settings : ScriptableObject
    {
        [Tooltip("The default outliner to use on a MeshConverter if no outliner is specified on it.")]
        public Outliner defaultOutliner;

        private static Settings _settings;
        public static Settings GetSettings => _settings || (_settings = Resources.Load<Settings>(nameof(Settings))) ? _settings : (_settings = CreateInstance<Settings>());
    }
}
