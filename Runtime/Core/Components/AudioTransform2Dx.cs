#nullable enable
using System;
using System.Collections;
using UnityEngine;

namespace Unity2Dx
{
    [AddComponentMenu("2Dx/Audio Transform 2Dx")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class AudioTransform2Dx : Convertible
    {
        private static AudioListener? _activeAudioListener;
        private static AudioListener activeAudioListener
        {
            get
            {
                if (_activeAudioListener == null
                    || !_activeAudioListener.isActiveAndEnabled)
                {
                    var audioListeners = FindObjectsOfType<AudioListener>(false);
                    _activeAudioListener = Array.Find(audioListeners, audioListener => audioListener.enabled);
                }

                return _activeAudioListener;
            }
        }

        [SerializeField][HideInInspector] private ChildLock _audioTransform = new ChildLock("Audio Transform");
        public Transform audioTransform => _audioTransform.GetChild(transform);

        public override GameObject gameObject3D => gameObject;
        public override GameObject gameObject2D => audioTransform.gameObject;

        [field: SerializeField]
        public float blendTime { get; set; }

        [field: Tooltip("Curve to blend from 3D audio position to 2D audio position and back.")]
        [field: SerializeField]
        public AnimationCurve blendCurve { get; set; } = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Coroutine? blendRoutine;

        private void LateUpdate()
        {
            if (blendRoutine == null)
            {
                SetDepth(is2DNot3D);
            }
        }

        private void OnDestroy()
        {
            DestroyImmediate(_audioTransform.gameObject);
        }

        protected override void OnConvert(bool convertTo2DNot3D)
        {
            ChangeDepth(convertTo2DNot3D);
        }

        /// <include file='../Documentation.xml' path='docs/AudioTransform2Dx/SetDepth/*' />
        public void SetDepth(bool toAudioListenerNotSelf)
        {
            if (blendRoutine != null)
            {
                StopCoroutine(blendRoutine);
                blendRoutine = null;
            }

            if (toAudioListenerNotSelf)
            {
                var audioPosition = transform.position;
                audioPosition.z = activeAudioListener.transform.position.z;
                audioTransform.position = audioPosition;
            }
            else
            {
                audioTransform.position = transform.position;
            }
        }

        /// <include file='../Documentation.xml' path='docs/AudioTransform2Dx/ChangeDepth/*' />
        public void ChangeDepth(bool toAudioListenerNotSelf)
        {
            if (isActiveAndEnabled)
            {
                if (blendRoutine != null)
                {
                    StopCoroutine(blendRoutine);
                }
                blendRoutine = StartCoroutine(LerpDepth(toAudioListenerNotSelf));
            }
            else
            {
                SetDepth(toAudioListenerNotSelf);
            }
        }

        private IEnumerator LerpDepth(bool toAudioListenerNotSelf)
        {
            var blendTime = this.blendTime;

            if (blendTime > 0f)
            {
                var lerpMultiplier = 1f / blendTime;

                while (blendTime > 0f)
                {
                    blendTime -= Time.unscaledDeltaTime;

                    var t = blendTime * lerpMultiplier;
                    var blend = toAudioListenerNotSelf
                        ? blendCurve.Evaluate(t)
                        : blendCurve.Evaluate(1f - t);

                    var audioPosition = transform.position;
                    audioPosition.z = Mathf.Lerp(activeAudioListener.transform.position.z, transform.position.z, blend);
                    audioTransform.position = audioPosition;

                    yield return null;
                }
            }

            SetDepth(toAudioListenerNotSelf);
        }
    }
}
