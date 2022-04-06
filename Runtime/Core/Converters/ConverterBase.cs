#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity2Dx
{
    public abstract class ConverterBase : MonoBehaviour, IConverter
    {
        public abstract IConverter converter { get; }

        [field: SerializeField]
        [field: HideInInspector]
        public UnityEvent<bool> converted { get; private set; } = new UnityEvent<bool>();

        public virtual GameObject gameObject3D => converter.gameObject3D;
        public virtual GameObject gameObject2D => converter.gameObject2D;

        protected abstract void OnConvert(bool convertTo2DNot3D);
        protected virtual void OnConverted(bool convertTo2DNot3D) { }

        protected void Convert(bool convertTo2DNot3D)
        {
            OnConvert(convertTo2DNot3D);

            converted.Invoke(convertTo2DNot3D);

            OnConverted(convertTo2DNot3D);
        }
    }

    public abstract class ConverterBase<TComponent, TComponent2D> : ConverterBase, IConverter<TComponent, TComponent2D>
    where TComponent : Component
    where TComponent2D : Component
    {
        public List<TComponent> component3Ds { get; } = new List<TComponent>();
        public List<TComponent2D> component2Ds { get; } = new List<TComponent2D>();

        protected abstract void ComponentToComponent2D(TComponent component, TComponent2D component2D);
        protected abstract void Component2DToComponent(TComponent2D component2D, TComponent component);

        protected override void OnConvert(bool convertTo2DNot3D) => this.OnConvert(convertTo2DNot3D, Component2DToComponent, ComponentToComponent2D);

        public virtual Type? GetConversionType(Component component)
        {
            return component switch
            {
                TComponent _ => typeof(TComponent2D),
                TComponent2D _ => typeof(TComponent),
                _ => null,
            };
        }
    }

    public abstract class CopyConverterBase<TComponent, TComponent2D> : ConverterBase<TComponent, TComponent2D>, ICopyConverter<TComponent, TComponent2D>
    where TComponent : Component
    where TComponent2D : Component
    {
        public override IConverter converter => copyConverter;
        public abstract ICopyConverter copyConverter { get; }
        
        [field: SerializeField]
        [field: HideInInspector]
        public UnityEvent<bool> copied { get; private set; } = new UnityEvent<bool>();

        public virtual GameObject copyGameObject3D => copyConverter.copyGameObject3D;
        public virtual GameObject copyGameObject2D => copyConverter.copyGameObject2D;

        public List<TComponent> copyComponent3Ds { get; } = new List<TComponent>();
        public List<TComponent2D> copyComponent2Ds { get; } = new List<TComponent2D>();

        protected abstract void ComponentToComponent(TComponent component, TComponent other);
        protected abstract void Component2DToComponent2D(TComponent2D component2D, TComponent2D other);

        protected override void OnConvert(bool convertTo2DNot3D) => this.OnConvert(convertTo2DNot3D, Component2DToComponent, ComponentToComponent2D);
        protected virtual void OnCopy(bool copy3DNot2D) => this.OnCopy(copy3DNot2D, ComponentToComponent, Component2DToComponent2D);
        protected virtual void OnCopied(bool copy3DNot2D) => ((ICopyConverter<TComponent, TComponent2D>)this).OnCopied(copy3DNot2D);

        protected void Copy(bool copy3DNot2D)
        {
            OnCopy(copy3DNot2D);

            copied.Invoke(copy3DNot2D);

            OnCopied(copy3DNot2D);
        }
    }
}
