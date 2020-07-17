using Physics2DxSystem;
using System;
using System.Collections;

namespace UnityEngine
{
    public class Physics2Dx
    {
        internal const string componentMenu = "Physics 2Dx/";

        #region Persistent
        private static Persistent coroutineHandler;

        private class Persistent : MonoBehaviour
        {
            public static bool willDisable = false;

            public static void Init()
            {
                willDisable = false;

                Application.quitting -= SetCanDisableToTrue;
                Application.quitting += SetCanDisableToTrue;
            }

            private static void SetCanDisableToTrue()
            {
                willDisable = true;
            }

            private void OnDisable()
            {
                if(!willDisable)
                {
                    throw new CannotDisableException($"{GetType().Name} may not be disabled except on quit!");
                }
            }

            private class CannotDisableException : Exception
            {
                public CannotDisableException() { }
                public CannotDisableException(string message) : base(message) { }
                public CannotDisableException(string message, Exception innerException) : base(message, innerException) { }
            }
        }

        public static Coroutine StartCoroutine(IEnumerator routine) => Persistent.willDisable ? null : coroutineHandler.StartCoroutine(routine);
        public static void StopCoroutine(Coroutine routine)
        {
            if(!Persistent.willDisable)
            {
                coroutineHandler.StopCoroutine(routine);
            }
        }
        public static void StopCoroutine(IEnumerator routine)
        {
            if(!Persistent.willDisable)
            {
                coroutineHandler.StopCoroutine(routine);
            }
        }
        public static void StopAllCoroutines()
        {
            if(!Persistent.willDisable)
            {
                coroutineHandler.StopAllCoroutines();
            }
        }
        #endregion

        #region Conversion
        private static bool _is2Dnot3D = false;
        /// <include file='./Documentation.xml' path='docs/Physics2Dx/is2Dnot3D/*' />
        public static bool is2Dnot3D
        {
            get => _is2Dnot3D;
            set
            {
                if(isConverting || _is2Dnot3D == value)
                {
                    return;
                }

                StartCoroutine(UpdateIsConverting(value));
                if(splitConversionOverMultipleFrames)
                {
                    StartCoroutine(Module2Dx.ConvertInstancesAsCoroutine(value));
                }
                else
                {
                    Module2Dx.ConvertInstances(value);
                }
            }
        }

        /// <include file='./Documentation.xml' path='docs/Physics2Dx/Convert/*' />
        public static void Convert()
        {
            is2Dnot3D = !is2Dnot3D;
        }

        /// <include file='./Documentation.xml' path='docs/Physics2Dx/onConvert/*' />
        public delegate void OnConvert(bool to2Dnot3D);
        /// <include file='./Documentation.xml' path='docs/Physics2Dx/onConvert/*' />
        public static OnConvert onBeforeConvert { get; set; }
        /// <include file='./Documentation.xml' path='docs/Physics2Dx/onConvert/*' />
        public static OnConvert onAfterConvert { get; set; }

        /// <include file='./Documentation.xml' path='docs/Physics2Dx/waitForConversionTime/*' />
        public static WaitForSecondsRealtime waitForConversionTime { get; private set; }
        /// <include file='./Documentation.xml' path='docs/Physics2Dx/conversionTime/*' />
        public static float conversionTime
        {
            get => waitForConversionTime.waitTime;
            set => waitForConversionTime.waitTime = value;
        }

        /// <include file='./Documentation.xml' path='docs/Physics2Dx/isConverting/*' />
        public static bool isConverting { get; private set; }

        /// <include file='./Documentation.xml' path='docs/Physics2Dx/waitWhileConverting/*' />
        public static WaitWhile waitWhileConverting => new WaitWhile(IsConverting);

        private static bool IsConverting() => isConverting;

        private static IEnumerator UpdateIsConverting(bool to2Dnot3D)
        {
            onBeforeConvert?.Invoke(to2Dnot3D);

            isConverting = true;
            yield return waitForConversionTime;
            isConverting = false;

            _is2Dnot3D = to2Dnot3D;
            onAfterConvert?.Invoke(to2Dnot3D);
        }

        /// <include file='./Documentation.xml' path='docs/Physics2Dx/splitConversionOverMultipleFrames/*' />
        public static bool splitConversionOverMultipleFrames { get; set; }
        #endregion

        #region Debugging
        public static bool slimHierarchy { get; private set; }
        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            onBeforeConvert = onAfterConvert = default;

            var settings = Resources.Load<Physics2DxSettings>(nameof(Physics2DxSettings));
            _is2Dnot3D = settings.is2Dnot3D;
            waitForConversionTime = new WaitForSecondsRealtime(settings.conversionTime);
            splitConversionOverMultipleFrames = settings.splitConversionOverMultipleFrames;
            slimHierarchy = settings.slimHierarchy;

            var coroutineHandlerGO = new GameObject(nameof(coroutineHandler));
            Object.DontDestroyOnLoad(coroutineHandlerGO);
            if(slimHierarchy)
            {
                coroutineHandlerGO.hideFlags |= HideFlags.HideInHierarchy;
            }

            coroutineHandler = coroutineHandlerGO.AddComponent<Persistent>();
            Persistent.Init();
        }
    }
}
