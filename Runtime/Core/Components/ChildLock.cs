#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity2Dx
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class ChildLock : MonoBehaviour, ISerializationCallbackReceiver
    {
        private Dictionary<(Component @lock, string id), Transform> children = new Dictionary<(Component @lock, string id), Transform>();

        [SerializeField] [HideInInspector] private Component[] _locks = Array.Empty<Component>();
        [SerializeField] [HideInInspector] private string[] _ids = Array.Empty<string>();
        [SerializeField] [HideInInspector] private Transform[] _children = Array.Empty<Transform>();

        public void OnBeforeSerialize()
        {
            UpdateLocks();

            //if (!Application.IsPlaying(this) && children.Count == 0)
            //{
            //    DestroyImmediate(this);
            //}

            _locks = new Component[children.Count];
            _ids = new string[children.Count];
            _children = new Transform[children.Count];

            int index = 0;
            foreach (var child in children)
            {
                _locks[index] = child.Key.@lock;
                _ids[index] = child.Key.id;
                _children[index] = child.Value;

                index++;
            }
        }

        public void OnAfterDeserialize()
        {
            children.Clear();

            for (int i = 0; i < _ids.Length; i++)
            {
                var @lock = _locks[i];
                var id = _ids[i];
                var transform = _children[i];

                var key = (@lock, id);

                children.Add(key, transform);
            }
        }

        public Transform GetChild(Component @lock, string id)
        {
            var key = (@lock, id);

            if (children.TryGetValue(key, out var child))
            {
                return child;
            }

            children.Add(key, child = CreateChild(transform, id));
            return child;
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

        private void OnTransformChildrenChanged()
        {
            Invoke(nameof(UpdateLocks), Time.fixedUnscaledDeltaTime);
            Invoke(nameof(UpdateChildren), Time.fixedUnscaledDeltaTime);
        }

        private void UpdateLocks()
        {
            var keys = children.Keys.ToArray();

            foreach (var key in keys)
            {
                if (!key.@lock)
                {
                    if (children[key])
                    {
                        DestroyImmediate(children[key].gameObject);
                    }
                    children.Remove(key);
                }
            }
        }

        private void UpdateChildren()
        {
            var keys = children.Keys.ToArray();

            foreach (var key in keys)
            {
                if (!children[key])
                {
                    children[key] = CreateChild(transform, key.id);
                }

                children[key].SetParent(transform, false);
            }
        }
    }
}
