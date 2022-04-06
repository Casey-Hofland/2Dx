#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Unity2Dx
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject, Type componentType) where T : Component => (T)gameObject.GetOrAddComponent(typeof(T), componentType);
        public static Component GetOrAddComponent(this GameObject gameObject, Type type, Type componentType)
        {
            return gameObject.TryGetComponent(type, out Component component)
                ? component
                : gameObject.AddComponent(componentType);
        }

        public static void ForceComponents<T>(this GameObject gameObject, List<T> results, params Type[] componentTypes)
            where T : Component
        {
            gameObject.GetComponents(results);

            // Destroy excess
            var missingComponentTypesCount = componentTypes.Length;
            for (int i = results.Count - 1; i >= 0; i--)
            {
                var result = results[i];

                var componentTypeIndex = Array.IndexOf(componentTypes, result.GetType(), 0, missingComponentTypesCount);
                if (componentTypeIndex == -1)
                {
                    Object.DestroyImmediate(result);
                    results.RemoveAt(i);
                }
                else
                {
                    componentTypes[componentTypeIndex] = componentTypes[missingComponentTypesCount - 1];
                    missingComponentTypesCount--;
                }
            }

            // Add missing
            results.Capacity = componentTypes.Length;
            for (int i = 0; i < missingComponentTypesCount; i++)
            {
                var componentType = componentTypes[i];
                var component = gameObject.AddComponent(componentType);
                results.Add((T)component);
            }
        }
    }
}
