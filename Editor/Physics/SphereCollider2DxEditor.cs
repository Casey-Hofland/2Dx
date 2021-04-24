using System.Linq;
using Unity2Dx.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity2Dx.Physics.Editor
{
    [CustomEditor(typeof(SphereCollider2Dx))]
    [CanEditMultipleObjects]
    public class SphereCollider2DxEditor : UnityEditor.Editor
    {
        private void Awake()
        {
            if(targets.Cast<Component>().Any(c => c.GetComponent<SphereCollider>() || c.GetComponent<CircleCollider2D>()))
            {
                EditorUtility.DisplayDialog($"Move Colliders", $"Colliders on this object will be moved to Transform3D for 3D Colliders and Transform2D for 2D Colliders", "ok", "ok", DialogOptOutDecisionType.ForThisMachine, "SphereConverterEditorKey");
            }

            foreach(SphereCollider2Dx sphereCollider2Dx in targets)
            {
                foreach(SphereCollider sphereCollider in sphereCollider2Dx.GetComponents<SphereCollider>())
                {
                    var newSphereCollider = sphereCollider2Dx.transform2Dx.gameObject3D.AddComponent<SphereCollider>();
                    sphereCollider.ToSphereCollider(newSphereCollider);

                    ReferenceUtility.ChangeReferences(sphereCollider, newSphereCollider);
                    EditorApplication.delayCall += () => DestroyImmediate(sphereCollider);
                }

                foreach(CircleCollider2D circleCollider2D in sphereCollider2Dx.GetComponents<CircleCollider2D>())
                {
                    var newCircleCollider2D = sphereCollider2Dx.transform2Dx.gameObject2D.AddComponent<CircleCollider2D>();
                    circleCollider2D.ToCircleCollider2D(newCircleCollider2D);

                    ReferenceUtility.ChangeReferences(circleCollider2D, newCircleCollider2D);
                    EditorApplication.delayCall += () => DestroyImmediate(circleCollider2D);
                }
            }
        }
    }
}
