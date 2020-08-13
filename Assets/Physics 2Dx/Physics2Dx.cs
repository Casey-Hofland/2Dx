using Physics2DxSystem;
using System;
using System.Collections;
using System.Collections.Generic;

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
#if UNITY_EDITOR
                    Debug.LogWarning("Cannot convert Physics2Dx while Physics2Dx is converting.");
#endif
                    return;
                }

                StartCoroutine(UpdateIsConverting(value));
                if(splitConversionOverMultipleFrames)
                {
                    StartCoroutine(ConvertModule2DxesRoutine(value));
                }
                else
                {
                    ConvertModule2Dxes(value);
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
        public static WaitForSecondsRealtime waitForConversionTime { get; } = new WaitForSecondsRealtime(0f);
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

        #region Module2Dx Instances Conversion
        internal static void AddModule2DxInstance(Module2Dx module2Dx)
        {
            if(!splitConversionOverMultipleFrames || StartCoroutine(AddToConversionList()) == null)
            {
                var order = module2DxTypeOrder[module2Dx.GetType()];
                orderedModule2Dxes[order].Add(module2Dx);
            }

            IEnumerator AddToConversionList()
            {
                yield return waitWhileConverting;

                var order = module2DxTypeOrder[module2Dx.GetType()];
                orderedModule2Dxes[order].Add(module2Dx);
            }
        }

        internal static void RemoveModule2DxInstance(Module2Dx module2Dx)
        {
            if(!splitConversionOverMultipleFrames || StartCoroutine(RemoveFromConversionList()) == null)
            {
                var order = module2DxTypeOrder[module2Dx.GetType()];
                orderedModule2Dxes[order].Remove(module2Dx);
            }

            IEnumerator RemoveFromConversionList()
            {
                yield return waitWhileConverting;

                var order = module2DxTypeOrder[module2Dx.GetType()];
                orderedModule2Dxes[order].Remove(module2Dx);
            }
        }

        private static Dictionary<Type, int> module2DxTypeOrder = new Dictionary<Type, int>();
        private static SortedList<int, HashSet<Module2Dx>> orderedModule2Dxes = new SortedList<int, HashSet<Module2Dx>>();
        private static Dictionary<int, uint> orderedBatchSizes3D = new Dictionary<int, uint>();
        private static Dictionary<int, uint> orderedBatchSizes2D = new Dictionary<int, uint>();

        private static IEnumerator ConvertModule2DxesRoutine(bool to2Dnot3D)
        {
            uint currentBatchSize = 0;
            if(to2Dnot3D)
            {
                foreach(var orderModule2Dxes in orderedModule2Dxes)
                {
                    if(isConverting)
                    {
                        var newBatchSize = orderedBatchSizes2D[orderModule2Dxes.Key];
                        if(currentBatchSize >= newBatchSize)
                        {
                            yield return null;
                            currentBatchSize = 0;
                        }

                        foreach(var module2Dx in orderModule2Dxes.Value)
                        {
                            module2Dx.ConvertTo2D();
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
                        foreach(var module2Dx in orderModule2Dxes.Value)
                        {
                            module2Dx.ConvertTo2D();
                        }
                    }
                }
            }
            else
            {
                foreach(var orderModule2Dxes in orderedModule2Dxes)
                {
                    if(isConverting)
                    {
                        var newBatchSize = orderedBatchSizes3D[orderModule2Dxes.Key];
                        if(currentBatchSize >= newBatchSize)
                        {
                            yield return null;
                            currentBatchSize = 0;
                        }

                        foreach(var module2Dx in orderModule2Dxes.Value)
                        {
                            module2Dx.ConvertTo3D();
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
                        foreach(var module2Dx in orderModule2Dxes.Value)
                        {
                            module2Dx.ConvertTo3D();
                        }
                    }
                }
            }
        }

        private static void ConvertModule2Dxes(bool to2Dnot3D)
        {
            if(to2Dnot3D)
            {
                foreach(var module2Dxes in orderedModule2Dxes.Values)
                {
                    foreach(var module2Dx in module2Dxes)
                    {
                        module2Dx.ConvertTo2D();
                    }
                }
            }
            else
            {
                foreach(var module2Dxes in orderedModule2Dxes.Values)
                {
                    foreach(var module2Dx in module2Dxes)
                    {
                        module2Dx.ConvertTo3D();
                    }
                }
            }
        }
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
            isConverting = false;
            conversionTime = settings.conversionTime;
            splitConversionOverMultipleFrames = settings.splitConversion;
            slimHierarchy = settings.slimHierarchy;

            module2DxTypeOrder = new Dictionary<Type, int>();
            orderedModule2Dxes = new SortedList<int, HashSet<Module2Dx>>();
            orderedBatchSizes3D = new Dictionary<int, uint>();
            orderedBatchSizes2D = new Dictionary<int, uint>();
            foreach(var module2DxSettings in settings.module2DxesSettings)
            {
                module2DxTypeOrder.Add(module2DxSettings.type, module2DxSettings.order);
                orderedModule2Dxes.Add(module2DxSettings.order, new HashSet<Module2Dx>());
                orderedBatchSizes3D.Add(module2DxSettings.order, module2DxSettings.batchSize3D);
                orderedBatchSizes2D.Add(module2DxSettings.order, module2DxSettings.batchSize2D);
            }

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
