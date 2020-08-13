using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
using UnityEditorInternal;
using System.Reflection;

namespace Physics2DxSystem.Editor
{
    public class Physics2DxSettingsProvider : SettingsProvider
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
        private static Physics2DxSettings instance
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

        private static SerializedObject GetSerializedObject() => new SerializedObject(instance);

        public static bool IsSettingsAvailable()
        {
            return instance;
        }

        private static IEnumerable<string> GetSearchKeywords() => GetSearchKeywordsFromPath(assetPath);
        #endregion

        private SerializedObject customSettings;
        private SerializedProperty is2Dnot3D;
        private SerializedProperty conversionTime;
        private SerializedProperty splitConversion;
        private SerializedProperty slimHierarchy;
        private SerializedProperty module2DxesSettings;
        private ReorderableList reorderableList;

        public Physics2DxSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }
        
        //public override void OnActivate(string searchContext, VisualElement rootElement)
        //{
        //    customSettings = GetSerializedObject();

        //    var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Physics 2Dx/Editor/Physics2DxSettingsProvider.uxml");
        //    visualTreeAsset.CloneTree(rootElement);

        //    rootElement.Bind(customSettings);

        //    var conversionOrder = rootElement.Q<ListView>("conversionOrder", "unity-list-view");

        //    var list = new List<string>()
        //    {
        //        "Lol",
        //        "Dope",
        //        "Cool",
        //    };

        //    conversionOrder.makeItem = () => new Label();
        //    conversionOrder.bindItem = (element, index) => (element as Label).text = list[index];
        //    conversionOrder.itemsSource = list;

        //    conversionOrder.pickingMode = PickingMode.Position;
        //    conversionOrder.selectionType = SelectionType.Single;

        //    conversionOrder.onItemChosen += obj => Debug.Log(obj);

        //    conversionOrder.style.flexGrow = 1f;
        //}

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            customSettings = GetSerializedObject();

            is2Dnot3D = customSettings.FindProperty(nameof(is2Dnot3D));
            conversionTime = customSettings.FindProperty(nameof(conversionTime));
            splitConversion = customSettings.FindProperty(nameof(splitConversion));
            slimHierarchy = customSettings.FindProperty(nameof(slimHierarchy));
            module2DxesSettings = customSettings.FindProperty(nameof(module2DxesSettings));

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

            var module2DxTypes = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 where assembly != typeof(Physics2Dx).Assembly
                                 from type in GetLoadableTypes(assembly)
                                 where type.IsSubclassOf(typeof(Module2Dx))
                                 where !type.IsAbstract
                                 select type;

            foreach(var module2DxType in module2DxTypes)
            {
                if(!module2DxesTypes.Contains(module2DxType))
                {
                    module2DxesSettings.arraySize++;
                    var module2DxSettings = module2DxesSettings.GetArrayElementAtIndex(module2DxesSettings.arraySize - 1);
                    module2DxSettings.FindPropertyRelative("typeName").stringValue = module2DxType.AssemblyQualifiedName;
                    module2DxSettings.FindPropertyRelative("order").intValue = 0;
                    module2DxSettings.FindPropertyRelative("batchSize3D").intValue = 100;
                    module2DxSettings.FindPropertyRelative("batchSize2D").intValue = 100;
                }
            }

            customSettings.ApplyModifiedProperties();

            reorderableList = new ReorderableList(customSettings, module2DxesSettings, false, true, false, false)
            {
                drawHeaderCallback = DrawListHeader,
                drawElementCallback = DrawListElement,
            };
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if(assembly == null)
            {
                return null;
            }

            try
            {
                return assembly.GetTypes();
            }
            catch(ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        private void DrawListHeader(Rect rect)
        {
            var typeRect = new Rect(rect.x, rect.y, 140, rect.height);
            var orderRect = new Rect(rect.x + 140, rect.y, 60, rect.height);
            var batchSize3DRect = new Rect(rect.x + 205, rect.y, 90, rect.height);
            var batchSize2DRect = new Rect(rect.x + 300, rect.y, 90, rect.height);

            EditorGUI.LabelField(typeRect, "Module2Dx Type");
            EditorGUI.LabelField(orderRect, "Order");
            EditorGUI.LabelField(batchSize3DRect, "Batch Size 3D");
            EditorGUI.LabelField(batchSize2DRect, "Batch Size 2D");
        }

        private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, element);
            if(EditorGUI.EndChangeCheck())
            {
                var sortedIndexes = new SortedList<int, Module2DxSettings>(module2DxesSettings.arraySize);
                for(int i = 0; i < module2DxesSettings.arraySize; i++)
                {
                    var module2DxSettings = module2DxesSettings.GetArrayElementAtIndex(i);
                    var order = module2DxSettings.FindPropertyRelative("order").intValue;
                    var type = Type.GetType(module2DxSettings.FindPropertyRelative("typeName").stringValue);

                    if(type.Assembly != typeof(Physics2Dx).Assembly)
                    {
                        while(sortedIndexes.ContainsKey(order))
                        {
                            order++;
                        }
                    }
                    else
                    {
                        if(sortedIndexes.ContainsKey(order))
                        {
                            var sortedModule2Dx = sortedIndexes[order];
                            sortedIndexes.Remove(order);
                            do
                            {
                                sortedModule2Dx.order++;
                            } while(sortedIndexes.ContainsKey(sortedModule2Dx.order));
                            sortedIndexes.Add(sortedModule2Dx.order, sortedModule2Dx);
                        }
                    }

                    var module2DxSettingsCopy = new Module2DxSettings
                    {
                        order = order,
                        type = type,
                        batchSize3D = (uint)module2DxSettings.FindPropertyRelative("batchSize3D").intValue,
                        batchSize2D = (uint)module2DxSettings.FindPropertyRelative("batchSize2D").intValue,
                    };

                    sortedIndexes.Add(order, module2DxSettingsCopy);
                }
                var module2DxSettingsCopies = sortedIndexes.Values;

                for(int i = 0; i < module2DxesSettings.arraySize; i++)
                {
                    var module2DxSettingsCopy = module2DxSettingsCopies[i];
                    var module2DxSettings = module2DxesSettings.GetArrayElementAtIndex(i);

                    module2DxSettings.FindPropertyRelative("order").intValue = module2DxSettingsCopy.order;
                    module2DxSettings.FindPropertyRelative("typeName").stringValue = module2DxSettingsCopy.type.AssemblyQualifiedName;
                    module2DxSettings.FindPropertyRelative("batchSize3D").intValue = (int)module2DxSettingsCopy.batchSize3D;
                    module2DxSettings.FindPropertyRelative("batchSize2D").intValue = (int)module2DxSettingsCopy.batchSize2D;
                }
                customSettings.ApplyModifiedProperties();
            }
        }

        public override void OnGUI(string searchContext)
        {
            customSettings.Update();

            EditorGUILayout.PropertyField(is2Dnot3D);
            EditorGUILayout.PropertyField(conversionTime);
            EditorGUILayout.PropertyField(splitConversion);
            EditorGUILayout.PropertyField(slimHierarchy);

            reorderableList.DoLayoutList();

            customSettings.ApplyModifiedProperties();
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
            return !IsSettingsAvailable()
                ? null
                : new Physics2DxSettingsProvider(customPath, SettingsScope.Project, GetSearchKeywords());
        }
    }
}
