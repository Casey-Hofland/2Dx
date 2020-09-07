using System;
using System.Collections;
using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Transform Splitter")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class TransformSplitter : MonoBehaviour
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
        #region Properties
        private static readonly Quaternion zRotation90Deg = new Quaternion(0f, 0f, 0.7071068f, 0.7071068f);

#pragma warning disable CS0649
        [Tooltip("Determines the axis to align in 2D space. If Horizontal, X axis will be aligned, and if vertical, the Y axis.")] [SerializeField] private CapsuleDirection2D direction2D;
#pragma warning restore CS0649

        public GameObject gameObject3D { get; private set; }
        public Transform transform3D => gameObject3D.transform;

        public GameObject gameObject2D { get; private set; }
        public Transform transform2D => gameObject2D.transform;

        private Vector3 upwardDirection
        {
            get
            {
                switch(direction2D)
                {
                    case CapsuleDirection2D.Vertical:
                        return transform.up;
                    case CapsuleDirection2D.Horizontal:
                        return zRotation90Deg * transform.right;
                    default:
                        return default;
                }
            }
        }

#if UNITY_EDITOR
        private GameObject GetGameObject(string name, int siblingIndex)
        {
            var gameObject = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(name, HideFlags.None);
            gameObject.transform.SetParent(transform, false);
            if(siblingIndex > -1)
            {
                gameObject.transform.SetSiblingIndex(siblingIndex);
            }

            gameObject.layer = this.gameObject.layer;
            gameObject.tag = this.gameObject.tag;

            if(!Application.isPlaying)
            {
                var staticEditorFlags = UnityEditor.GameObjectUtility.GetStaticEditorFlags(this.gameObject);
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(gameObject, staticEditorFlags);
            }

            return gameObject;
        }
#endif
        #endregion

        #region Serialization
#if UNITY_EDITOR
        [HideInInspector] [SerializeField] private GameObject _gameObject3D;
        [HideInInspector] [SerializeField] private GameObject _gameObject2D;
        private int siblingIndex3D = -1;
        private int siblingIndex2D = -1;
        private bool isDuplicate = true;

        public void OnBeforeSerialize() 
        {
            if(gameObject3D)
            {
                _gameObject3D = gameObject3D;
            }
            if(gameObject2D)
            {
                _gameObject2D = gameObject2D;
            }
        }

        public void OnAfterDeserialize()
        {
            if(_gameObject3D)
            {
                gameObject3D = _gameObject3D;
            }
            if(_gameObject2D)
            {
                gameObject2D = _gameObject2D;
            }

            if(isDuplicate)
            {
                UnityEditor.EditorApplication.delayCall += OnDuplicate;
                UnityEditor.EditorApplication.delayCall += Reset;
            }

            UnityEditor.EditorApplication.playModeStateChanged -= DestroyWithRequireComponent;
            UnityEditor.EditorApplication.playModeStateChanged += DestroyWithRequireComponent;
        }
       
        private void OnDuplicate()
        {
            siblingIndex3D = _gameObject3D.transform.GetSiblingIndex();
            siblingIndex2D = _gameObject2D.transform.GetSiblingIndex();

            DestroyImmediate(transform.GetChild(Mathf.Max(siblingIndex2D, siblingIndex3D)).gameObject);
            DestroyImmediate(transform.GetChild(Mathf.Min(siblingIndex2D, siblingIndex3D)).gameObject);
        }

        private void Reset()
        {
            isDuplicate = false;

            if(!gameObject3D)
            {
                gameObject3D = GetGameObject("Transform3D", (siblingIndex2D < siblingIndex3D) ? siblingIndex3D - 1 : siblingIndex3D);
            }
            if(!gameObject2D)
            {
                gameObject2D = GetGameObject("Transform2D", siblingIndex2D);
            }

            gameObject3D.layer = gameObject2D.layer = gameObject.layer;
            gameObject3D.tag = gameObject2D.tag = gameObject.tag;

            if(!Application.isPlaying)
            {
                var staticEditorFlags = UnityEditor.GameObjectUtility.GetStaticEditorFlags(gameObject);
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(gameObject3D, staticEditorFlags);
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(gameObject2D, staticEditorFlags);
            }
        }

        private void DestroyWithRequireComponent(UnityEditor.PlayModeStateChange playModeStateChange)
        {
            if(playModeStateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                var components = GetComponents<Component>();
                var requiredComponentsArray = new RequireComponent[components.Length][];
                for(int i = 0; i < components.Length; i++)
                {
                    var component = components[i];
                    requiredComponentsArray[i] = Attribute.GetCustomAttributes(component.GetType(), typeof(RequireComponent), true) as RequireComponent[];
                }

                DestroyWithRequireComponentOfType(typeof(TransformSplitter));
                UnityEditor.EditorApplication.playModeStateChanged -= DestroyWithRequireComponent;

                void DestroyWithRequireComponentOfType(Type type)
                {
                    for(int i = 0; i < components.Length; i++)
                    {
                        var component = components[i];
                        Type componentType;
                        if(!component || (componentType = component.GetType()) == type)
                        {
                            continue;
                        }

                        var requiredComponents = requiredComponentsArray[i];
                        if(Array.Exists(requiredComponents, requiredComponent => requiredComponent.m_Type0 == type || requiredComponent.m_Type1 == type || requiredComponent.m_Type2 == type))
                        {
                            DestroyWithRequireComponentOfType(componentType);
                            DestroyImmediate(component);
                        }
                    }
                }
            }
        }
