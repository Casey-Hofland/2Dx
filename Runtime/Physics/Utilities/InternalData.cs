#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity2Dx.Physics
{
    internal static class InternalData
    {
        public static Dictionary<Joint, (UnityAction<bool> connectEvent, Rigidbody2Dx connectedBody2Dx)> connectEvents = new Dictionary<Joint, (UnityAction<bool> connectEvent, Rigidbody2Dx connectedBody)>();
        public static Dictionary<Joint2D, (UnityAction<bool> connectEvent2D, Rigidbody2Dx connectedBody2Dx)> connectEvent2Ds = new Dictionary<Joint2D, (UnityAction<bool> connectEvent2D, Rigidbody2Dx connectedBody2Dx)>();
    }
}
