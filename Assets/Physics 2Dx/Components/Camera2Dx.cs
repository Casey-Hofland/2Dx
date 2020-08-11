using Physics2DxSystem.Utilities;
using System.Collections;
using UnityEngine;

namespace Physics2DxSystem
{
    [AddComponentMenu(Physics2Dx.componentMenu + "Camera 2Dx")]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class Camera2Dx : MonoBehaviour
    {
        #region Required Components
        private Camera _camera;
        public new Camera camera => _camera ? _camera : (_camera = GetComponent<Camera>());
        #endregion

        [Tooltip("Orthographic to perspective view blends from left to right, and perspective to orthographic view blends from right to left.")] public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Header("Perspective Projection")]
        [Range(Vector2.kEpsilon, 179f)] public float fieldOfView = 60f;
        [Header("Orthographic Projection")]
        public float orthographicSize = 5f;

        private Matrix4x4 perspectiveMatrix => Matrix4x4.Perspective(fieldOfView, camera.aspect, camera.nearClipPlane, camera.farClipPlane);
        private Matrix4x4 orthographicMatrix => Matrix4x4.Ortho(-orthographicSize * camera.aspect, orthographicSize * camera.aspect, -orthographicSize, orthographicSize, camera.nearClipPlane, camera.farClipPlane);

        private void OnEnable()
        {
            SetView(Physics2Dx.is2Dnot3D);
            Physics2Dx.onBeforeConvert += ChangeView;
        }

        private void OnDisable()
        {
            Physics2Dx.onBeforeConvert -= ChangeView;
        }

        public void SetView(bool toOrthographicNotPerspective)
        {
            if(toOrthographicNotPerspective)
            {
                camera.projectionMatrix = orthographicMatrix;
            }
            else
            {
                camera.projectionMatrix = perspectiveMatrix;
            }
        }

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
            var conversionTime = Physics2Dx.conversionTime;

            if(conversionTime == 0f)
            {
                SetView(toOrthographicNotPerspective);
            }
            else
            {
                var lerpMultiplier = 1 / conversionTime;

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
        }
    }
}
