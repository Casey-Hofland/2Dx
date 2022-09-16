#nullable enable
using UnityEngine;

namespace Unity2Dx
{
    public abstract class Converter<TConverter> : ConverterBase
        where TConverter : Component, IConverter
    {
        private TConverter? _converter;
        public override IConverter converter => _converter ? _converter! : (_converter = GetComponent<TConverter>());

        protected virtual void OnValidate() => this.OnValidate(Convert);

        protected virtual void Awake() => this.Awake(Convert);
        protected virtual void OnDestroy() => this.OnDestroy(Convert);
    }

    public abstract class Converter<TConverter, TComponent, TComponent2D> : ConverterBase<TComponent, TComponent2D>
        where TConverter : Component, IConverter
        where TComponent : Component
        where TComponent2D : Component
    {
        private TConverter? _converter;
        public override IConverter converter => _converter ? _converter! : (_converter = GetComponent<TConverter>());

        protected virtual void OnValidate() => this.OnValidate(Convert);

        protected virtual void Awake() => this.Awake(Convert);
        protected virtual void OnDestroy() => this.OnDestroy(Convert);
    }

    public abstract class CopyConverter<TConverter, TComponent, TComponent2D> : CopyConverterBase<TComponent, TComponent2D>
        where TConverter : Component, ICopyConverter
        where TComponent : Component
        where TComponent2D : Component
    {
        private TConverter? _converter;
        public override ICopyConverter copyConverter => _converter ? _converter! : (_converter = GetComponent<TConverter>());

        protected virtual void OnValidate() => this.OnValidate(Convert);

        protected virtual void Awake() => this.Awake(Convert, Copy);
        protected virtual void OnDestroy() => this.OnDestroy(Convert, Copy);
    }
}
