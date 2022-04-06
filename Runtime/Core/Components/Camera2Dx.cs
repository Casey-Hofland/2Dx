#nullable enable
using System.Collections;
using UnityEngine;

namespace Unity2Dx
{
    [AddComponentMenu("2Dx/Camera 2Dx")]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class Camera2Dx : Convertible
    {
        private const float minSensorSize = 0.1f;

        #region Properties
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        private Camera? _camera;
        public Camera camera => _camera ? _camera! : (_camera = GetComponent<Camera>());
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        public override GameObject gameObject3D => gameObject;
        public override GameObject gameObject2D => gameObject;

        private Matrix4x4 perspectiveMatrix => Matrix4x4.Perspective(fieldOfView, camera.aspect, camera.nearClipPlane, camera.farClipPlane);
        private Matrix4x4 orthographicMatrix => Matrix4x4.Ortho(-orthographicSize * camera.aspect, orthographicSize * camera.aspect, -orthographicSize, orthographicSize, camera.nearClipPlane, camera.farClipPlane);

        [field: Header("Perspective Projection")]
        [field: Tooltip("The camera's view angle measured in degrees along the selected axis.")]
        [field: Range(Vector2.kEpsilon, 179f)]
        [field: SerializeField] public float fieldOfView { get; set; } = 60f;

        [field: Tooltip("Enables Physical camera mode. When checked, the field of view is calculated from properties for simulating physical attributes (focal length, senzor size and lens shift).")]
        [field: SerializeField] private bool usePhysicalProperties { get; set; }

        [field: Tooltip("The simulated distance between the lens and the sensor of the physical camera. Larger values give a narrower field of view.")]
        [field: SerializeField] private float focalLength { get; set; } = 50f;

        [Tooltip("The size of the camera sensor in millimeters.")]
        [Min(minSensorSize)]
        [SerializeField] private Vector2 _sensorSize = new Vector2(36f, 24f);

        public Vector2 sensorSize
        {
            get => _sensorSize;
            set
            {
                _sensorSize.x = Mathf.Max(minSensorSize, value.x);
                _sensorSize.y = Mathf.Max(minSensorSize, value.y);
            }
        }

        [field: Tooltip("Offset from the camera sensor. Use these properties to simulate a shift lens. Measured as a multiple of the sensor size.")]
        [field: SerializeField] private Vector2 lensShift { get; set; }

        [field: Tooltip("Determines how the rendered area (resolution gate) fits within the sensor area (film gate).")]
        [field: SerializeField] private Camera.GateFitMode gateFit { get; set; } = Camera.GateFitMode.Horizontal;

        [field: Header("Orthographic Projection")]
        [field: Tooltip("The vertical size of the camera view.")]
        [field: SerializeField] private float orthographicSize { get; set; } = 5f;

        [field: Header("Blending")]
        [field: SerializeField] public float blendTime { get; set; }

        [field: Tooltip("Curve to blend from perspective camera view to orthographic view and back.")]
        [field: SerializeField] public AnimationCurve blendCurve { get; set; } = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Coroutine? blendRoutine;
        #endregion

        private void LateUpdate()
        {
            if (blendRoutine == null)
            {
                SetView(is2DNot3D);
            }
        }

        protected override void OnConvert(bool convertTo2DNot3D)
        {
            ChangeView(convertTo2DNot3D);
        }

        /// <include file='../Documentation.xml' path='docs/Camera2Dx/SetView/*' />
        public void SetView(bool toOrthographicNotPerspective)
        {
            if (toOrthographicNotPerspective)
            {
                camera.orthographic = true;
                camera.orthographicSize = orthographicSize;
            }
            else
            {
                camera.orthographic = false;

                if (camera.usePhysicalProperties = usePhysicalProperties)
                {
                    camera.focalLength = focalLength;
                    camera.sensorSize = sensorSize;
                    camera.lensShift = lensShift;
                    camera.gateFit = gateFit;

                    fieldOfView = camera.fieldOfView;
                }
                else
                {
                    camera.fieldOfView = fieldOfView;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/Camera2Dx/ChangeView/*' />
        public void ChangeView(bool toOrthographicNotPerspective)
        {
            if (isActiveAndEnabled)
            {
                if (blendRoutine != null)
                {
                    StopCoroutine(blendRoutine);
                }
                blendRoutine = StartCoroutine(LerpView(toOrthographicNotPerspective));
            }
            else
            {
                SetView(toOrthographicNotPerspective);
            }
        }

        private IEnumerator LerpView(bool toOrthographicNotPerspective)
        {
            var blendTime = this.blendTime;

            if (blendTime > 0f)
            {
                var lerpMultiplier = 1 / blendTime;
                camera.orthographic = false;

                while (blendTime > 0f)
                {
                    blendTime -= Time.unscaledDeltaTime;

                    var t = blendTime * lerpMultiplier;
                    var blend = toOrthographicNotPerspective
                        ? blendCurve.Evaluate(t)
                        : blendCurve.Evaluate(1 - t);

                    camera.projectionMatrix = Matrix4x4Helper.LerpUnclamped(orthographicMatrix, perspectiveMatrix, blend);

                    yield return null;
                }
            }

            SetView(toOrthographicNotPerspective);
        }
    }
}
