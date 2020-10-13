using UnityEngine;

namespace DimensionConverter.Utilities
{
    public static class Matrix4x4Helper
    {
        public static Matrix4x4 Lerp(Matrix4x4 a, Matrix4x4 b, float t)
        {
            for(int i = 0; i < 16; i++)
            {
                a[i] = Mathf.Lerp(a[i], b[i], t);
            }
            return a;
        }

        public static Matrix4x4 LerpUnclamped(Matrix4x4 a, Matrix4x4 b, float t)
        {
            for(int i = 0; i < 16; i++)
            {
                a[i] = Mathf.LerpUnclamped(a[i], b[i], t);
            }
            return a;
        }
    }
}

