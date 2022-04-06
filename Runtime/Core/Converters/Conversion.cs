#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity2Dx
{
    public class Conversion
    {
        public static void ConvertAll(bool convertTo2DNot3D) => Convert(convertTo2DNot3D, Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IConvertible>());
        public static void Convert(bool convertTo2DNot3D, GameObject gameObject) => Convert(convertTo2DNot3D, gameObject.GetComponents<IConvertible>());
        public static void Convert(bool convertTo2DNot3D, params IConvertible[] convertibles) => Convert(convertTo2DNot3D, (IEnumerable<IConvertible>)convertibles);
        public static void Convert(bool convertTo2DNot3D, IEnumerable<IConvertible> convertibles)
        {
            foreach (var convertible in convertibles)
            {
                convertible.Convert(convertTo2DNot3D);
            }
        }
    }
}
