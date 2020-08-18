using System;
using UnityEditor;
using UnityEngine;

namespace Physics2DxSystem.Editor
{
    [CustomEditor(typeof(Camera2Dx))]
    public class Camera2DxEditor : UnityEditor.Editor
    {
        Camera2Dx[] camera2Dxes => Array.ConvertAll(targets, target => (Camera2Dx)target);
        
        SerializedProperty blendCurve;
        SerializedProperty fieldOfView;
        SerializedProperty usePhysicalProperties;
        SerializedProperty focalLength;
        SerializedProperty sensorSize;
        SerializedProperty lensShift;
        SerializedProperty gateFit;
        SerializedProperty orthographicSize;

        private void OnEnable()
        {
            blendCurve = serializedObject.FindProperty(nameof(blendCurve));
            fieldOfView = serializedObject.FindProperty(nameof(fieldOfView));
            usePhysicalProperties = serializedObject.FindProperty(nameof(usePhysicalProperties));
            focalLength = serializedObject.FindProperty(nameof(focalLength));
            sensorSize = serializedObject.FindProperty(nameof(sensorSize));
            lensShift = serializedObject.FindProperty(nameof(lensShift));
            gateFit = serializedObject.FindProperty(nameof(gateFit));
            orthographicSize = serializedObject.FindProperty(nameof(orthographicSize));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(blendCurve);
            
            // Field Of View
            if(usePhysicalProperties.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(fieldOfView);
                if(EditorGUI.EndChangeCheck())
                {
                    focalLength.floatValue = Camera.FieldOfViewToFocalLength(fieldOfView.floatValue, sensorSize.vector2Value.y);
                    Debug.LogWarning($"Updated Focal Length might be incorrect if the camera's FOV Axis is set to Horizontal.");
                }
            }
            else
            {
                EditorGUILayout.PropertyField(fieldOfView);
            }

            // Physical Properties
            EditorGUILayout.PropertyField(usePhysicalProperties);
            if(usePhysicalProperties.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(focalLength);
                focalLength.floatValue = Mathf.Clamp(focalLength.floatValue, 0.1047227f, 1.375099e+08f);

                EditorGUILayout.PropertyField(sensorSize);
                EditorGUILayout.PropertyField(lensShift);
                EditorGUILayout.PropertyField(gateFit);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(orthographicSize);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

