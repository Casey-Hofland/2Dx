using UnityEditor;
using UnityEngine;

namespace Unity2Dx.Editor
{
    public static class ReferenceUtility
    {
        public static void ChangeReferences(Object referenced, Object newReference)
        {
            var allObjects = Object.FindObjectsOfType<GameObject>();

            for(int j = 0; j < allObjects.Length; j++)
            {
                var gameObject = allObjects[j];
                var components = gameObject.GetComponents<Component>();
                for(int i = 0; i < components.Length; i++)
                {
                    var component = components[i];
                    var serializedObject = new SerializedObject(component);
                    var serializedProperty = serializedObject.GetIterator();

                    while(serializedProperty.NextVisible(true))
                    {
                        if(serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            if(serializedProperty.objectReferenceValue == referenced)
                            {
                                serializedProperty.objectReferenceValue = newReference;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }
            }
        }
    }
}

