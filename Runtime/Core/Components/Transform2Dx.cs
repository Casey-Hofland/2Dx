#nullable enable
using UnityEngine;

namespace Unity2Dx
{
    [AddComponentMenu("2Dx/Transform 2Dx")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ChildLock))]
    [ExecuteAlways]
    public sealed class Transform2Dx : Convertible
    {
        #region Properties
        private static readonly Quaternion zRotation90Deg = new Quaternion(0f, 0f, 0.7071068f, 0.7071068f);

        private ChildLock? _childLock;
        public ChildLock childLock => _childLock ? _childLock! : (_childLock = GetComponent<ChildLock>());

        private Transform? _transform3D;
        public Transform transform3D => _transform3D ? _transform3D! : (_transform3D = childLock.GetChild(this, "Transform 3D"));
        public override GameObject gameObject3D => transform3D.gameObject;

        private Transform? _transform2D;
        public Transform transform2D => _transform2D ? _transform2D! : (_transform2D = childLock.GetChild(this, "Transform 2D"));
        public override GameObject gameObject2D => transform2D.gameObject;


        [field: Tooltip("Controls when the transforms will be updated.")]
        [field: SerializeField]
        public UpdateMode updateMode { get; set; }

        [Tooltip("Determines the axis to align in 2D space. If Horizontal, X axis will be aligned, and if vertical, the Y axis.")]
        [SerializeField]
        private CapsuleDirection2D _direction2D = default;

        /// <include file='../Documentation.xml' path='docs/Transform2Dx/direction2D/*' />
        public CapsuleDirection2D direction2D
        {
            get => _direction2D;
            set
            {
                _direction2D = value;
                transform2D.hasChanged = true;
            }
        }

        private Vector3 upwards2D
        {
            get
            {
                switch (direction2D)
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

        private void Awake()
        {
            //WorldLock.worldRotated += WorldRotated;
        }

        private void OnDestroy()
        {
            DestroyImmediate(gameObject3D);
            DestroyImmediate(gameObject2D);

            //WorldLock.worldRotated -= WorldRotated;
        }

        private void LateUpdate()
        {
            if (updateMode == UpdateMode.LateUpdate ||
                (!Application.IsPlaying(gameObject) && updateMode != UpdateMode.Custom))
            {
                UpdateTransforms();
            }
        }

        private void FixedUpdate()
        {
            if (updateMode == UpdateMode.FixedUpdate)
            {
                UpdateTransforms();
            }
        }

        private void OnValidate()
        {
            // Validation check because of warning "SendMessage cannot be called during Awake, CheckConsistency, or OnValidate"
            if (_transform2D != null)
            {
                _transform2D.hasChanged = true;
            }
        }

        protected override void OnConvert(bool convertTo2DNot3D)
        {
            gameObject3D.SetActive(!convertTo2DNot3D);
            gameObject2D.SetActive(convertTo2DNot3D);
        }

        private void WorldRotated(Vector3 point, Quaternion rotation)
        {
            UpdateTransforms();
        }

        /// <include file='../Documentation.xml' path='docs/Transform2Dx/UpdateTransforms/*' />
        [ContextMenu("Update Transforms")]
        public void UpdateTransforms()
        {
            if (!transform.hasChanged)
            {
                if (transform3D.hasChanged)
                {
                    UpdateTransform3D();
                }

                if (transform2D.hasChanged)
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
    }
}
