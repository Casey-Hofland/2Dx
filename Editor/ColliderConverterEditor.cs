using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DimensionConverter.Editor
{
    [CustomEditor(typeof(ColliderConverter))]
    [CanEditMultipleObjects]
    public class ColliderConverterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
