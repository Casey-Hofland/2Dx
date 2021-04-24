using System.Linq;
using Unity2Dx.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity2Dx.Physics.Editor
{
    [CustomEditor(typeof(CapsuleCollider2Dx))]
    [CanEditMultipleObjects]
    public class CapsuleCollider2DxEditor : UnityEditor.Editor
    {
        private void Awake()
        {
            if(targets.Cast<Component>().Any(c => c.GetComponent<CapsuleCollider>() || c.GetComponent<CapsuleCollider2D>()))
            {
                EditorUtility.DisplayDialog($"Move Colliders", $"Colliders on this object will be moved to Transform3D for 3D Colliders and Transform2D for 2D Colliders", "ok", "ok", DialogOptOutDecisionType.ForThisMachine, "CapsuleConverterEditorKey");
            }

            foreach(CapsuleCollider2Dx capsuleCollider2Dx in targets)
            {
                foreach(CapsuleCollider capsuleCollider in capsuleCollider2Dx.GetComponents<CapsuleCollider>())
                {
                    var newCapsuleCollider = capsuleCollider2Dx.transform2Dx.gameObject3D.AddComponent<CapsuleCollider>();
                    capsuleCollider.ToCapsuleCollider(newCapsuleCollider);

                    ReferenceUtility.ChangeReferences(capsuleCollider, newCapsuleCollider);
                    EditorApplication.delayCall += () => DestroyImmediate(capsuleCollider);
                }

                foreach(CapsuleCollider2D capsuleCollider2D in capsuleCollider2Dx.GetComponents<CapsuleCollider2D>())
                {
                    var newCapsuleCollider2D = capsuleCollider2Dx.transform2Dx.gameObject2D.AddComponent<CapsuleCollider2D>();
                    capsuleCollider2D.ToCapsuleCollider2D(newCapsuleCollider2D);

                    ReferenceUtility.ChangeReferences(capsuleCollider2D, newCapsuleCollider2D);
                    EditorApplication.delayCall += () => DestroyImmediate(capsuleCollider2D);
                }
            }
        }
    }
}
