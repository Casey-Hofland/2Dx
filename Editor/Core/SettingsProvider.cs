using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity2Dx.Physics;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

using Object = UnityEngine.Object;

namespace Unity2Dx.Editor
{
    internal class SettingsProvider : UnityEditor.SettingsProvider
    {
        private const string customPath = "Project/2Dx";
        private const string typeNamePropertyPath = "typeName";

        #region Asset Reference
        private static readonly string settingsAssetPath = $"Packages/com.caseydecoder.2dx/Runtime/Core/Settings/Resources/{nameof(Settings)}.asset";

        private static Settings _settings;
        private static Settings settings
        {
            get
            {
                if(
                    (!_settings && !(_settings = AssetDatabase.LoadAssetAtPath<Settings>(settingsAssetPath))) ||
                    (_settings && AssetDatabase.GetAssetPath(_settings) != settingsAssetPath))
                {
                    _settings = ScriptableObject.CreateInstance<Settings>();

                    AssetDatabase.CreateAsset(_settings, settingsAssetPath);
                    AssetDatabase.SaveAssets();
                }

                return _settings;
            }
        }

        private static SerializedObject GetSerializedSettings() => new SerializedObject(settings);
        public static bool IsSettingsAvailable() => settings;
        private static IEnumerable<string> GetSettingsSearchKeywords() => GetSearchKeywordsFromPath(settingsAssetPath);

        private static readonly string physicsSettingsAssetPath = $"Packages/com.caseydecoder.2dx/Runtime/Physics/Settings/Resources/{nameof(Physics.Settings)}.asset";

        private static Physics.Settings _physicsSettings;
        private static Physics.Settings physicsSettings
        {
            get
            {
                if (
                    (!_physicsSettings && !(_physicsSettings = AssetDatabase.LoadAssetAtPath<Physics.Settings>(physicsSettingsAssetPath))) ||
                    (_physicsSettings && AssetDatabase.GetAssetPath(_physicsSettings) != physicsSettingsAssetPath))
                {
                    _physicsSettings = ScriptableObject.CreateInstance<Physics.Settings>();

                    AssetDatabase.CreateAsset(_physicsSettings, physicsSettingsAssetPath);
                    AssetDatabase.SaveAssets();
                }

                return _physicsSettings;
            }
        }

        private static SerializedObject GetSerializedPhysicsSettings() => new SerializedObject(physicsSettings);
        public static bool IsPhysicsSettingsAvailable() => physicsSettings;
        private static IEnumerable<string> GetPhysicsSettingsSearchKeywords() => GetSearchKeywordsFromPath(physicsSettingsAssetPath);
        #endregion

        private SerializedObject customSettings;
        private SerializedObject customPhysicsSettings;
        private SerializedProperty is2DNot3D;
        private SerializedProperty defaultOutliner;
        private SerializedProperty conversionTime;
        private SerializedProperty conversionTimeScale;
        private SerializedProperty batchConversion;
        private SerializedProperty convertersSettings;
        private ReorderableList reorderableList;

        public SettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            customSettings = GetSerializedSettings();
            is2DNot3D = customSettings.FindProperty(nameof(is2DNot3D));
            conversionTime = customSettings.FindProperty(nameof(conversionTime));
            conversionTimeScale = customSettings.FindProperty(nameof(conversionTimeScale));
            batchConversion = customSettings.FindProperty(nameof(batchConversion));
            convertersSettings = customSettings.FindProperty(nameof(convertersSettings));

            customPhysicsSettings = GetSerializedPhysicsSettings();
            defaultOutliner = customPhysicsSettings.FindProperty(nameof(defaultOutliner));

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
                var typeName = converterSettings.FindPropertyRelative(typeNamePropertyPath).stringValue;
                var type = Type.GetType(typeName);
                if(type == null || !type.IsSubclassOf(typeof(Converter)) || convertersTypes.Contains(type))
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
                    converterSettings.FindPropertyRelative(typeNamePropertyPath).stringValue = converterType.AssemblyQualifiedName;
                    var batchSize3D = converterSettings.FindPropertyRelative(nameof(ConverterSettings.batchSize3D));
                    var batchSize2D = converterSettings.FindPropertyRelative(nameof(ConverterSettings.batchSize2D));

