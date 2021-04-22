using DimensionConverter;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    public static class Dimension
    {
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
                    enabled = true;
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
        private static bool _is2DNot3D = false;
        /// <include file='./Documentation.xml' path='docs/Dimension/is2DNot3D/*' />
        public static bool is2DNot3D
        {
            get => _is2DNot3D;
            set
            {
                if(isConverting || _is2DNot3D == value)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("Cannot convert Dimension while Dimension is converting.");
#endif
                    return;
                }

                StartCoroutine(UpdateIsConverting(value));
                if(batchConversion)
                {
                    StartCoroutine(ConvertConvertersRoutine(value));
                }
                else
                {
                    ConvertConverters(value);
                }
            }
        }

        /// <include file='./Documentation.xml' path='docs/Dimension/Convert/*' />
        public static void Convert()
        {
            is2DNot3D = !is2DNot3D;
        }

        /// <include file='./Documentation.xml' path='docs/Dimension/onConvert/*' />
        public delegate void OnConvert(bool to2Dnot3D);
        /// <include file='./Documentation.xml' path='docs/Dimension/onConvert/*' />
        public static event OnConvert onBeforeConvert;
        /// <include file='./Documentation.xml' path='docs/Dimension/onConvert/*' />
        public static event OnConvert onAfterConvert;

        /// <include file='./Documentation.xml' path='docs/Dimension/waitForConversionTime/*' />
        public static WaitForSecondsRealtime waitForConversionTime { get; } = new WaitForSecondsRealtime(0f);
        /// <include file='./Documentation.xml' path='docs/Dimension/conversionTime/*' />
        public static float conversionTime
        {
            get => waitForConversionTime.waitTime;
            set => waitForConversionTime.waitTime = value;
        }
        /// <include file='./Documentation.xml' path='docs/Dimension/conversionTimeScale/*' />
        public static float conversionTimeScale { get; set; }

        /// <include file='./Documentation.xml' path='docs/Dimension/isConverting/*' />
        public static bool isConverting { get; private set; }

        /// <include file='./Documentation.xml' path='docs/Dimension/waitWhileConverting/*' />
        public static WaitWhile waitWhileConverting => new WaitWhile(IsConverting);

        private static bool IsConverting() => isConverting;

        private static IEnumerator UpdateIsConverting(bool to2Dnot3D)
        {
            var timeScale = Time.timeScale;

            onBeforeConvert?.Invoke(to2Dnot3D);

            Time.timeScale = conversionTimeScale;
            isConverting = true;
            yield return waitForConversionTime;
            isConverting = false;
            Time.timeScale = timeScale;

            _is2DNot3D = to2Dnot3D;
            onAfterConvert?.Invoke(to2Dnot3D);
        }

        /// <include file='./Documentation.xml' path='docs/Dimension/batchConversion/*' />
        public static bool batchConversion { get; set; }
        #endregion

        #region Converter Conversion
        private static int convertersLength;
        private static Dictionary<Type, int> converterTypeIndex = new Dictionary<Type, int>();
        private static HashSet<Converter>[] orderedConverters = Array.Empty<HashSet<Converter>>();
        private static uint[] orderedBatchSizes3D = Array.Empty<uint>();
        private static uint[] orderedBatchSizes2D = Array.Empty<uint>();

        /// <include file='./Documentation.xml' path='docs/Dimension/AddConverter/*' />
        public static void AddConverter(Converter converter)
        {
            if(!batchConversion || StartCoroutine(AddToConversionList()) == null)
            {
                var index = converterTypeIndex[converter.GetType()];
                orderedConverters[index].Add(converter);
            }

            IEnumerator AddToConversionList()
            {
                yield return waitWhileConverting;

                var index = converterTypeIndex[converter.GetType()];
                orderedConverters[index].Add(converter);
            }
        }

        /// <include file='./Documentation.xml' path='docs/Dimension/RemoveConverter/*' />
        public static void RemoveConverter(Converter converter)
        {
            if(!batchConversion || StartCoroutine(RemoveFromConversionList()) == null)
            {
                var index = converterTypeIndex[converter.GetType()];
                orderedConverters[index].Remove(converter);
            }

            IEnumerator RemoveFromConversionList()
            {
                yield return waitWhileConverting;

                var index = converterTypeIndex[converter.GetType()];
                orderedConverters[index].Remove(converter);
            }
        }

        /// <include file='./Documentation.xml' path='docs/Dimension/ConvertConverters/*' />
        public static IEnumerator ConvertConvertersRoutine(bool to2DNot3D)
        {
            uint currentBatchSize = 0;
            if(to2DNot3D)
            {
                for(int i = 0; i < convertersLength; i++)
                {
                    var converters = orderedConverters[i];

                    if(isConverting)
                    {
                        var newBatchSize = orderedBatchSizes2D[i];
                        if(currentBatchSize >= newBatchSize)
                        {
                            yield return null;
                            currentBatchSize = 0;
                        }

                        foreach(var converter in converters)
                        {
                            converter.ConvertTo2D();
                            currentBatchSize++;
                            if(currentBatchSize == newBatchSize)
                            {
                                if(isConverting)
                                {
                                    yield return null;
                                    currentBatchSize = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach(var converter in converters)
                        {
                            converter.ConvertTo2D();
                        }
                    }
                }
            }
            else
            {
                for(int i = 0; i < convertersLength; i++)
                {
                    var converters = orderedConverters[i];

                    if(isConverting)
                    {
                        var newBatchSize = orderedBatchSizes3D[i];
                        if(currentBatchSize >= newBatchSize)
                        {
                            yield return null;
                            currentBatchSize = 0;
                        }

                        foreach(var converter in converters)
                        {
                            converter.ConvertTo3D();
                            currentBatchSize++;
                            if(currentBatchSize == newBatchSize)
                            {
                                if(isConverting)
                                {
                                    yield return null;
                                    currentBatchSize = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach(var converter in converters)
                        {
                            converter.ConvertTo3D();
                        }
                    }
                }
            }
        }

        /// <include file='./Documentation.xml' path='docs/Dimension/ConvertConverters/*' />
        public static void ConvertConverters(bool to2DNot3D)
        {
            if(to2DNot3D)
            {
                foreach(var converters in orderedConverters)
                {
                    foreach(var converter in converters)
                    {
                        converter.ConvertTo2D();
                    }
                }
            }
            else
            {
                foreach(var converters in orderedConverters)
                {
                    foreach(var converter in converters)
                    {
                        converter.ConvertTo3D();
                    }
                }
            }
        }
        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration()
        {
            onBeforeConvert = onAfterConvert = default;

            var settings = Settings.GetSettings;
            _is2DNot3D = settings.is2DNot3D;
            isConverting = false;
            conversionTime = settings.conversionTime;
            conversionTimeScale = settings.conversionTimeScale;
            batchConversion = settings.batchConversion;

            convertersLength = settings.convertersSettings.Length;
            converterTypeIndex = new Dictionary<Type, int>();
            orderedConverters = new HashSet<Converter>[convertersLength];
            orderedBatchSizes3D = new uint[convertersLength];
            orderedBatchSizes2D = new uint[convertersLength];
            for(int i = 0; i < convertersLength; i++)
            {
                var converterSettings = settings.convertersSettings[i];
                converterTypeIndex.Add(converterSettings.type, i);
                orderedConverters[i] = new HashSet<Converter>();
                orderedBatchSizes3D[i] = converterSettings.batchSize3D;
                orderedBatchSizes2D[i] = converterSettings.batchSize2D;
            }

            var coroutineHandlerGO = new GameObject(nameof(coroutineHandler));
            Object.DontDestroyOnLoad(coroutineHandlerGO);

            coroutineHandler = coroutineHandlerGO.AddComponent<Persistent>();
            Persistent.Init();
        }
    }
}
