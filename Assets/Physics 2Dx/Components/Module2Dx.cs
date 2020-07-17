using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics2DxSystem
{
    public abstract class Module2Dx : MonoBehaviour
    {
        #region Static Collection
        private static SortedDictionary<int, SortedDictionary<uint, HashSet<Module2Dx>>> orderedBatches = new SortedDictionary<int, SortedDictionary<uint, HashSet<Module2Dx>>>();

        /// <include file='../Documentation.xml' path='docs/Modules/Module2Dx/ConvertInstances/*'/>
        internal static IEnumerator ConvertInstancesAsCoroutine(bool to2Dnot3D)
        {
            uint currentBatchSize;
            foreach(var batches in orderedBatches.Values)
            {
                foreach(var batch in batches)
                {
                    currentBatchSize = batch.Key;
                    foreach(var instance in batch.Value)
                    {
                        if(currentBatchSize == 0)
                        {
                            yield return null;
                            currentBatchSize = batch.Key;
                        }

                        instance.Convert(to2Dnot3D);
                        if(Physics2Dx.isConverting)
                        {
                            currentBatchSize--;
                        }
                    }
                    if(Physics2Dx.isConverting)
                    {
                        yield return null;
                    }
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/Modules/Module2Dx/ConvertInstances/*'/>
        internal static void ConvertInstances(bool to2Dnot3D)
        {
            foreach(var batches in orderedBatches.Values)
            {
                foreach(var instances in batches.Values)
                {
                    foreach(var instance in instances)
                    {
                        instance.Convert(to2Dnot3D);
                    }
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            orderedBatches = new SortedDictionary<int, SortedDictionary<uint, HashSet<Module2Dx>>>();
        }
        #endregion

        /// <include file='../Documentation.xml' path='docs/Modules/Module2Dx/conversionOrder/*'/>
        public abstract int conversionOrder { get; }
        /// <include file='../Documentation.xml' path='docs/Modules/Module2Dx/batchSize/*'/>
        public abstract uint batchSize { get; }

        protected virtual void Awake()
        {
            var clampedBatchSize = Math.Max(1, batchSize);

            if(!orderedBatches.ContainsKey(conversionOrder))
            {
                orderedBatches.Add(conversionOrder, new SortedDictionary<uint, HashSet<Module2Dx>>());
            }
            if(!orderedBatches[conversionOrder].ContainsKey(clampedBatchSize))
            {
                orderedBatches[conversionOrder].Add(clampedBatchSize, new HashSet<Module2Dx>());
            }
        }

        protected virtual void OnEnable()
        {
            if(!Physics2Dx.splitConversionOverMultipleFrames || Physics2Dx.StartCoroutine(AddToConversionList()) == null)
            {
                orderedBatches[conversionOrder][batchSize].Add(this);
            }

            Convert(Physics2Dx.is2Dnot3D);

            IEnumerator AddToConversionList()
            {
                yield return Physics2Dx.waitWhileConverting;
                orderedBatches[conversionOrder][batchSize].Add(this);
            }
        }

        protected virtual void OnDisable()
        {
            if(!Physics2Dx.splitConversionOverMultipleFrames || Physics2Dx.StartCoroutine(RemoveFromConversionList()) == null)
            {
                orderedBatches[conversionOrder][batchSize].Remove(this);
            }

            IEnumerator RemoveFromConversionList()
            {
                yield return Physics2Dx.waitWhileConverting;
                orderedBatches[conversionOrder][batchSize].Remove(this);
            }
        }

        /// <include file='../Documentation.xml' path='docs/Modules/Module2Dx/Convert/*'/>
        public virtual void Convert(bool to2Dnot3D)
        {
            if(to2Dnot3D)
            {
                ConvertTo2D();
            }
            else
            {
                ConvertTo3D();
            }
        }

        /// <include file='../Documentation.xml' path='docs/Modules/Module2Dx/ConvertTo2D/*'/>
        public abstract void ConvertTo2D();
        /// <include file='../Documentation.xml' path='docs/Modules/Module2Dx/ConvertTo3D/*'/>
        public abstract void ConvertTo3D();
    }
}
