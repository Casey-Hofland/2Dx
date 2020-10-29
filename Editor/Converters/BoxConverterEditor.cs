using DimensionConverter.Utilities;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DimensionConverter.Editor
{
    [CustomEditor(typeof(BoxConverter))]
    [CanEditMultipleObjects]
    public class BoxConverterEditor : UnityEditor.Editor
    {
        private void Awake()
        {
            if(targets.Cast<Component>().Any(c => c.GetComponent<BoxCollider>() || c.GetComponent<BoxCollider2D>() || c.GetComponent<PolygonCollider2D>()))
            {
                EditorUtility.DisplayDialog($"Move Colliders", $"Colliders on this object will be moved to Transform3D for 3D Colliders and Transform2D for 2D Colliders", "ok", "ok", DialogOptOutDecisionType.ForThisMachine, "BoxConverterEditorKey");
            }

            foreach(BoxConverter boxConverter in targets)
            {
                foreach(BoxCollider boxCollider in boxConverter.GetComponents<BoxCollider>())
                {
                    var newBoxCollider = boxConverter.transformSplitter.gameObject3D.AddComponent<BoxCollider>();
                    boxCollider.ToBoxCollider(newBoxCollider);

                    ReferenceUtility.ChangeReferences(boxCollider, newBoxCollider);
                    EditorApplication.delayCall += () => DestroyImmediate(boxCollider);
                }

                foreach(BoxCollider2D boxCollider2D in boxConverter.GetComponents<BoxCollider2D>())
                {
                    var newBoxCollider2D = boxConverter.transformSplitter.gameObject2D.AddComponent<BoxCollider2D>();
                    boxCollider2D.ToBoxCollider2D(newBoxCollider2D);

                    ReferenceUtility.ChangeReferences(boxCollider2D, newBoxCollider2D);
                    EditorApplication.delayCall += () => DestroyImmediate(boxCollider2D);
                }

                foreach(PolygonCollider2D polygonCollider2D in boxConverter.GetComponents<PolygonCollider2D>())
                {
                    var newPolygonCollider2D = boxConverter.transformSplitter.gameObject2D.AddComponent<PolygonCollider2D>();
                    polygonCollider2D.ToPolygonCollider2D(newPolygonCollider2D);

                    ReferenceUtility.ChangeReferences(polygonCollider2D, newPolygonCollider2D);
                    EditorApplication.delayCall += () => DestroyImmediate(polygonCollider2D);
                }
            }
        }
    }
}
