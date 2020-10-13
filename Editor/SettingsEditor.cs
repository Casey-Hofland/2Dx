using UnityEditor;
using UnityEngine;

namespace DimensionConverter.Editor
{
    [CustomEditor(typeof(Settings))]
    [CanEditMultipleObjects]
    internal class SettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}
