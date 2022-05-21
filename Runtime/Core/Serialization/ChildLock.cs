#nullable enable
using System;
using UnityEngine;
using UnityExtras;

namespace Unity2Dx
{
    [Serializable]
    public struct ChildLock
    {
        [SerializeField][HideInInspector] private NonResetable<Transform?> _nonResetable;
        public Transform? child;
        public GameObject? gameObject => child?.gameObject;

        private string _childName;

        public ChildLock(string childName)
        {
            _nonResetable = default;
            child = null;
            _childName = childName;
        }

        public static implicit operator Transform?(ChildLock childLock) => childLock.child;

        public Transform GetChild(Transform parent)
        {
            if (child == null)
            {
                child = _nonResetable.value ? _nonResetable.value! : CreateChild(parent, _childName);
            }
            else if (child.parent != parent)
            {
                child.SetParent(parent, false);
            }

            return _nonResetable.value = child;
        }

        private static Transform CreateChild(Transform parent, string name)
        {
            var gameObject = new GameObject(name);

            gameObject.transform.SetParent(parent, false);

            gameObject.layer = parent.gameObject.layer;
            gameObject.tag = parent.gameObject.tag;

#if UNITY_EDITOR
            if (!Application.IsPlaying(gameObject))
            {
                var staticEditorFlags = UnityEditor.GameObjectUtility.GetStaticEditorFlags(parent.gameObject);
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(gameObject, staticEditorFlags);

                var currentUndoGroup = UnityEditor.Undo.GetCurrentGroupName();
                UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, currentUndoGroup);
            }
#endif

            return gameObject.transform;
        }
    }

}
