using UnityEditor;
using UnityEngine;

namespace DimensionConverter.Tests.Editor
{
    [CustomEditor(typeof(ConversionTester), true)]
    public class ConversionTesterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var conversionTester = target as ConversionTester;

            if(GUILayout.Button("Convert to 2D"))
            {
                conversionTester.ConvertTo2D();
            }
            if(GUILayout.Button("Convert to 3D"))
            {
                conversionTester.ConvertTo3D();
            }

            EditorGUILayout.Space(12);

            EditorGUILayout.HelpBox("This component will not be saved in a build.", MessageType.Info);
        }
    }
}

