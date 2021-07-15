using System;
using System.Collections;
using Unity2Dx;
using UnityEngine;

[AddComponentMenu("2Dx/Audio Transform 2Dx")]
[DisallowMultipleComponent]
[ExecuteAlways]
public class AudioTransform2Dx : MonoBehaviour, ISerializationCallbackReceiver
{
    #region Properties
    private static AudioListener _activeAudioListener;
    private static AudioListener activeAudioListener
    {
        get
        {
            if (!_activeAudioListener
                || !_activeAudioListener.isActiveAndEnabled)
            {
                var audioListeners = FindObjectsOfType<AudioListener>(false);
                _activeAudioListener = Array.Find(audioListeners, audioListener => audioListener.enabled);
            }

            return _activeAudioListener;
        }
    }

    /// <include file='../Documentation.xml' path='docs/AudioTransform2Dx/audioGameObject/*' />
    public GameObject audioGameObject { get; private set; }
    /// <include file='../Documentation.xml' path='docs/AudioTransform2Dx/audioTransform/*' />
    public Transform audioTransform => audioGameObject.transform;

    [Tooltip("How to blend from 3D audio position to 2D audio position and back.")]
    [SerializeField] private AnimationCurve _blendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    #endregion

    #region Serialization
    [HideInInspector] [SerializeField] private GameObject _audioGameObject;
    [HideInInspector] [SerializeField] private int audioSiblingIndex = -1;

#if UNITY_EDITOR
    [HideInInspector] [SerializeField] private bool isDuplicate = true;
#endif

    public void OnBeforeSerialize()
    {
        if (audioGameObject)
        {
            audioSiblingIndex = (_audioGameObject = audioGameObject).transform.GetSiblingIndex();
        }
    }

    public void OnAfterDeserialize()
    {
        if (_audioGameObject)
        {
            audioGameObject = _audioGameObject;
        }

#if UNITY_EDITOR
        if (isDuplicate)
        {
            UnityEditor.EditorApplication.delayCall += OnDuplicate;
        }
#endif
    }

#if UNITY_EDITOR
    private void OnDuplicate()
    {
        DestroyImmediate(transform.GetChild(audioSiblingIndex).gameObject);
    }
#endif
    #endregion

    #region Unity Methods
    private void Awake()
    {
#if UNITY_EDITOR
        isDuplicate = false;
#endif

        if (!audioGameObject)
        {
            audioGameObject = CreateGameObject("Audio Transform");
        }
    }

    private void OnEnable()
    {
        if (Application.IsPlaying(gameObject))
        {
            Dimension.onBeforeConvert += ChangeDepth;
            SetDepth(Dimension.is2DNot3D);
        }
    }

    private void OnDisable()
    {
        if (Application.IsPlaying(gameObject))
        {
            Dimension.onBeforeConvert -= ChangeDepth;
        }
    }

    private void LateUpdate()
    {
        if (!Dimension.isConverting)
        {
            SetDepth(Dimension.is2DNot3D);
        }
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        if (Application.IsPlaying(gameObject))
        {
            Destroy(audioGameObject);
        }
        else
        {
            DestroyImmediate(audioGameObject);
        }
#else
            Destroy(audioGameObject);
#endif
    }
    #endregion

    private GameObject CreateGameObject(string name)
    {
#if UNITY_EDITOR
        var gameObject = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(name, HideFlags.None);
        var currentUndoGroup = UnityEditor.Undo.GetCurrentGroupName();
        UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, currentUndoGroup);
#else
            var gameObject = new GameObject(name);
#endif
        gameObject.transform.SetParent(transform, false);

        gameObject.layer = this.gameObject.layer;
        gameObject.tag = this.gameObject.tag;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var staticEditorFlags = UnityEditor.GameObjectUtility.GetStaticEditorFlags(this.gameObject);
            UnityEditor.GameObjectUtility.SetStaticEditorFlags(gameObject, staticEditorFlags);
        }
#endif

        return gameObject;
    }


    /// <include file='../Documentation.xml' path='docs/AudioTransform2Dx/SetDepth/*' />
    public void SetDepth(bool toAudioListenerNotSelf)
    {
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
            StartCoroutine(LerpDepth(toAudioListenerNotSelf));
        }
        else
        {
            SetDepth(toAudioListenerNotSelf);
        }
    }

    private IEnumerator LerpDepth(bool toAudioListenerNotSelf)
    {
        var conversionTime = Dimension.conversionTime;

        if (conversionTime > 0f)
        {
            var lerpMultiplier = 1f / conversionTime;

            while (conversionTime > 0f)
            {
                conversionTime -= Time.unscaledDeltaTime;

                var t = conversionTime * lerpMultiplier;
                var blend = toAudioListenerNotSelf
                    ? _blendCurve.Evaluate(t)
                    : _blendCurve.Evaluate(1f - t);

                var audioPosition = transform.position;
                audioPosition.z = Mathf.Lerp(activeAudioListener.transform.position.z, transform.position.z, blend);
                audioTransform.position = audioPosition;

                yield return null;
            }
        }

        SetDepth(toAudioListenerNotSelf);
    }

    #region Validation
    private void OnTransformChildrenChanged()
    {
        Invoke(nameof(Awake), Time.fixedUnscaledDeltaTime);
    }
    #endregion
}
