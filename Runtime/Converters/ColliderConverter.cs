using System;
using UnityEngine;

namespace DimensionConverter
{
    [RequireComponent(typeof(SphereConverter))]
    [RequireComponent(typeof(CapsuleConverter))]
    [RequireComponent(typeof(BoxConverter))]
    [RequireComponent(typeof(MeshConverter))]
    [AddComponentMenu(Settings.componentMenu + "Collider Converter")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [Obsolete("Unfinished!", false)]
    public class ColliderConverter : MonoBehaviour
    {
        #region Required Components
        private SphereConverter _sphereConverter;
        public SphereConverter sphereConverter => _sphereConverter ? _sphereConverter : (_sphereConverter = GetComponent<SphereConverter>());

        private CapsuleConverter _capsuleConverter;
        public CapsuleConverter capsuleConverter => _capsuleConverter ? _capsuleConverter : (_capsuleConverter = GetComponent<CapsuleConverter>());

        private BoxConverter _boxConverter; 
        public BoxConverter boxConverter => _boxConverter ? _boxConverter : (_boxConverter = GetComponent<BoxConverter>());

        private MeshConverter _meshConverter;
        public MeshConverter meshConverter => _meshConverter ? _meshConverter : (_meshConverter = GetComponent<MeshConverter>());
        #endregion

        #region Unity Methods
        private void Awake()
        {
            sphereConverter.hideFlags
                = capsuleConverter.hideFlags
                = boxConverter.hideFlags
                = meshConverter.hideFlags
                = HideFlags.HideInInspector;
        }

        private void OnDestroy()
        {
            sphereConverter.hideFlags
                = capsuleConverter.hideFlags
                = boxConverter.hideFlags
                = meshConverter.hideFlags
                = HideFlags.None;
        }
        #endregion

        public void SetAutoUpdate(bool autoUpdate)
        {
            sphereConverter.autoUpdate 
                = capsuleConverter.autoUpdate
                = boxConverter.autoUpdate
                = meshConverter.autoUpdate 
                = autoUpdate;
        }
    }
}
