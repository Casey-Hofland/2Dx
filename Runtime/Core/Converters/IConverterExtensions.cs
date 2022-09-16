#nullable enable
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Unity2Dx
{
    public static class IConverterExtensions
    {
        public static IConvertible? GetRootConvertible(this IConverter converter)
        {
            while (!(converter is IConvertible))
            {
                converter = converter.converter;
            }
            return (IConvertible)converter;
        }

        public static bool TryGetRootConvertible(this IConverter converter, out IConvertible? convertible)
        {
            try
            {
                convertible = GetRootConvertible(converter);
            }
            catch (Exception)
            {
                convertible = null;
                return false;
            }

            return true;
        }

        public static Type?[] GetConversionTypes<TComponent, TComponent2D>(this IConverter<TComponent, TComponent2D> converter, params Component[] components)
            where TComponent : Component
            where TComponent2D : Component
        {
            return Array.ConvertAll(components, component => converter.GetConversionType(component));
        }

        #region Internal
        internal static void Awake(this IConverter converter, UnityAction<bool> convert)
        {
            converter.converter.converted.AddListener(convert);
        }

        internal static void OnDestroy(this IConverter converter, UnityAction<bool> convert)
        {
            converter.converter.converted.RemoveListener(convert);
        }

        internal static void OnValidate(this IConverter converter, UnityAction<bool> convert)
        {
#if UNITY_EDITOR
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(converter.converter.converted, convert);
            UnityEditor.EditorApplication.update -= Update;

            if (!Application.isPlaying)
            {
                UnityEditor.Events.UnityEventTools.AddPersistentListener(converter.converter.converted, convert);
                converter.converter.converted.SetPersistentListenerState(converter.converter.converted.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);
                UnityEditor.EditorApplication.update += Update;
            }

            void Update()
            {
                if (converter is UnityEngine.Object obj && !obj)
                {
                    try
                    {
                        UnityEditor.Events.UnityEventTools.RemovePersistentListener(converter.converter.converted, convert);
                    }
                    catch (MissingReferenceException) { }

                    UnityEditor.EditorApplication.update -= Update;
                }
            }
#endif
        }

        private static Type?[] GetConversionType3Ds<TComponent, TComponent2D>(this IConverter<TComponent, TComponent2D> converter)
            where TComponent : Component
            where TComponent2D : Component
        {
            return converter.GetConversionTypes(converter.component3Ds.ToArray());
        }

        private static Type?[] GetConversionType2Ds<TComponent, TComponent2D>(this IConverter<TComponent, TComponent2D> converter)
            where TComponent : Component
            where TComponent2D : Component
        {
            return converter.GetConversionTypes(converter.component2Ds.ToArray());
        }

        internal static void OnConvert<TComponent, TComponent2D>(this IConverter<TComponent, TComponent2D> converter, bool convertTo2DNot3D, Action<TComponent2D, TComponent> convertTo3D, Action<TComponent, TComponent2D> convertTo2D)
            where TComponent : Component
            where TComponent2D : Component
        {
            if (convertTo2DNot3D)
            {
                converter.ConvertTo2D(convertTo2D);
            }
            else
            {
                converter.ConvertTo3D(convertTo3D);
            }
        }

        private static void ConvertTo3D<TComponent, TComponent2D>(this IConverter<TComponent, TComponent2D> converter, Action<TComponent2D, TComponent> convert)
            where TComponent : Component
            where TComponent2D : Component
        {
            // Get components
            converter.gameObject2D.GetComponents(converter.component2Ds);
            converter.gameObject3D.ForceComponents(converter.component3Ds, Array.FindAll(converter.GetConversionType2Ds(), type => type != null)!);

            // Convert components
            for (int i = 0; i < converter.component2Ds.Count; i++)
            {
                convert(converter.component2Ds[i], converter.component3Ds[i]);
            }
        }

        private static void ConvertTo2D<TComponent, TComponent2D>(this IConverter<TComponent, TComponent2D> converter, Action<TComponent, TComponent2D> convert)
            where TComponent : Component
            where TComponent2D : Component
        {
            // Get components
            converter.gameObject3D.GetComponents(converter.component3Ds);
            converter.gameObject2D.ForceComponents(converter.component2Ds, Array.FindAll(converter.GetConversionType3Ds(), type => type != null)!);

            // Convert components
            for (int i = 0; i < converter.component3Ds.Count; i++)
            {
                convert(converter.component3Ds[i], converter.component2Ds[i]);
            }
        }
        #endregion
    }
}
