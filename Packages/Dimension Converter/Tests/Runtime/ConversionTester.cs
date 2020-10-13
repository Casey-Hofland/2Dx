using DimensionConverter.Utilities;
using UnityEditor;
using UnityEngine;

namespace DimensionConverter.Tests
{
    public class ConversionTester : MonoBehaviour
    {
        public virtual void ConvertTo2D()
        {
            Undo.SetCurrentGroupName("Convert To 2D");

            foreach(var sphereCollider in GetComponentsInChildren<SphereCollider>(true))
            {
                var gameObject = new GameObject();
                Undo.RegisterCreatedObjectUndo(gameObject, "GameObject creation");

                gameObject.transform.SetParent(sphereCollider.transform, false);
                var circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
                sphereCollider.ToCircleCollider2D(circleCollider2D);

                Undo.DestroyObjectImmediate(sphereCollider);
            }

            Undo.IncrementCurrentGroup();
        }

        public virtual void ConvertTo3D()
        {
            Undo.SetCurrentGroupName("Convert To 2D");

            foreach(var circleCollider2D in GetComponentsInChildren<CircleCollider2D>(true))
            {
                var parent = circleCollider2D.transform.parent;
                
                var sphereCollider = Undo.AddComponent<SphereCollider>(parent.gameObject);
                circleCollider2D.ToSphereCollider(sphereCollider);

                Undo.DestroyObjectImmediate(circleCollider2D.gameObject);
            }

            Undo.IncrementCurrentGroup();
        }

        protected virtual void Reset()
        {
            hideFlags = HideFlags.DontSaveInBuild;
        }
    }
}

