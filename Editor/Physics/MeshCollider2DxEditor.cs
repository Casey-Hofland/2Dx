using System.Linq;
using Unity2Dx.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity2Dx.Physics.Editor
{
    [CustomEditor(typeof(MeshCollider2Dx))]
    [CanEditMultipleObjects]
    public class MeshCollider2DxEditor : UnityEditor.Editor
    {
        private void Awake()
        {
            if(targets.Cast<Component>().Any(c => c.GetComponent<MeshCollider>() || c.GetComponent<PolygonCollider2D>())) 
            {
                EditorUtility.DisplayDialog($"Move Colliders", $"Colliders on this object will be moved to Transform3D for 3D Colliders and Transform2D for 2D Colliders", "ok", "ok", DialogOptOutDecisionType.ForThisMachine, "MeshConverterEditorKey");
            }

            foreach(MeshCollider2Dx meshCollider2Dx in targets)
            {
                foreach(MeshCollider meshCollider in meshCollider2Dx.GetComponents<MeshCollider>())
                {
                    var newMeshCollider = meshCollider2Dx.transform2Dx.gameObject3D.AddComponent<MeshCollider>();
                    meshCollider.ToMeshCollider(newMeshCollider);

                    ReferenceUtility.ChangeReferences(meshCollider, newMeshCollider);
                    EditorApplication.delayCall += () => DestroyImmediate(meshCollider);
                }

                foreach(PolygonCollider2D polygonCollider2D in meshCollider2Dx.GetComponents<PolygonCollider2D>())
                {
                    var newPolygonCollider2D = meshCollider2Dx.transform2Dx.gameObject2D.AddComponent<PolygonCollider2D>();
                    polygonCollider2D.ToPolygonCollider2D(newPolygonCollider2D);

                    ReferenceUtility.ChangeReferences(polygonCollider2D, newPolygonCollider2D);
                    EditorApplication.delayCall += () => DestroyImmediate(polygonCollider2D);
                }
            }
        }
    }
}
