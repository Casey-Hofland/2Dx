using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Physics2DxSystem.Editor
{
    public class Physics2DxSettingsProviderOld : SettingsProvider
    {
        private const string customPath = "Project/Physics 2Dx Old";

        #region Asset Reference
        private static string _assetPath;
        private static string assetPath
        {
            get
            {
                if(_instance)
                {
                    return _assetPath;
                }

                // Find the directory this script resides in.
                var typeName = nameof(Physics2DxSettings);
                var guids = AssetDatabase.FindAssets($"{typeName} t:script");
                var directory = (from guid in guids
                                 let guidPath = AssetDatabase.GUIDToAssetPath(guid)
                                 let startIndex = guidPath.LastIndexOf('/') + 1
                                 where guidPath.Substring(startIndex, guidPath.IndexOf('.') - startIndex) == typeName
                                 select guidPath.Remove(startIndex)).First();

                _assetPath = directory.Contains("Editor/") ? directory.Remove(directory.IndexOf("Editor/")) : directory;
                _assetPath += _assetPath.EndsWith("Resources/") ? string.Empty : "Resources/";
                _assetPath += $"{typeName}.asset";

                return _assetPath;
            }
        }

        private static Physics2DxSettings _instance;
        internal static Physics2DxSettings instance
        {
            get
            {
                if(
                    (!_instance && !(_instance = AssetDatabase.LoadAssetAtPath<Physics2DxSettings>(assetPath))) ||
                    (_instance && AssetDatabase.GetAssetPath(_instance) != assetPath))
                {
                    _instance = ScriptableObject.CreateInstance<Physics2DxSettings>();
                    
                    var directory = assetPath.Remove(assetPath.LastIndexOf('/'));
                    if(!AssetDatabase.IsValidFolder(directory))
                    {
                        var folders = directory.Split('/');
                        var currentPath = folders[0]; // First folder will always be Assets.
                        for(int f = 1; f < folders.Length; f++)
                        {
                            var folder = folders[f];
                            var newPath = currentPath + '/' + folder;
                            if(!AssetDatabase.IsValidFolder(newPath))
                            {
                                AssetDatabase.CreateFolder(currentPath, folder);
                            }

                            currentPath = newPath;
                        }
                    }

                    AssetDatabase.CreateAsset(_instance, assetPath);
                    AssetDatabase.SaveAssets();
                }

                return _instance;
            }
        }
        #endregion

        private SerializedObject customSettings;
        private VisualElement rootElement;

        public Physics2DxSettingsProviderOld(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        public static bool IsSettingsAvailable()
        {
            return instance;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            customSettings = new SerializedObject(instance);
            this.rootElement = rootElement;

            // Draw the title.
            var title = new Label(label);
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 19;
            title.style.marginLeft = 9;
            title.style.marginTop = 1;
            title.style.marginBottom = 8;
            title.AddToClassList("title");
            rootElement.Add(title);

            // Draw all (supported) properties.
            var property = customSettings.GetIterator();
            VisualElement element;
            property.NextVisible(true); // Skip the script property.
            while(property.NextVisible(true))
            {
                switch(property.propertyType)
                {
                    case SerializedPropertyType.Boolean:
                        element = new Toggle(property.displayName);
                        break;
                    case SerializedPropertyType.Float:
                        element = new FloatField(property.displayName);
                        break;
                    default:
                        continue;
                }

                if(element is IBindable bindable)
                {
                    bindable.BindProperty(property);
                }
                if(typeof(Physics2DxSettings).GetField(property.name).GetCustomAttributes(typeof(TooltipAttribute), true).FirstOrDefault() is TooltipAttribute attribute)
                {
                    element.tooltip = attribute.tooltip;
                }

                element.SetEnabled(property.editable);
                rootElement.Add(element);
            }

            if(Application.isPlaying)
            {
                AddPlayingWarning();
            }
            else
            {
                EditorApplication.playModeStateChanged += TogglePlayingWarning;
            }
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            EditorApplication.playModeStateChanged -= TogglePlayingWarning;
            if(rootElement != null)
            {
                rootElement.SetEnabled(true);
            }
            rootElement = null;
        }

        private void TogglePlayingWarning(PlayModeStateChange playModeState)
        {
            if(playModeState == PlayModeStateChange.EnteredPlayMode)
            {
                AddPlayingWarning();
            }
        }

        private void AddPlayingWarning()
        {
            var playingHelpBox = new IMGUIContainer(() => EditorGUILayout.HelpBox($"{nameof(Physics2DxSettings)} can't be edited in playmode.", MessageType.Warning));
            rootElement.Add(playingHelpBox);
            rootElement.SetEnabled(false);
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            return null;

            // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
            return !IsSettingsAvailable()
                ? null
                : new Physics2DxSettingsProviderOld(customPath, SettingsScope.Project, GetSearchKeywordsFromPath(assetPath));
        }
    }
}
