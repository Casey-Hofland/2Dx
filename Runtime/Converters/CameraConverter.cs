using DimensionConverter.Utilities;
using System.Collections;
using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Camera Converter")]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class CameraConverter : MonoBehaviour
    {
        #region Required Components
        private Camera _camera;
        public new Camera camera => _camera ? _camera : (_camera = GetComponent<Camera>());
        #endregion

        [Tooltip("Orthographic to perspective view blends from left to right, and perspective to orthographic view blends from right to left, within range 0 to 1.")] 
        public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        [Header("Perspective Projection")]
        [Tooltip("The camera's view angle measured in degrees along the selected axis.")] [Range(Vector2.kEpsilon, 179f)] 
        public float fieldOfView = 60f;

        [Tooltip("Enables Physical camera mode. When checked, the field of view is calculated from properties for simulating physical attributes (focal length, senzor size and lens shift).")] 
        public bool usePhysicalProperties = false;

        [Tooltip("The simulated distance between the lens and the sensor of the physical camera. Larger values give a narrower field of view.")]
        public float focalLength = 50f;

        [Tooltip("The size of the camera sensor in millimeters.")]
        [Min(0.1f)] 
        public Vector2 sensorSize = new Vector2(36f, 24f);

        [Tooltip("Offset from the camera sensor. Use these properties to simulate a shift lens. Measured as a multiple of the sensor size.")]
        public Vector2 lensShift = Vector2.zero;

        [Tooltip("Determines how the rendered area (resolution gate) fits within the sensor area (film gate).")]
        public Camera.GateFitMode gateFit = Camera.GateFitMode.Horizontal;

        [Header("Orthographic Projection")]
        [Tooltip("The vertical size of the camera view.")] 
        public float orthographicSize = 5f;

        private Matrix4x4 perspectiveMatrix => Matrix4x4.Perspective(fieldOfView, camera.aspect, camera.nearClipPlane, camera.farClipPlane);
        private Matrix4x4 orthographicMatrix => Matrix4x4.Ortho(-orthographicSize * camera.aspect, orthographicSize * camera.aspect, -orthographicSize, orthographicSize, camera.nearClipPlane, camera.farClipPlane);

        private void OnEnable()
        {
            SetView(Dimension.is2DNot3D);
            Dimension.onBeforeConvert += ChangeView;
        }

        private void OnDisable()
        {
            Dimension.onBeforeConvert -= ChangeView;
        }

        /// <include file='../Documentation.xml' path='docs/CameraConverter/SetView/*' />
        public void SetView(bool toOrthographicNotPerspective)
        {
            if(toOrthographicNotPerspective)
            {
                camera.orthographic = true;
                camera.orthographicSize = orthographicSize;
            }
            else
            {
                camera.orthographic = false;

                if(camera.usePhysicalProperties = usePhysicalProperties)
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

        /// <include file='../Documentation.xml' path='docs/CameraConverter/ChangeView/*' />
        public void ChangeView(bool toOrthographicNotPerspective)
        {
            if(isActiveAndEnabled)
            {
                StartCoroutine(LerpView(toOrthographicNotPerspective));
            }
            else
            {
                SetView(toOrthographicNotPerspective);
            }
        }

        private IEnumerator LerpView(bool toOrthographicNotPerspective)
        {
            var conversionTime = Dimension.conversionTime;

            if(conversionTime > 0f)
            {
                var lerpMultiplier = 1 / conversionTime;
                camera.orthographic = false;

                while(conversionTime > 0f)
                {
                    conversionTime -= Time.unscaledDeltaTime;

                    var t = conversionTime * lerpMultiplier;
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
