using DimensionConverter.Utilities;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DimensionConverter.Editor
{
    [CustomEditor(typeof(SphereConverter))]
    [CanEditMultipleObjects]
    public class SphereConverterEditor : UnityEditor.Editor
    {
        private void Awake()
        {
            if(targets.Cast<Component>().Any(c => c.GetComponent<SphereCollider>() || c.GetComponent<CircleCollider2D>()))
            {
                EditorUtility.DisplayDialog($"Move Colliders", $"Colliders on this object will be moved to Transform3D for 3D Colliders and Transform2D for 2D Colliders", "ok", "ok", DialogOptOutDecisionType.ForThisMachine, "SphereConverterEditorKey");
            }

            foreach(SphereConverter sphereConverter in targets)
            {
                foreach(SphereCollider sphereCollider in sphereConverter.GetComponents<SphereCollider>())
                {
                    var newSphereCollider = sphereConverter.transformSplitter.gameObject3D.AddComponent<SphereCollider>();
                    sphereCollider.ToSphereCollider(newSphereCollider);

                    ReferenceUtility.ChangeReferences(sphereCollider, newSphereCollider);
                    EditorApplication.delayCall += () => DestroyImmediate(sphereCollider);
                }

                foreach(CircleCollider2D circleCollider2D in sphereConverter.GetComponents<CircleCollider2D>())
                {
                    var newCircleCollider2D = sphereConverter.transformSplitter.gameObject2D.AddComponent<CircleCollider2D>();
                    circleCollider2D.ToCircleCollider2D(newCircleCollider2D);

                    ReferenceUtility.ChangeReferences(circleCollider2D, newCircleCollider2D);
                    EditorApplication.delayCall += () => DestroyImmediate(circleCollider2D);
                }
            }
        }
    }
}
