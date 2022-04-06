#nullable enable
using UnityEditor;

namespace Unity2Dx.Editor
{
    [CustomEditor(typeof(WorldLock))]
    public sealed class WorldLockEditor : UnityEditor.Editor
    {
        private SerializedProperty? _rotateSkybox;
        public SerializedProperty rotateSkybox => _rotateSkybox ??= serializedObject.FindProperty($"<{nameof(WorldLock.rotateSkybox)}>k__BackingField");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (rotateSkybox.boolValue)
            {
                EditorGUILayout.HelpBox(
@$"Baked Scene Lighting cannot be rotated. If consistent lighting is important, open the Lighting window and make sure you have the following settings:
- You have no Baked Lightmaps (Realtime Lightmaps should be okay).
- Environment > Environment Lighting > Source is set to Color.
- Environment > Environment Reflections > Intensity Multiplier is set to 0.", MessageType.Warning);
            }
        }
    }
}
