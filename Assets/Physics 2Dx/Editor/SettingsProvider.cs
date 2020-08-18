using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Physics2DxSystem.Editor
{
    internal class SettingsProvider : UnityEditor.SettingsProvider
    {
        private const string customPath = "Project/Physics 2Dx";

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
                var typeName = nameof(Settings);
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

        private static Settings _instance;
        private static Settings instance
        {
            get
            {
                if(
                    (!_instance && !(_instance = AssetDatabase.LoadAssetAtPath<Settings>(assetPath))) ||
                    (_instance && AssetDatabase.GetAssetPath(_instance) != assetPath))
                {
                    _instance = ScriptableObject.CreateInstance<Settings>();

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
                    _instance.Reset();
                    AssetDatabase.SaveAssets();
                }

                return _instance;
            }
        }

        private static SerializedObject GetSerializedObject() => new SerializedObject(instance);

        public static bool IsSettingsAvailable()
        {
            return instance;
        }

        private static IEnumerable<string> GetSearchKeywords() => GetSearchKeywordsFromPath(assetPath);
        #endregion

        private static bool editorSettingsFoldout = false;

        private SerializedObject customSettings;
        private SerializedProperty is2DNot3D;
        private SerializedProperty conversionTime;
        private SerializedProperty splitConversion;
        private SerializedProperty slimHierarchy;
        private SerializedProperty module2DxesSettings;
        private SerializedProperty camera2DxLiveUpdate;
        private ReorderableList reorderableList;

        public SettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            customSettings = GetSerializedObject();

            is2DNot3D = customSettings.FindProperty(nameof(is2DNot3D));
            conversionTime = customSettings.FindProperty(nameof(conversionTime));
            splitConversion = customSettings.FindProperty(nameof(splitConversion));
            slimHierarchy = customSettings.FindProperty(nameof(slimHierarchy));
            module2DxesSettings = customSettings.FindProperty(nameof(module2DxesSettings));
            camera2DxLiveUpdate = customSettings.FindProperty(nameof(camera2DxLiveUpdate));

            CreateReorderableList();
        }

        private void CreateReorderableList()
        {
            // Get types and delete duplicates or null values.
            var module2DxesTypes = new HashSet<Type>();
            for(int i = module2DxesSettings.arraySize - 1; i >= 0; i--)
            {
                var module2DxSettings = module2DxesSettings.GetArrayElementAtIndex(i);
                var typeName = module2DxSettings.FindPropertyRelative("typeName").stringValue;
                var type = Type.GetType(typeName);
                if(type == null || module2DxesTypes.Contains(type))
                {
                    module2DxesSettings.DeleteArrayElementAtIndex(i);
                }
                else
                {
                    module2DxesTypes.Add(type);
                }
            }

            // Get all Module2Dx types in the project.
            var module2DxTypes = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in GetLoadableTypes(assembly)
                                 where type.IsSubclassOf(typeof(Module2Dx))
                                 where !type.IsAbstract
                                 select type;

            // Populate the SerializedProperty array.
            foreach(var module2DxType in module2DxTypes)
            {
                if(!module2DxesTypes.Contains(module2DxType))
                {
                    var arraySize = module2DxesSettings.arraySize;
                    module2DxesSettings.arraySize++;
                    var module2DxSettings = module2DxesSettings.GetArrayElementAtIndex(arraySize);
                    module2DxSettings.FindPropertyRelative("typeName").stringValue = module2DxType.AssemblyQualifiedName;
                    var batchSize3D = module2DxSettings.FindPropertyRelative("batchSize3D");
                    var batchSize2D = module2DxSettings.FindPropertyRelative("batchSize2D");

                    int index;
                    switch(module2DxType.Name)
                    {
                        case nameof(Transform2Dx):
                            batchSize3D.intValue = batchSize2D.intValue = 1000;
                            module2DxesSettings.MoveArrayElement(arraySize, Math.Min(0, arraySize));
                            break;
                        case nameof(SphereConverter):
                            batchSize3D.intValue = batchSize2D.intValue = 100;
                            index = FindIndex(module2DxesSettings, element => element.FindPropertyRelative("typeName").stringValue == typeof(Transform2Dx).AssemblyQualifiedName) + 1;
                            module2DxesSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(CapsuleConverter):
                            batchSize3D.intValue = batchSize2D.intValue = 100;
                            index = FindIndex(module2DxesSettings, element => element.FindPropertyRelative("typeName").stringValue == typeof(SphereConverter).AssemblyQualifiedName) + 1;
                            module2DxesSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(BoxConverter):
                            batchSize3D.intValue = batchSize2D.intValue = 50;
                            index = FindIndex(module2DxesSettings, element => element.FindPropertyRelative("typeName").stringValue == typeof(CapsuleConverter).AssemblyQualifiedName) + 1;
                            module2DxesSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(MeshConverter):
                            batchSize3D.intValue = batchSize2D.intValue = 20;
                            index = FindIndex(module2DxesSettings, element => element.FindPropertyRelative("typeName").stringValue == typeof(BoxConverter).AssemblyQualifiedName) + 1;
                            module2DxesSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        default:
                            batchSize3D.intValue = batchSize2D.intValue = 100;
                            break;
                    }
                }
            }
            customSettings.ApplyModifiedProperties();

            // Create the Reorderable List.
            reorderableList = new ReorderableList(customSettings, module2DxesSettings, true, true, false, false)
            {
                drawHeaderCallback = DrawListHeader,
                drawElementCallback = DrawListElement,
                onSelectCallback = SelectList,
            };
        }

        private int FindIndex(SerializedProperty serializedProperty, Func<SerializedProperty, bool> predicate)
        {
            for(int i = 0; i < serializedProperty.arraySize; i++)
            {
                var element = serializedProperty.GetArrayElementAtIndex(i);
                if(predicate.Invoke(element))
                {
                    return i;
                }
            }

            return -1;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly?.GetTypes();
            }
            catch(ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        private void DrawListHeader(Rect rect)
        {
            GUIContent header = new GUIContent
            {
                text = "Conversion Order",
                tooltip = "The order in which Module2Dxes are converted and in how large of a batch.",
            };

            EditorGUI.LabelField(rect, header);
        }

        private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element);
        }

        private void SelectList(ReorderableList reorderableList)
        {
            var selectedElement = reorderableList.serializedProperty.GetArrayElementAtIndex(reorderableList.index);

            var type = Type.GetType(selectedElement.FindPropertyRelative("typeName").stringValue);
            if(type.Assembly == typeof(Physics2Dx).Assembly)
            {
                reorderableList.draggable = false;
            }
        }

        public override void OnGUI(string searchContext)
        {
            customSettings.Update();

            EditorGUILayout.PropertyField(is2DNot3D);
            EditorGUILayout.PropertyField(conversionTime);
            EditorGUILayout.PropertyField(splitConversion);

            if(Event.current.type == EventType.MouseUp)
            {
                reorderableList.draggable = true;
            }

            reorderableList.DoLayoutList();

            if(editorSettingsFoldout = EditorGUILayout.Foldout(editorSettingsFoldout, "Editor Settings"))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(slimHierarchy);
                EditorGUILayout.PropertyField(camera2DxLiveUpdate);
                EditorGUI.indentLevel--;
            }

            if(GUILayout.Button("Reset"))
            {
                var physics2DxSettings = (Settings)customSettings.targetObject;
                physics2DxSettings.Reset();
                EditorApplication.delayCall += CreateReorderableList;
            }

            customSettings.ApplyModifiedProperties();
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static UnityEditor.SettingsProvider CreateMyCustomSettingsProvider()
        {
            // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
            return !IsSettingsAvailable()
                ? null
                : new SettingsProvider(customPath, SettingsScope.Project, GetSearchKeywords());
        }
    }
}