                    int index;
                    switch(converterType.Name)
                    {
                        case nameof(SphereCollider2Dx):
                            batchSize3D.intValue = batchSize2D.intValue = 100;
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(0, arraySize));
                            break;
                        case nameof(CapsuleCollider2Dx):
                            batchSize3D.intValue = batchSize2D.intValue = 100;
                            index = FindConverterIndex(typeof(SphereCollider2Dx));
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(BoxCollider2Dx):
                            batchSize3D.intValue = batchSize2D.intValue = 50;
                            index = FindConverterIndex(typeof(CapsuleCollider2Dx), typeof(SphereCollider2Dx));
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(MeshCollider2Dx):
                            batchSize3D.intValue = batchSize2D.intValue = 20;
                            index = FindConverterIndex(typeof(BoxCollider2Dx), typeof(CapsuleCollider2Dx), typeof(SphereCollider2Dx));
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(ConstantForce2Dx):
                            batchSize3D.intValue = batchSize2D.intValue = 100;
                            index = FindConverterIndex(typeof(MeshCollider2Dx), typeof(BoxCollider2Dx), typeof(CapsuleCollider2Dx), typeof(SphereCollider2Dx));
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        case nameof(Rigidbody2Dx):
                            batchSize3D.intValue = batchSize2D.intValue = 50;
                            index = FindConverterIndex(typeof(ConstantForce2Dx), typeof(MeshCollider2Dx), typeof(BoxCollider2Dx), typeof(CapsuleCollider2Dx), typeof(SphereCollider2Dx));
                            convertersSettings.MoveArrayElement(arraySize, Math.Min(index, arraySize));
                            break;
                        default:
                            batchSize3D.intValue = batchSize2D.intValue = 100;
                            break;
                    }

                    int FindConverterIndex(params Type[] converterTypes) 
                    {
                        for(int i = 0; i < converterTypes.Length; i++)
                        {
                            var converterType = converterTypes[i];
                            var index = FindIndex(convertersSettings, element => element.FindPropertyRelative(typeNamePropertyPath).stringValue == converterType.AssemblyQualifiedName) + 1;

                            if(index > 0)
                            {
                                return index;
                            }
                        }

                        return 0;
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
            customPhysicsSettings.Update();

            EditorGUILayout.PropertyField(is2DNot3D);
            EditorGUILayout.PropertyField(defaultOutliner);
            EditorGUILayout.PropertyField(conversionTime);
            EditorGUILayout.PropertyField(conversionTimeScale);
            EditorGUILayout.PropertyField(batchConversion);

            if(Event.current.type == EventType.MouseUp)
            {
                reorderableList.draggable = true;
            }

            reorderableList.DoLayoutList();

            if(GUILayout.Button("Reset"))
            {
                var tempSettings = ScriptableObject.CreateInstance<Settings>();
                _settings.is2DNot3D = tempSettings.is2DNot3D;
                _settings.conversionTime = tempSettings.conversionTime;
                _settings.batchConversion = tempSettings.batchConversion;
                _settings.convertersSettings = tempSettings.convertersSettings;
                Object.DestroyImmediate(tempSettings, true);

                var tempPhysicsSettings = ScriptableObject.CreateInstance<Physics.Settings>();
                _physicsSettings.defaultOutliner = tempPhysicsSettings.defaultOutliner;
                Object.DestroyImmediate(tempPhysicsSettings, true);

                CreateReorderableList();
            }

            customSettings.ApplyModifiedProperties();
            customPhysicsSettings.ApplyModifiedProperties();
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static UnityEditor.SettingsProvider CreateMyCustomSettingsProvider()
        {
            // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
            return !IsSettingsAvailable() || !IsPhysicsSettingsAvailable()
                ? null
                : new SettingsProvider(customPath, SettingsScope.Project, GetSettingsSearchKeywords().Concat(GetPhysicsSettingsSearchKeywords()));
        }
    }
}
