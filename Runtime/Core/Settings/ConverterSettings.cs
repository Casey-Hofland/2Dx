using System;
using UnityEngine;

namespace Unity2Dx
{
    [Serializable]
    internal struct ConverterSettings : ISerializationCallbackReceiver
    {
        public Type type;

        public uint batchSize3D;
        public uint batchSize2D;

        [SerializeField] [HideInInspector] private string typeName;

        public void OnBeforeSerialize()
        {
            typeName = type?.AssemblyQualifiedName;
        }

        public void OnAfterDeserialize()
        {
            type = Type.GetType(typeName);
        }

        public override bool Equals(object obj)
        {
            return obj is ConverterSettings converterSettings
                && converterSettings.type == type
                && converterSettings.batchSize3D == batchSize3D
                && converterSettings.batchSize2D == batchSize2D
                && converterSettings.typeName == typeName;
        }

        public override int GetHashCode() => (type, batchSize3D, batchSize2D, typeName).GetHashCode();

        public static bool operator ==(ConverterSettings left, ConverterSettings right) => left.Equals(right);
        public static bool operator !=(ConverterSettings left, ConverterSettings right) => !(left == right);
    }
}

