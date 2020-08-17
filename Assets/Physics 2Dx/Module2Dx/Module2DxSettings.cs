using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Physics2Dx.Editor")]
namespace Physics2DxSystem
{
    [Serializable]
    internal struct Module2DxSettings : ISerializationCallbackReceiver
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
            return obj is Module2DxSettings module2DxSettings
                && module2DxSettings.type == type
                && module2DxSettings.batchSize3D == batchSize3D
                && module2DxSettings.batchSize2D == batchSize2D
                && module2DxSettings.typeName == typeName;
        }

        public override int GetHashCode() => (type, batchSize3D, batchSize2D, typeName).GetHashCode();

        public static bool operator ==(Module2DxSettings left, Module2DxSettings right) => left.Equals(right);
        public static bool operator !=(Module2DxSettings left, Module2DxSettings right) => !(left == right);
    }
}

