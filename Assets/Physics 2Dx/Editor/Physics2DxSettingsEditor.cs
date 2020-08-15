using UnityEditor;
using UnityEngine;

namespace Physics2DxSystem.Editor
{
    [CustomEditor(typeof(Physics2DxSettings))]
    [CanEditMultipleObjects]
    public class Physics2DxSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}
