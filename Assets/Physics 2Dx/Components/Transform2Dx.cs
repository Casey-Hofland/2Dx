using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics2DxSystem
{
    [AddComponentMenu(Physics2Dx.componentMenu + "Transform 2Dx")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class Transform2Dx : Module2Dx
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
        #region Properties
        private static readonly Quaternion zRotation90Deg = new Quaternion(0f, 0f, 0.7071068f, 0.7071068f);

        [Tooltip("Determines the axis to align in 2D space. If Horizontal, X axis will be aligned, and if vertical, the Y axis.")] [SerializeField] private CapsuleDirection2D direction2D;

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

        private GameObject GetGameObject(string name, int siblingIndex)
        {
            var gameObject = new GameObject(name);
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
                    Debug.LogWarning($"Child {gameObject3D.name} is required. To remove it, destroy the parent's {nameof(Transform2Dx)}.");
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
                    Debug.LogWarning($"Child {gameObject2D.name} is required. To remove it, destroy the parent's {nameof(Transform2Dx)}.");
                }
#else
                throw new MissingReferenceException($"Required child {gameObject2D} was removed from {gameObject}.");
#endif
            }
        }
        #endregion

        #region Unity Methods
        protected override void OnEnable()
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
            {
                base.OnEnable();
            }
#else
            base.OnEnable();
#endif
        }

        protected override void OnDisable()
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
            {
                try
                {
                    base.OnDisable();
                }
                catch(KeyNotFoundException) { }
            }
#else
            base.OnDisable();
#endif
        }

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

        #region Module2Dx overrides
        public override void ConvertTo2D()
        {
            gameObject3D.SetActive(false);
            gameObject2D.SetActive(true);
        }

        public override void ConvertTo3D()
        {
            gameObject2D.SetActive(false);
            gameObject3D.SetActive(true);
        }
        #endregion
    }
}
