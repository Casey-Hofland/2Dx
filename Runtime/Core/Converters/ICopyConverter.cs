#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity2Dx
{
    public interface ICopyConverter : IConverter
    {
        public ICopyConverter copyConverter { get; }

        public UnityEvent<bool> copied { get; }

        public GameObject copyGameObject3D { get; }
        public GameObject copyGameObject2D { get; }
    }

    public interface ICopyConverter<TComponent, TComponent2D> : ICopyConverter, IConverter<TComponent, TComponent2D>
        where TComponent : Component
        where TComponent2D : Component
    {
        public List<TComponent> copyComponent3Ds { get; }
        public List<TComponent2D> copyComponent2Ds { get; }
    }
}
