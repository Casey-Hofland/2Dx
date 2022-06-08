#nullable enable
using System;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;

namespace Unity2Dx
{
    public static class ICopyConverterExtensions
    {
        internal static void Awake(this ICopyConverter copyConverter, UnityAction<bool> convert, UnityAction<bool> copy)
        {
            copyConverter.Awake(convert);

#if UNITY_EDITOR
            // Due to Undo operations or entering and exiting playmode, the persistent listener may remain, so we must make sure to remove it on awake as well.
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(copyConverter.copyConverter.copied, copy);

            if (!Application.isPlaying)
            {
                UnityEditor.Events.UnityEventTools.AddPersistentListener(copyConverter.copyConverter.copied, copy);
                copyConverter.copyConverter.copied.SetPersistentListenerState(copyConverter.copyConverter.copied.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);
            }
            else
#endif
            {
                copyConverter.copyConverter.copied.AddListener(copy);
            }
        }

        internal static void OnDestroy(this ICopyConverter copyConverter, UnityAction<bool> convert, UnityAction<bool> copy)
        {
            copyConverter.OnDestroy(convert);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Events.UnityEventTools.RemovePersistentListener(copyConverter.copyConverter.copied, copy);
            }
            else
#endif
            {
                copyConverter.copyConverter.copied.RemoveListener(copy);
            }
        }

