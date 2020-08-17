using System;
using UnityEditor;
using UnityEngine;

namespace Physics2DxSystem.Editor
{
    [CustomPropertyDrawer(typeof(Module2DxSettings))]
    internal class Module2DxSettingsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented.
            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUI.indentLevel = 0;

            // Calculate rects.
            var typeRect = new Rect(position.x, position.y, 160, position.height);
            var batchSize3DRect = new Rect(position.x + 165, position.y, 150, position.height);
            var batchSize2DRect = new Rect(position.x + 320, position.y, 150, position.height);

            // Set variables.
            var type = Type.GetType(property.FindPropertyRelative("typeName").stringValue);
            
            // Draw fields.
            if(GUI.enabled)
            {
                GUI.enabled = type.Assembly != typeof(Physics2Dx).Assembly;
                EditorGUI.LabelField(typeRect, type.Name);
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.LabelField(typeRect, type.Name);
            }

            EditorGUIUtility.labelWidth = 90f;
            EditorGUI.PropertyField(batchSize3DRect, property.FindPropertyRelative("batchSize3D"));
            EditorGUI.PropertyField(batchSize2DRect, property.FindPropertyRelative("batchSize2D"));

            // Set indent back to what it was.
            EditorGUI.indentLevel = indent;
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.EndProperty();
        }
    }
}
