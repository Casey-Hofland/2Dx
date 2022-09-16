#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;

namespace Unity2Dx.Physics
{
    public class Connector2Dx
    {
        public readonly Rigidbody2Dx connectedBody2Dx;
        private readonly UnityAction<bool>? connected;

        public Connector2Dx(Rigidbody2Dx connectedBody2Dx, Action<Rigidbody2D>? setRigidbody2D, Action<Rigidbody>? setRigidbody)
        {
            this.connectedBody2Dx = connectedBody2Dx;

            connected = null;
            connected = new UnityAction<bool>((convertTo2DNot3D) =>
            {
                if (convertTo2DNot3D)
                {
                    if (connectedBody2Dx.component2Ds.Count > 0)
                    {
                        setRigidbody2D?.Invoke(connectedBody2Dx.component2Ds[0]);
                    }
                }
                else
                {
                    if (connectedBody2Dx.component3Ds.Count > 0)
                    {
                        setRigidbody?.Invoke(connectedBody2Dx.component3Ds[0]);
                    }
                }
                connectedBody2Dx.converted.RemoveListener(connected);
            });

            connectedBody2Dx.converted.AddListener(connected);
        }

        public Connector2Dx(Rigidbody2Dx connectedBody2Dx, Action<Rigidbody2D> setRigidbody2D) : this(connectedBody2Dx, setRigidbody2D, default) { }
        public Connector2Dx(Rigidbody2Dx connectedBody2Dx, Action<Rigidbody> setRigidbody) : this(connectedBody2Dx, default, setRigidbody) { }

        ~Connector2Dx()
        {
            connectedBody2Dx.converted.RemoveListener(connected);
        }
    }

    public static class ConnectedBodyManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration()
        {
            rigidbodyCoupler = new();
            rigidbody2DCoupler = new();

            convertedBodies = new();
            convertedBody2Ds = new();
        }

        private class Coupler<T> where T : Component
        {
            private Dictionary<T, Connector2Dx> connectors = new();

            public void Connect(T component, Action<Rigidbody2D>? setRigidbody2D, Action<Rigidbody>? setRigidbody)
            {
                if (!component || !component.TryGetComponent(out Rigidbody2Dx rigidbody2Dx))
                {
                    connectors.Remove(component);
                    return;
                }

                if (connectors.TryGetValue(component, out var connector)
                    && connector.connectedBody2Dx == rigidbody2Dx)
                {
                    return;
                }

                connectors[component] = new Connector2Dx(rigidbody2Dx, setRigidbody2D, setRigidbody);
            }

            public bool Disconnect(T component) => connectors.Remove(component);
        }

        private static Coupler<Rigidbody> rigidbodyCoupler = new();
        private static Coupler<Rigidbody2D> rigidbody2DCoupler = new();

        private static Dictionary<Rigidbody, Rigidbody2D> convertedBodies = new();
        private static Dictionary<Rigidbody2D, Rigidbody> convertedBody2Ds = new();

        public static void Connect(Rigidbody rigidbody)
        {
            rigidbodyCoupler.Connect(rigidbody, SetRigidbody2D, default);

            void SetRigidbody2D(Rigidbody2D rigidbody2D)
            {
                convertedBodies[rigidbody] = rigidbody2D;
                rigidbodyCoupler.Disconnect(rigidbody);
            }
        }
        public static void Connect(Rigidbody rigidbody, Action<Rigidbody2D> setRigidbody2D) => rigidbodyCoupler.Connect(rigidbody, setRigidbody2D, default);
        public static void Connect(Rigidbody2D rigidbody2D)// => rigidbody2DThing.Connect(rigidbody2D, default, (rigidbody) => convertedBody2Ds[rigidbody2D] = rigidbody);
        {
            rigidbody2DCoupler.Connect(rigidbody2D, default, SetRigidbody);

            void SetRigidbody(Rigidbody rigidbody)
            {
                convertedBody2Ds[rigidbody2D] = rigidbody;
                rigidbody2DCoupler.Disconnect(rigidbody2D);
            }
        }
        public static void Connect(Rigidbody2D rigidbody2D, Action<Rigidbody> setRigidbody) => rigidbody2DCoupler.Connect(rigidbody2D, default, setRigidbody);

        public static bool Disconnect(Rigidbody rigidbody) => convertedBodies.Remove(rigidbody) || rigidbodyCoupler.Disconnect(rigidbody);
        public static bool Disconnect(Rigidbody2D rigidbody2D) => convertedBody2Ds.Remove(rigidbody2D) || rigidbody2DCoupler.Disconnect(rigidbody2D);

        public static Rigidbody2D GetConverted(Rigidbody rigidbody) => convertedBodies[rigidbody];
        public static Rigidbody GetConverted(Rigidbody2D rigidbody2D) => convertedBody2Ds[rigidbody2D];

        public static bool TryGetConverted(Rigidbody rigidbody, [NotNullWhen(true)] out Rigidbody2D? rigidbody2D) => convertedBodies.TryGetValue(rigidbody, out rigidbody2D);
        public static bool TryGetConverted(Rigidbody2D rigidbody2D, [NotNullWhen(true)] out Rigidbody? rigidbody) => convertedBody2Ds.TryGetValue(rigidbody2D, out rigidbody);
    }
}
