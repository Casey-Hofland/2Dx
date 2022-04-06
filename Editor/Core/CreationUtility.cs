#nullable enable
using UnityEditor;
using UnityEngine;

namespace Unity2Dx.Editor
{
    public static class CreationUtility
    {
        [MenuItem("GameObject/Camera 2Dx", false, 11)]
        public static void CreateCamera2Dx(MenuCommand? menuCommand)
        {
            EditorApplication.ExecuteMenuItem("GameObject/Camera");

            Selection.activeObject.name = "Camera 2Dx";

            GameObjectUtility.SetParentAndAlign(Selection.activeGameObject, menuCommand?.context as GameObject);
            GameObjectUtility.EnsureUniqueNameForSibling(Selection.activeGameObject);

            Undo.AddComponent<Camera2Dx>(Selection.activeGameObject);
        }

        private static AudioTransform2Dx CreateAudioTransform2Dx(string name, GameObject? parent)
        {
            var audioGameObject = CreateObject(name, parent);
            return Undo.AddComponent<AudioTransform2Dx>(audioGameObject);
        }

        [MenuItem("GameObject/Audio/Audio Source 2Dx")]
        public static void CreateAudioSource2Dx(MenuCommand? menuCommand)
        {
            var audioTransform2Dx = CreateAudioTransform2Dx("Audio Source 2Dx", menuCommand?.context as GameObject);
            Undo.AddComponent<AudioSource>(audioTransform2Dx.gameObject2D);
        }

        [MenuItem("GameObject/Audio/Audio Reverb Zone 2Dx")]
        public static void CreateReverbZone2Dx(MenuCommand? menuCommand)
        {
            var audioTransform2Dx = CreateAudioTransform2Dx("Audio Reverb Zone 2Dx", menuCommand?.context as GameObject);
            Undo.AddComponent<AudioReverbZone>(audioTransform2Dx.gameObject2D);
        }

        public static GameObject CreateObject(string name, GameObject? parent)
        {
            var gameObject = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(gameObject, nameof(CreateObject));

            GameObjectUtility.SetParentAndAlign(gameObject, parent);
            GameObjectUtility.EnsureUniqueNameForSibling(gameObject);

            PlaceCreatedObject(gameObject);
            Selection.activeGameObject = gameObject;

            return gameObject;
        }

        public static void PlaceCreatedObject(GameObject gameObject)
        {
            var placeAtWorldOrigin = EditorPrefs.GetBool("Create3DObject.PlaceAtWorldOrigin");
            gameObject.transform.position = placeAtWorldOrigin ? Vector3.zero : SceneView.lastActiveSceneView.pivot;
        }
    }
}
