#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity2Dx
{
    public interface IConverter
    {
        public IConverter converter { get; }

        public UnityEvent<bool> converted { get; }

        public GameObject gameObject3D { get; }
        public GameObject gameObject2D { get; }
    }

    public interface IConverter<TComponent, TComponent2D> : IConverter
        where TComponent : Component
        where TComponent2D : Component
    {
        public List<TComponent> component3Ds { get; }
        public List<TComponent2D> component2Ds { get; }

        public Type? GetConversionType(Component component);
    }
}