        private static Type?[] GetConversionType3Ds<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter)
            where TComponent : Component
            where TComponent2D : Component
        {
            return copyConverter.GetConversionTypes(copyConverter.copyComponent3Ds.ToArray());
        }

        private static Type?[] GetConversionType2Ds<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter)
            where TComponent : Component
            where TComponent2D : Component
        {
            return copyConverter.GetConversionTypes(copyConverter.copyComponent2Ds.ToArray());
        }

        internal static Type[] CopyTypeComponents<TComponent, TComponent2D>(this IConverter<TComponent, TComponent2D> converter, params Component[] components)
            where TComponent : Component
            where TComponent2D : Component
        {
            return Array.ConvertAll(components, component => component.GetType());
        }

        private static Type[] CopyType3Ds<TComponent, TComponent2D>(this IConverter<TComponent, TComponent2D> converter)
            where TComponent : Component
            where TComponent2D : Component
        {
            return converter.CopyTypeComponents(converter.component3Ds.ToArray());
        }

        private static Type[] CopyType2Ds<TComponent, TComponent2D>(this IConverter<TComponent, TComponent2D> converter)
            where TComponent : Component
            where TComponent2D : Component
        {
            return converter.CopyTypeComponents(converter.component2Ds.ToArray());
        }

        internal static void OnCopy<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter, bool copy3DNot2D, Action<TComponent, TComponent> copy3D, Action<TComponent2D, TComponent2D> copy2D)
            where TComponent : Component
            where TComponent2D : Component
        {
            if (copy3DNot2D)
            {
                copyConverter.Copy3D(copy3D);
            }
            else
            {
                copyConverter.Copy2D(copy2D);
            }
        }

        private static void Copy3D<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter, Action<TComponent, TComponent> copy)
            where TComponent : Component
            where TComponent2D : Component
        {
            // Copy the transform
            copyConverter.copyGameObject3D.transform.SetPositionAndRotation(copyConverter.gameObject3D.transform.position, copyConverter.gameObject3D.transform.rotation);
            copyConverter.copyGameObject3D.transform.localScale = copyConverter.gameObject3D.transform.localScale;

            // Get Components
            copyConverter.gameObject3D.GetComponents(copyConverter.component3Ds);
            copyConverter.copyGameObject3D.ForceComponents(copyConverter.copyComponent3Ds, copyConverter.CopyType3Ds());

            // Copy Components
            for (int i = 0; i < copyConverter.component3Ds.Count; i++)
            {
                copy(copyConverter.component3Ds[i], copyConverter.copyComponent3Ds[i]);

                if (copyConverter.copyReference2Ds.Remove(copyConverter.component3Ds[i], out var copyReference2D) && copyReference2D)
                {
                    copyConverter.copyReference2Ds.Add(copyConverter.copyComponent3Ds[i], copyReference2D);
                }
            }
        }

        private static void Copy2D<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter, Action<TComponent2D, TComponent2D> copy)
            where TComponent : Component
            where TComponent2D : Component
        {
            // Copy the transform
            copyConverter.copyGameObject2D.transform.SetPositionAndRotation(copyConverter.gameObject2D.transform.position, copyConverter.gameObject2D.transform.rotation);
            copyConverter.copyGameObject2D.transform.localScale = copyConverter.gameObject2D.transform.localScale;

            // Get Components
            copyConverter.gameObject2D.GetComponents(copyConverter.component2Ds);
            copyConverter.copyGameObject2D.ForceComponents(copyConverter.copyComponent2Ds, copyConverter.CopyType2Ds());

            // Copy Components
            for (int i = 0; i < copyConverter.component2Ds.Count; i++)
            {
                copy(copyConverter.component2Ds[i], copyConverter.copyComponent2Ds[i]);

                if (copyConverter.copyReference3Ds.Remove(copyConverter.component2Ds[i], out var copyReference3D) && copyReference3D)
                {
                    copyConverter.copyReference3Ds.Add(copyConverter.copyComponent2Ds[i], copyReference3D);
                }
            }
        }

        internal static void OnCopied<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter, bool copy3DNot2D)
            where TComponent : Component
            where TComponent2D : Component
        {
            if (copy3DNot2D)
            {
                copyConverter.Copied3D();
            }
            else
            {
                copyConverter.Copied2D();
            }
        }

        private static void Copied3D<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter)
            where TComponent : Component
            where TComponent2D : Component
        {
            // Destroy components
            for (int i = 0; i < copyConverter.component3Ds.Count; i++)
            {
                Object.DestroyImmediate(copyConverter.component3Ds[i]);
            }
        }

        private static void Copied2D<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter)
            where TComponent : Component
            where TComponent2D : Component
        {
            // Destroy components
            for (int i = 0; i < copyConverter.component2Ds.Count; i++)
            {
                Object.DestroyImmediate(copyConverter.component2Ds[i]);
            }
        }

        internal static void OnConvert<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter, bool convertTo2DNot3D, Action<TComponent, TComponent> copyTo3D, Action<TComponent2D, TComponent> convertTo3D, Action<TComponent2D, TComponent2D> copyTo2D, Action<TComponent, TComponent2D> convertTo2D)
            where TComponent : Component
            where TComponent2D : Component
        {
            if (convertTo2DNot3D)
            {
                copyConverter.ConvertTo2D(copyTo2D, convertTo2D);
            }
            else
            {
                copyConverter.ConvertTo3D(copyTo3D, convertTo3D);
            }
        }

        private static void ConvertTo3D<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter, Action<TComponent, TComponent> copy, Action<TComponent2D, TComponent> convert)
            where TComponent : Component
            where TComponent2D : Component
        {
            // Get components
            copyConverter.gameObject3D.ForceComponents(copyConverter.component3Ds, Array.FindAll(copyConverter.GetConversionType2Ds(), type => type != null)!);

            // Convert components
            copyConverter.copyReference2Ds.Clear();
            for (int i = 0; i < copyConverter.copyComponent2Ds.Count; i++)
            {
                if (copyConverter.copyReference3Ds.Remove(copyConverter.copyComponent2Ds[i], out var copyReference3D))
                {
                    copy(copyReference3D, copyConverter.component3Ds[i]);
                }

                convert(copyConverter.copyComponent2Ds[i], copyConverter.component3Ds[i]);
                copyConverter.copyReference2Ds.Add(copyConverter.component3Ds[i], copyConverter.copyComponent2Ds[i]);
            }
        }

        private static void ConvertTo2D<TComponent, TComponent2D>(this ICopyConverter<TComponent, TComponent2D> copyConverter, Action<TComponent2D, TComponent2D> copy, Action<TComponent, TComponent2D> convert)
            where TComponent : Component
            where TComponent2D : Component
        {
            // Get components
            copyConverter.gameObject2D.ForceComponents(copyConverter.component2Ds, Array.FindAll(copyConverter.GetConversionType3Ds(), type => type != null)!);

            // Convert components
            copyConverter.copyReference3Ds.Clear();
            for (int i = 0; i < copyConverter.copyComponent3Ds.Count; i++)
            {
                if (copyConverter.copyReference2Ds.Remove(copyConverter.copyComponent3Ds[i], out var copyReference2D))
                {
                    copy(copyReference2D, copyConverter.component2Ds[i]);
                }

                convert(copyConverter.copyComponent3Ds[i], copyConverter.component2Ds[i]);
                copyConverter.copyReference3Ds.Add(copyConverter.component2Ds[i], copyConverter.copyComponent3Ds[i]);
            }
        }
    }
}
