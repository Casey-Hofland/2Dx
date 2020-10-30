using DimensionConverter.Utilities;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DimensionConverter.Editor
{
    [CustomEditor(typeof(CapsuleConverter))]
    [CanEditMultipleObjects]
    public class CapsuleConverterEditor : UnityEditor.Editor
    {
        private void Awake()
        {
            if(targets.Cast<Component>().Any(c => c.GetComponent<CapsuleCollider>() || c.GetComponent<CapsuleCollider2D>()))
            {
                EditorUtility.DisplayDialog($"Move Colliders", $"Colliders on this object will be moved to Transform3D for 3D Colliders and Transform2D for 2D Colliders", "ok", "ok", DialogOptOutDecisionType.ForThisMachine, "CapsuleConverterEditorKey");
            }

            foreach(CapsuleConverter capsuleConverter in targets)
            {
                foreach(CapsuleCollider capsuleCollider in capsuleConverter.GetComponents<CapsuleCollider>())
                {
                    var newCapsuleCollider = capsuleConverter.transformSplitter.gameObject3D.AddComponent<CapsuleCollider>();
                    capsuleCollider.ToCapsuleCollider(newCapsuleCollider);

                    ReferenceUtility.ChangeReferences(capsuleCollider, newCapsuleCollider);
                    EditorApplication.delayCall += () => DestroyImmediate(capsuleCollider);
                }

                foreach(CapsuleCollider2D capsuleCollider2D in capsuleConverter.GetComponents<CapsuleCollider2D>())
                {
                    var newCapsuleCollider2D = capsuleConverter.transformSplitter.gameObject2D.AddComponent<CapsuleCollider2D>();
                    capsuleCollider2D.ToCapsuleCollider2D(newCapsuleCollider2D);

                    ReferenceUtility.ChangeReferences(capsuleCollider2D, newCapsuleCollider2D);
                    EditorApplication.delayCall += () => DestroyImmediate(capsuleCollider2D);
                }
            }
        }
    }
}
