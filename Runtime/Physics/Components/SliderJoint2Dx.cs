#nullable enable
using System.Collections.Generic;
using Unity2Dx;
using Unity2Dx.Physics;
using UnityEngine;
using UnityExtras;

[RequireComponent(typeof(Rigidbody2Dx))]
[DisallowMultipleComponent]
public sealed class SliderJoint2Dx : CopyConverter<Rigidbody2Dx, SliderJoint, SliderJoint2D>
{
    private Dictionary<Joint, Rigidbody2D> convertedBodies = new();
    private Dictionary<Joint2D, Rigidbody> convertedBody2Ds = new();

    private Dictionary<Joint, Rigidbody> connectedBodies = new();
    private Dictionary<Joint2D, Rigidbody2D> connectedBody2Ds = new();

    protected override void ComponentToComponent(SliderJoint component, SliderJoint other)
    {
        if (connectedBodies.TryGetValue(component.configurableJoint, out var connectedBody))
        {
            if (ConnectedBodyManager.TryGetConverted(connectedBody, out var rigidbody2D))
            {
                convertedBodies.Add(other.configurableJoint, rigidbody2D);
            }
            ConnectedBodyManager.Disconnect(connectedBody);
        }

        component.ToSliderJoint(other);
        DestroyImmediate(component.configurableJoint);
    }

    protected override void Component2DToComponent2D(SliderJoint2D component2D, SliderJoint2D other)
    {
        if (connectedBody2Ds.TryGetValue(component2D, out var connectedBody))
        {
            if (ConnectedBodyManager.TryGetConverted(connectedBody, out var rigidbody))
            {
                convertedBody2Ds.Add(other, rigidbody);
            }
            ConnectedBodyManager.Disconnect(connectedBody);
        }

        component2D.ToSliderJoint2D(other);
    }

    protected override void ComponentToComponent2D(SliderJoint component, SliderJoint2D component2D)
    {
        if (convertedBodies.Remove(component.configurableJoint, out var rigidbody2D))
        {
            component2D.connectedBody = rigidbody2D;
        }

        component.ToSliderJoint2D(component2D);
    }

    protected override void Component2DToComponent(SliderJoint2D component2D, SliderJoint component)
    {
        if (convertedBody2Ds.Remove(component2D, out var rigidbody))
        {
            component.connectedBody = rigidbody;
        }

        component2D.ToSliderJoint(component);
    }

    private void Update()
    {
        if (this.TryGetRootConvertible(out var convertible))
        {
            if (convertible!.is2DNot3D)
            {
                gameObject2D.GetComponents(component2Ds);

                foreach (SliderJoint2D joint2D in component2Ds)
                {
                    if (joint2D.connectedBody == null)
                    {
                        continue;
                    }

                    if (!connectedBody2Ds.TryAdd(joint2D, joint2D.connectedBody))
                    {
                        ConnectedBodyManager.Disconnect(connectedBody2Ds[joint2D]);
                        connectedBody2Ds[joint2D] = joint2D.connectedBody;
                    }

                    if (connectedBody2Ds.TryGetValue(joint2D, out var connectedBody) && connectedBody != joint2D.connectedBody)
                    {
                        ConnectedBodyManager.Disconnect(connectedBody2Ds[joint2D]);
                    }

                    connectedBody2Ds[joint2D] = joint2D.connectedBody;
                    ConnectedBodyManager.Connect(joint2D.connectedBody);
                }

                connectedBodies.Clear();
            }
            else
            {
                gameObject3D.GetComponents(component3Ds);

                foreach (SliderJoint joint in component3Ds)
                {
                    if (joint.connectedBody == null)
                    {
                        continue;
                    }

                    if (connectedBodies.TryGetValue(joint.configurableJoint, out var connectedBody) && connectedBody != joint.connectedBody)
                    {
                        ConnectedBodyManager.Disconnect(connectedBodies[joint.configurableJoint]);
                    }

                    connectedBodies[joint.configurableJoint] = joint.connectedBody;
                    ConnectedBodyManager.Connect(joint.connectedBody);
                }

                connectedBody2Ds.Clear();
            }
        }
    }
}
