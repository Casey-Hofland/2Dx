using UnityEditor;
using UnityEngine;

namespace Unity2Dx.Editor
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
