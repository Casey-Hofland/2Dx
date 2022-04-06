#nullable enable
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

    public abstract class CopyConvertible<TComponent, TComponent2D> : CopyConverterBase<TComponent, TComponent2D>, IConvertible
        where TComponent : Component
        where TComponent2D : Component
    {
        public override ICopyConverter copyConverter => this;

        public override GameObject gameObject3D => gameObject;
        public override GameObject gameObject2D => gameObject;

        internal GameObject? _copyGameObject3D;
        public override GameObject copyGameObject3D
        {
            get
            {
                if (!_copyGameObject3D)
                {
                    _copyGameObject3D = new GameObject($"{name} {nameof(copyGameObject3D)}");
                    _copyGameObject3D.SetActive(false);

#if UNITY_EDITOR
                    // We cannot use DontDestroyOnLoad during edit mode, so instead we destroy the copy the next frame.
                    if (!Application.IsPlaying(_copyGameObject3D))
                    {
                        UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(_copyGameObject3D);
                    }
                    else
#endif
                    {
                        DontDestroyOnLoad(_copyGameObject3D);
                    }
                }

                //var _copyGameObject3DFieldInfo = typeof(CopyConvertible<,>).GetField(nameof(_copyGameObject3D));
                //var copyConvertibles = from component in GetComponents<INewConvertible>()
                //                       where (Component)component != this
                //                       let type = component.GetType()
                //                       where type.GetGenericTypeDefinition() == typeof(CopyConvertible<,>)
                //                       select type;

                //Debug.Log("Does this actually work?");
                //foreach (var copyConvertible in copyConvertibles)
                //{
                //    Debug.Log("Yeah it does!");
                //    _copyGameObject3DFieldInfo.SetValue(copyConvertible, _copyGameObject3D);
                //}

                return _copyGameObject3D!;
            }
        }

        internal GameObject? _copyGameObject2D;
        public override GameObject copyGameObject2D
        {
            get
            {
                if (!_copyGameObject2D)
                {
                    _copyGameObject2D = new GameObject($"{name} {nameof(copyGameObject2D)}");
                    _copyGameObject2D.SetActive(false);

#if UNITY_EDITOR
                    // We cannot use DontDestroyOnLoad during edit mode, so instead we destroy the copy the next frame.
                    if (!Application.IsPlaying(_copyGameObject2D))
                    {
                        UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(_copyGameObject2D);
                    }
                    else
#endif
                    {
                        DontDestroyOnLoad(_copyGameObject2D);
                    }
                }

                //Debug.Log("Does this actually work?");
                //foreach (var copyConvertible in GetComponents<CopyConvertible<Component, Component>>())
                //{
                //    Debug.Log("Yeah it does!");
                //    copyConvertible._copyGameObject2D = _copyGameObject2D;
                //}

                return _copyGameObject2D!;
            }
        }

        protected virtual void OnDestroy()
        {
            Debug.LogWarning("Optimize for multiple Convertibles on the same Game Object!");

            DestroyImmediate(_copyGameObject3D);
            DestroyImmediate(_copyGameObject2D);
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
