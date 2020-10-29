using DimensionConverter.Utilities;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DimensionConverter.Editor
{
    [CustomEditor(typeof(MeshConverter))]
    [CanEditMultipleObjects]
    public class MeshConverterEditor : UnityEditor.Editor
    {
        private void Awake()
        {
            if(targets.Cast<Component>().Any(c => c.GetComponent<MeshCollider>() || c.GetComponent<PolygonCollider2D>())) 
            {
                EditorUtility.DisplayDialog($"Move Colliders", $"Colliders on this object will be moved to Transform3D for 3D Colliders and Transform2D for 2D Colliders", "ok", "ok", DialogOptOutDecisionType.ForThisMachine, "MeshConverterEditorKey");
            }

            foreach(MeshConverter meshConverter in targets)
            {
                foreach(MeshCollider meshCollider in meshConverter.GetComponents<MeshCollider>())
                {
                    var newMeshCollider = meshConverter.transformSplitter.gameObject3D.AddComponent<MeshCollider>();
                    meshCollider.ToMeshCollider(newMeshCollider);

                    ReferenceUtility.ChangeReferences(meshCollider, newMeshCollider);
                    EditorApplication.delayCall += () => DestroyImmediate(meshCollider);
                }

                foreach(PolygonCollider2D polygonCollider2D in meshConverter.GetComponents<PolygonCollider2D>())
                {
                    var newPolygonCollider2D = meshConverter.transformSplitter.gameObject2D.AddComponent<PolygonCollider2D>();
                    polygonCollider2D.ToPolygonCollider2D(newPolygonCollider2D);

                    ReferenceUtility.ChangeReferences(polygonCollider2D, newPolygonCollider2D);
                    EditorApplication.delayCall += () => DestroyImmediate(polygonCollider2D);
                }
            }
        }
    }
}
