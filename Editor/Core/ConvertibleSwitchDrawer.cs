#nullable enable
using UnityEngine;
using UnityEditor;

namespace Unity2Dx.Editor
{
    [CustomPropertyDrawer(typeof(ConvertibleSwitchAttribute))]
    public class ConvertibleSwitchDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.Toggle(position, label, property.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Convertible Switch");
                var c = property.serializedObject.targetObject as IConvertible;
                c?.Convert(newValue);
            }
        }
    }
}
