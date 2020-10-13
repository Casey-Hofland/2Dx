using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

using Object = UnityEngine.Object;

namespace DimensionConverter.Editor
{
    internal class SettingsProvider : UnityEditor.SettingsProvider
    {
        private const string customPath = "Project/Dimension Converter";

        #region Asset Reference
        private static readonly string assetPath = $"Packages/com.caseydecoder.dimensionconverter/Runtime/Resources/{nameof(Settings)}.asset";

        //private static string _assetPath;
        //private static string assetPath
        //{
        //    get
        //    {
        //        if(_instance)
        //        {
        //            return _assetPath;
        //        }


        //        // Find the directory this script resides in.
        //        var typeName = nameof(Settings);

        //        var settings = AssetDatabase.LoadAssetAtPath<Settings>($"Packages/Dimension Converter/Runtime/{typeName}.asset");
        //        Debug.Log(settings);
        //        Debug.Log(typeof(Settings));
        //        return null;

        //        var guids = AssetDatabase.FindAssets($"{typeName} t:script", new[] { "Packages/Dimension Converter/Runtime" });
        //        var directory = (from guid in guids
        //                         let guidPath = AssetDatabase.GUIDToAssetPath(guid)
        //                         let startIndex = guidPath.LastIndexOf('/') + 1
        //                         where guidPath.Substring(startIndex, guidPath.IndexOf('.') - startIndex) == typeName
        //                         select guidPath.Remove(startIndex)).First();

        //        _assetPath = directory.Contains("Editor/") ? directory.Remove(directory.IndexOf("Editor/")) : directory;
        //        _assetPath += _assetPath.EndsWith("Resources/") ? string.Empty : "Resources/";
        //        _assetPath += $"{typeName}.asset";

        //        return _assetPath;
        //    }
        //}

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

                    AssetDatabase.CreateAsset(_instance, assetPath);
                    AssetDatabase.SaveAssets();
                }

                return _instance;
            }
        }

        private static SerializedObject GetSerializedObject() => new SerializedObject(instance);
        public static bool IsSettingsAvailable() => instance;
        private static IEnumerable<string> GetSearchKeywords() => GetSearchKeywordsFromPath(assetPath);
        #endregion

        private SerializedObject customSettings;
        private SerializedProperty is2DNot3D;
        private SerializedProperty defaultOutliner;
        private SerializedProperty conversionTime;
        private SerializedProperty batchConversion;
        private SerializedProperty convertersSettings;
        private ReorderableList reorderableList;

        public SettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            customSettings = GetSerializedObject();

            is2DNot3D = customSettings.FindProperty(nameof(is2DNot3D));
            defaultOutliner = customSettings.FindProperty(nameof(defaultOutliner));
            conversionTime = customSettings.FindProperty(nameof(conversionTime));
            batchConversion = customSettings.FindProperty(nameof(batchConversion));
            convertersSettings = customSettings.FindProperty(nameof(convertersSettings));

            CreateReorderableList();
        }

        private void CreateReorderableList()
        {
            customSettings.Update();

            // Get types and delete duplicates or null values.
            var convertersTypes = new HashSet<Type>();
            for(int i = convertersSettings.arraySize - 1; i >= 0; i--)
            {
                var converterSettings = convertersSettings.GetArrayElementAtIndex(i);
                var typeName = converterSettings.FindPropertyRelative("typeName").stringValue;
                var type = Type.GetType(typeName);
                if(type == null || type != typeof(Converter) || convertersTypes.Contains(type))
                {
                    convertersSettings.DeleteArrayElementAtIndex(i);
                }
                else
                {
                    convertersTypes.Add(type);
                }
            }

            // Get all Converter types in the project.
            var converterTypes = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in GetLoadableTypes(assembly)
                                 where type.IsSubclassOf(typeof(Converter))
                                 where !type.IsAbstract
                                 select type;

            // Populate the SerializedProperty array.
            foreach(var converterType in converterTypes)
            {
                if(!convertersTypes.Contains(converterType))
                {
                    var arraySize = convertersSettings.arraySize;
                    convertersSettings.arraySize++;
                    var converterSettings = convertersSettings.GetArrayElementAtIndex(arraySize);
                    converterSettings.FindPropertyRelative("typeName").stringValue = converterType.AssemblyQualifiedName;
                    var batchSize3D = converterSettings.FindPropertyRelative("batchSize3D");
                    var batchSize2D = converterSettings.FindPropertyRelative("batchSize2D");

                    int index;
                    switch(converterType.Name)
                    {
                        case nameof(SplitterConverter):
                            batchSize3D.intValue = batchSize2D.intValue = 1000;
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(0, arraySize));
                            break;
                        case nameof(SphereConverter):
                            batchSize3D.intValue = batchSize2D.intValue = 100;
                            index = FindIndex(convertersSettings, element => element.FindPropertyRelative("typeName").stringValue == typeof(SplitterConverter).AssemblyQualifiedName) + 1;
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(CapsuleConverter):
                            batchSize3D.intValue = batchSize2D.intValue = 100;
                            index = FindIndex(convertersSettings, element => element.FindPropertyRelative("typeName").stringValue == typeof(SphereConverter).AssemblyQualifiedName) + 1;
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(BoxConverter):
                            batchSize3D.intValue = batchSize2D.intValue = 50;
                            index = FindIndex(convertersSettings, element => element.FindPropertyRelative("typeName").stringValue == typeof(CapsuleConverter).AssemblyQualifiedName) + 1;
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(MeshConverter):
                            batchSize3D.intValue = batchSize2D.intValue = 20;
                            index = FindIndex(convertersSettings, element => element.FindPropertyRelative("typeName").stringValue == typeof(BoxConverter).AssemblyQualifiedName) + 1;
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(RigidbodyConverter):
                            batchSize3D.intValue = batchSize2D.intValue = 50;
                            index = FindIndex(convertersSettings, element => element.FindPropertyRelative("typeName").stringValue == typeof(MeshConverter).AssemblyQualifiedName) + 1;
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        default:
                            batchSize3D.intValue = batchSize2D.intValue = 100;
                            break;
                    }
                }
            }

            customSettings.ApplyModifiedProperties();

            // Create the Reorderable List.
            reorderableList = new ReorderableList(customSettings, convertersSettings, true, true, false, false)
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
                tooltip = "The order in which Converters are converted and in how large of a batch.",
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
            if(type.Assembly == typeof(Dimension).Assembly)
            {
                reorderableList.draggable = false;
            }
        }

        public override void OnGUI(string searchContext)
        {
            customSettings.Update();

            EditorGUILayout.PropertyField(is2DNot3D);
            EditorGUILayout.PropertyField(defaultOutliner);
            EditorGUILayout.PropertyField(conversionTime);
            EditorGUILayout.PropertyField(batchConversion);

            if(Event.current.type == EventType.MouseUp)
            {
                reorderableList.draggable = true;
            }

            reorderableList.DoLayoutList();

            if(GUILayout.Button("Reset"))
            {
                var tempInstance = ScriptableObject.CreateInstance<Settings>();
                _instance.is2DNot3D = tempInstance.is2DNot3D;
                _instance.defaultOutliner = tempInstance.defaultOutliner;
                _instance.conversionTime = tempInstance.conversionTime;
                _instance.batchConversion = tempInstance.batchConversion;
                _instance.convertersSettings = tempInstance.convertersSettings;
                Object.DestroyImmediate(tempInstance, true);

                CreateReorderableList();
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
