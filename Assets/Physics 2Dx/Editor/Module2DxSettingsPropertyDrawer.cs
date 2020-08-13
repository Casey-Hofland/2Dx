using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Physics2DxSystem.Editor
{
    [CustomPropertyDrawer(typeof(Module2DxSettings))]
    public class Module2DxSettingsPropertyDrawer : PropertyDrawer
    {
        //public override VisualElement CreatePropertyGUI(SerializedProperty property)
        //{
        //    return base.CreatePropertyGUI(property);
        //}

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented.
            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUI.indentLevel = 0;

            // Calculate rects.
            var orderRect = new Rect(position.x, position.y, 200, position.height);
            var batchSize3DRect = new Rect(position.x + 205, position.y, 75, position.height);
            var batchSize2DRect = new Rect(position.x + 300, position.y, 75, position.height);

            // Set variables.
            var type = Type.GetType(property.FindPropertyRelative("typeName").stringValue);
            var orderContent = new GUIContent(type.Name);
            var emptyContent = new GUIContent(" ");
            
            // Draw fields.
            EditorGUIUtility.labelWidth = 140f;
            GUI.enabled = type.Assembly != typeof(Physics2Dx).Assembly;
            EditorGUI.PropertyField(orderRect, property.FindPropertyRelative("order"), orderContent);
            GUI.enabled = true;

            EditorGUIUtility.labelWidth = 15f;
            EditorGUI.PropertyField(batchSize3DRect, property.FindPropertyRelative("batchSize3D"), emptyContent);
            EditorGUI.PropertyField(batchSize2DRect, property.FindPropertyRelative("batchSize2D"), emptyContent);

            // Set indent back to what it was.
            EditorGUI.indentLevel = indent;
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.EndProperty();
        }
    }

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
