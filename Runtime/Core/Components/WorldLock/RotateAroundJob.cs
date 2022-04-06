#nullable enable
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Unity2Dx
{
    public struct RotateAroundJob : IJobParallelForTransform
    {
        [ReadOnly] public Vector3 point;
        [ReadOnly] public Quaternion rotation;

        public RotateAroundJob(Transform transform) : this(transform.position, Quaternion.Inverse(transform.rotation)) { }
        public RotateAroundJob(Vector3 euler) : this(Vector3.zero, euler) { }
        public RotateAroundJob(Vector3 point, Vector3 euler) : this(point, Quaternion.Euler(euler)) { }
        public RotateAroundJob(Quaternion rotation) : this(Vector3.zero, rotation) { }
        public RotateAroundJob(Vector3 point, Quaternion rotation)
        {
            this.point = point;
            this.rotation = rotation;
        }

        public void Execute(int index, TransformAccess transform)
        {
            var pointDelta = rotation * (transform.position - point);
            transform.position = pointDelta + point;
            transform.rotation = rotation * transform.rotation;
        }
    }
}