#endif
        #endregion

        #region Validation
        private void OnValidate()
        {
            transform.hasChanged = true;
        }

        private IEnumerator OnTransformChildrenChanged()
        {
            yield return null;

            if(!gameObject3D)
            {
#if UNITY_EDITOR
                if(Application.isPlaying)
                {
                    throw new MissingReferenceException($"Required child {gameObject3D} was removed from {gameObject}.");
                }
                else
                {
                    gameObject3D = GetGameObject("Transform3D", (siblingIndex2D < siblingIndex3D) ? siblingIndex3D - 1 : siblingIndex3D);
                    Debug.LogWarning($"Child {gameObject3D.name} is required. To remove it, destroy the parent's {nameof(TransformSplitter)}.");
                }
#else
                throw new MissingReferenceException($"Required child {gameObject3D} was removed from {gameObject}.");
#endif
            }
            if(!gameObject2D)
            {
#if UNITY_EDITOR
                if(Application.isPlaying)
                {
                    throw new MissingReferenceException($"Required child {gameObject2D} was removed from {gameObject}.");
                }
                else
                {
                    gameObject2D = GetGameObject("Transform2D", siblingIndex2D);
                    Debug.LogWarning($"Child {gameObject2D.name} is required. To remove it, destroy the parent's {nameof(TransformSplitter)}.");
                }
#else
                throw new MissingReferenceException($"Required child {gameObject2D} was removed from {gameObject}.");
#endif
            }
        }
        #endregion

        #region Unity Methods
        // Make sure the required gameObjects are always at the same position, (2D) rotation and scale as the parent transform.
        private void LateUpdate()
        {
            if(!transform.hasChanged)
            {
                if(transform3D.hasChanged)
                {
                    UpdateTransform3D();
                }

                if(transform2D.hasChanged)
                {
                    UpdateTransform2D();
                }
            }
            else
            {
                UpdateTransform3D();
                UpdateTransform2D();
                transform.hasChanged = false;
            }

            void UpdateTransform3D()
            {
                transform3D.SetPositionAndRotation(transform.position, transform.rotation);
                transform3D.localScale = Vector3.one;
                transform3D.hasChanged = false;
            }

            void UpdateTransform2D()
            {
                transform2D.SetPositionAndRotation(transform.position, Quaternion.LookRotation(Vector3.forward, upwardDirection));
                transform2D.localScale = Vector3.one;
                transform2D.hasChanged = false;
            }
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
            {
                Destroy(gameObject3D);
                Destroy(gameObject2D);
            }
            else
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(gameObject3D);
                    DestroyImmediate(gameObject2D);
                };
            }
#else
            Destroy(gameObject3D);
            Destroy(gameObject2D);
#endif
        }
        #endregion
    }
}
