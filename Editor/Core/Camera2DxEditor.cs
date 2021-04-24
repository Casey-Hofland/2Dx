using UnityEditor;
using UnityEngine;

namespace Unity2Dx.Editor
{
    [CustomEditor(typeof(Camera2Dx))]
    public class Camera2DxEditor : UnityEditor.Editor
    {
        private SerializedProperty _blendCurve;
        private SerializedProperty _fieldOfView;
        private SerializedProperty _usePhysicalProperties;
        private SerializedProperty _focalLength;
        private SerializedProperty _sensorSize;
        private SerializedProperty _lensShift;
        private SerializedProperty _gateFit;
        private SerializedProperty _orthographicSize;

        private void OnEnable()
        {
            _blendCurve = serializedObject.FindProperty(nameof(_blendCurve));
            _fieldOfView = serializedObject.FindProperty(nameof(_fieldOfView));
            _usePhysicalProperties = serializedObject.FindProperty(nameof(_usePhysicalProperties));
            _focalLength = serializedObject.FindProperty(nameof(_focalLength));
            _sensorSize = serializedObject.FindProperty(nameof(_sensorSize));
            _lensShift = serializedObject.FindProperty(nameof(_lensShift));
            _gateFit = serializedObject.FindProperty(nameof(_gateFit));
            _orthographicSize = serializedObject.FindProperty(nameof(_orthographicSize));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Set View
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel($"Set View");
            if(GUILayout.Button($"3D"))
            {
                foreach(Camera2Dx camera2Dx in targets)
                {
                    camera2Dx.SetView(false);
                }
            }
            if(GUILayout.Button($"2D"))
            {
                foreach(Camera2Dx camera2Dx in targets)
                {
                    camera2Dx.SetView(true);
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            // Blend Curve
            EditorGUILayout.PropertyField(_blendCurve);
            
            // Field Of View
            if(_usePhysicalProperties.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_fieldOfView);
                if(EditorGUI.EndChangeCheck())
                {
                    _focalLength.floatValue = Camera.FieldOfViewToFocalLength(_fieldOfView.floatValue, _sensorSize.vector2Value.y);
                    Debug.LogWarning($"Updated Focal Length might be incorrect if the camera's FOV Axis is set to Horizontal.");
                }
            }
            else
            {
                EditorGUILayout.PropertyField(_fieldOfView);
            }

            // Physical Properties
            EditorGUILayout.PropertyField(_usePhysicalProperties);
            if(_usePhysicalProperties.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_focalLength);
                _focalLength.floatValue = Mathf.Clamp(_focalLength.floatValue, 0.1047227f, 1.375099e+08f);

                EditorGUILayout.PropertyField(_sensorSize);
                EditorGUILayout.PropertyField(_lensShift);
                EditorGUILayout.PropertyField(_gateFit);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(_orthographicSize);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

