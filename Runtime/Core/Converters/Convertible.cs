#nullable enable
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity2Dx
{
    public abstract class Convertible : ConverterBase, IConvertible
    {
        public override IConverter converter => this;

        public override abstract GameObject gameObject3D { get; }
        public override abstract GameObject gameObject2D { get; }

        [field: SerializeField][field: ConvertibleSwitch] public bool is2DNot3D { get; private set; }

        public new void Convert(bool convertTo2DNot3D)
        {
            if (is2DNot3D == convertTo2DNot3D)
            {
                return;
            }

            base.Convert(convertTo2DNot3D);
            is2DNot3D = convertTo2DNot3D;
        }
    }

    public abstract class Convertible<TComponent, TComponent2D> : ConverterBase<TComponent, TComponent2D>, IConvertible
        where TComponent : Component
        where TComponent2D : Component
    {
        public override IConverter converter => this;

        public override abstract GameObject gameObject3D { get; }
        public override abstract GameObject gameObject2D { get; }

        [field: SerializeField][field: ConvertibleSwitch] public bool is2DNot3D { get; private set; }

        public new void Convert(bool convertTo2DNot3D)
        {
            if (is2DNot3D == convertTo2DNot3D)
            {
                return;
            }

            base.Convert(convertTo2DNot3D);
            is2DNot3D = convertTo2DNot3D;
        }
    }

    internal static class CopyGameObjectCache
    {
        private static Dictionary<GameObject, GameObject> copyGameObject3Ds = new();
        private static Dictionary<GameObject, int> count3D = new();

        private static Dictionary<GameObject, GameObject> copyGameObject2Ds = new();
        private static Dictionary<GameObject, int> count2D = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration()
        {
            copyGameObject3Ds = new();
            count3D = new();

            copyGameObject2Ds = new();
            count2D = new();
        }

        public static void Add(GameObject gameObject, GameObject copyGameObject)
        {
            copyGameObject3Ds.Add(gameObject, copyGameObject);
            count3D.Add(gameObject, 1);
        }

        public static void Add2D(GameObject gameObject, GameObject copyGameObject2D)
        {
            copyGameObject2Ds.Add(gameObject, copyGameObject2D);
            count2D.Add(gameObject, 1);
        }

        public static bool Remove(GameObject gameObject)
        {
            if (!count3D.ContainsKey(gameObject)
                || --count3D[gameObject] > 0)
            {
                return false;
            }

            copyGameObject3Ds.Remove(gameObject);
            count3D.Remove(gameObject);
            return true;
        }

        public static bool Remove2D(GameObject gameObject)
        {
            if (!count2D.ContainsKey(gameObject)
                || --count2D[gameObject] > 0)
            {
                return false;
            }

            copyGameObject2Ds.Remove(gameObject);
            count2D.Remove(gameObject);
            return true;
        }

        public static bool TryGetCopy(GameObject gameObject, out GameObject copyGameObject)
        {
            if (copyGameObject3Ds.TryGetValue(gameObject, out copyGameObject))
            {
                count3D[gameObject]++;
                return true;
            }

            return false;
        }

        public static bool TryGetCopy2D(GameObject gameObject, out GameObject copyGameObject2D)
        {
            if (copyGameObject2Ds.TryGetValue(gameObject, out copyGameObject2D))
            {
                count2D[gameObject]++;
                return true;
            }

            return false;
        }
    }

    public abstract class CopyConvertible<TComponent, TComponent2D> : CopyConverterBase<TComponent, TComponent2D>, IConvertible
        where TComponent : Component
        where TComponent2D : Component
    {
        internal const string debugConvertibleCopiesPrefsKey = "Debug Convertible Copies";

        public override ICopyConverter copyConverter => this;

        public override GameObject gameObject3D => gameObject;
        public override GameObject gameObject2D => gameObject;

        internal GameObject? _copyGameObject3D;
        public override GameObject copyGameObject3D
        {
            get
            {
                if (!_copyGameObject3D && !CopyGameObjectCache.TryGetCopy(gameObject, out _copyGameObject3D))
                {
                    _copyGameObject3D = new GameObject($"{name} {nameof(copyGameObject3D)}");
                    _copyGameObject3D.SetActive(false);
                    _copyGameObject3D.hideFlags =
#if UNITY_EDITOR
                        EditorPrefs.GetBool(debugConvertibleCopiesPrefsKey, false) ?
                        HideFlags.DontSave :
#endif
                        HideFlags.HideAndDontSave;

                    CopyGameObjectCache.Add(gameObject, _copyGameObject3D);
                }

                return _copyGameObject3D!;
            }
        }

        internal GameObject? _copyGameObject2D;
        public override GameObject copyGameObject2D
        {
            get
            {
                if (!_copyGameObject2D && !CopyGameObjectCache.TryGetCopy2D(gameObject, out _copyGameObject2D))
                {
                    _copyGameObject2D = new GameObject($"{name} {nameof(copyGameObject2D)}");
                    _copyGameObject2D.SetActive(false);
                    _copyGameObject2D.hideFlags =
#if UNITY_EDITOR
                        EditorPrefs.GetBool(debugConvertibleCopiesPrefsKey, false) ?
                        HideFlags.DontSave :
#endif
                        HideFlags.HideAndDontSave;
                    
                    CopyGameObjectCache.Add2D(gameObject, _copyGameObject2D);
                }

                return _copyGameObject2D!;
            }
        }

        protected virtual void OnDestroy()
        {
            if (CopyGameObjectCache.Remove(gameObject))
            {
                DestroyImmediate(_copyGameObject3D);
            }

            if (CopyGameObjectCache.Remove2D(gameObject))
            {
                DestroyImmediate(_copyGameObject2D);
            }
        }

        [field: SerializeField] [field: ConvertibleSwitch] public bool is2DNot3D { get; private set; }

        public new void Convert(bool convertTo2DNot3D)
        {
            if (is2DNot3D == convertTo2DNot3D)
            {
                return;
            }

            Copy(convertTo2DNot3D);
            base.Convert(convertTo2DNot3D);
            is2DNot3D = convertTo2DNot3D;
        }
    }
}
