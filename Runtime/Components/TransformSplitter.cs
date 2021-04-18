using System;
using System.Collections;
using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Transform Splitter")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class TransformSplitter : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region Properties
        private static readonly Quaternion zRotation90Deg = new Quaternion(0f, 0f, 0.7071068f, 0.7071068f);

        [Tooltip("Controls when the transforms will be updated.")] [SerializeField] private UpdateMode _updateMode;
        [Tooltip("Determines the axis to align in 2D space. If Horizontal, X axis will be aligned, and if vertical, the Y axis.")] [SerializeField] private CapsuleDirection2D _direction2D = default;
        [Tooltip("Activate and deactivate the split transforms on a dimension convert.")] [SerializeField] private bool _convert = true;

        public GameObject gameObject3D { get; private set; }
        public Transform transform3D => gameObject3D.transform;

        public GameObject gameObject2D { get; private set; }
        public Transform transform2D => gameObject2D.transform;

        public UpdateMode updateMode
        {
            get => _updateMode;
            set => _updateMode = value;
        }

        public CapsuleDirection2D direction2D
        {
            get => _direction2D;
            set
            {
                _direction2D = value;
                transform2D.hasChanged = true;
            }
        }

        public bool convert
        {
            get => _convert;
            set
            {
                if(_convert != value)
                {
                    if(Application.IsPlaying(gameObject))
                    {
                        if(convert)
                        {
                            ConvertTransforms(Dimension.is2DNot3D);
                            Dimension.onBeforeConvert += ConvertTransforms;
                        }
                        else
                        {
                            Dimension.onBeforeConvert -= ConvertTransforms;
                        }
                    }
                }
                _convert = value;
            }
        }

        private Vector3 upwards2D
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

        public enum UpdateMode
        {
            LateUpdate,
            FixedUpdate,
            Custom,
        }
        #endregion

        #region Serialization
        [HideInInspector] [SerializeField] private GameObject _gameObject3D;
        [HideInInspector] [SerializeField] private GameObject _gameObject2D;
        [HideInInspector] [SerializeField] private int siblingIndex3D = -1;
        [HideInInspector] [SerializeField] private int siblingIndex2D = -1;

#if UNITY_EDITOR
        [HideInInspector] [SerializeField] private bool isDuplicate = true;
#endif

        public void OnBeforeSerialize()
        {
            if(gameObject3D)
            {
                siblingIndex3D = (_gameObject3D = gameObject3D).transform.GetSiblingIndex();
            }
            if(gameObject2D)
            {
                siblingIndex2D = (_gameObject2D = gameObject2D).transform.GetSiblingIndex();
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

#if UNITY_EDITOR
            if(isDuplicate) 
            {
                UnityEditor.EditorApplication.delayCall += OnDuplicate;
            }
#endif
        }

#if UNITY_EDITOR
        private void OnDuplicate()
        {
            DestroyImmediate(transform.GetChild(Mathf.Max(siblingIndex2D, siblingIndex3D)).gameObject);
            DestroyImmediate(transform.GetChild(Mathf.Min(siblingIndex2D, siblingIndex3D)).gameObject);
        }
#endif
        #endregion

        #region Unity Methods
        private void Awake()
        {
#if UNITY_EDITOR
            isDuplicate = false;
#endif

            if(!gameObject3D)
            {
                gameObject3D = CreateGameObject("Transform3D", (siblingIndex2D < siblingIndex3D) ? siblingIndex3D - 1 : siblingIndex3D);
            }
            if(!gameObject2D)
            {
                gameObject2D = CreateGameObject("Transform2D", siblingIndex2D);
            }
        }

        private void OnEnable()
        {
            if(Application.IsPlaying(gameObject) && convert)
            {
                ConvertTransforms(Dimension.is2DNot3D);
                Dimension.onBeforeConvert += ConvertTransforms;
            }
        }

        private void OnDisable()
        {
            if(Application.IsPlaying(gameObject) && convert)
            {
                Dimension.onBeforeConvert -= ConvertTransforms;
            }
        }

        private void LateUpdate()
        {
            if(updateMode == UpdateMode.LateUpdate || 
                (!Application.IsPlaying(gameObject) && updateMode != UpdateMode.Custom))
            {
                UpdateTransforms();
            }
        }

        private void FixedUpdate()
        {
            if(updateMode == UpdateMode.FixedUpdate)
            {
                UpdateTransforms();
            }
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if(Application.IsPlaying(gameObject))
            {
                Destroy(gameObject3D);
                Destroy(gameObject2D);
            }
            else
            {
                DestroyImmediate(gameObject3D);
                DestroyImmediate(gameObject2D);
            }
#else
            Destroy(gameObject3D);
            Destroy(gameObject2D);
#endif
        }
#endregion

        private GameObject CreateGameObject(string name, int siblingIndex)
        {
#if UNITY_EDITOR
            var gameObject = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(name, HideFlags.None);
            var currentUndoGroup = UnityEditor.Undo.GetCurrentGroupName();
            UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, currentUndoGroup);
#else
            var gameObject = new GameObject(name);
#endif
            gameObject.transform.SetParent(transform, false);
            if(siblingIndex > -1)
            {
                gameObject.transform.SetSiblingIndex(siblingIndex);
            }

            gameObject.layer = this.gameObject.layer;
            gameObject.tag = this.gameObject.tag;

#if UNITY_EDITOR
            if(!Application.isPlaying)
            {
                var staticEditorFlags = UnityEditor.GameObjectUtility.GetStaticEditorFlags(this.gameObject);
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(gameObject, staticEditorFlags);
            }
#endif

            return gameObject;
        }

        private void ConvertTransforms(bool to2Dnot3D)
        {
            gameObject3D.SetActive(!to2Dnot3D);
            gameObject2D.SetActive(to2Dnot3D);
        }

        // Make sure the required gameObjects are always at the same position, (2D) rotation and scale as the parent transform.
        public void UpdateTransforms()
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
                transform3D.SetParent(transform, false);
                transform3D.SetPositionAndRotation(transform.position, transform.rotation);
                transform3D.localScale = Vector3.one;
                transform3D.hasChanged = false;
            }

            void UpdateTransform2D()
            {
                transform2D.SetParent(transform, false);
                transform2D.SetPositionAndRotation(transform.position, Quaternion.LookRotation(Vector3.forward, upwards2D));
                transform2D.localScale = Vector3.one;
                transform2D.hasChanged = false;
            }
        }

        #region Validation
        private void OnValidate()
        {
            transform2D.hasChanged = true;
        }

        private void OnTransformChildrenChanged()
        {
            Invoke(nameof(Awake), Time.fixedUnscaledDeltaTime);
        }
#endregion
    }
}
