using UnityEngine;
using DimensionConverter.Utilities;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Constant Force Converter")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RigidbodyConverter))]
    [RequireComponent(typeof(TransformSplitter))]
    public class ConstantForceConverter : Converter
    {
        #region Required Components
        private RigidbodyConverter _rigidbodyConverter;
        public RigidbodyConverter rigidbodyConverter => _rigidbodyConverter ? _rigidbodyConverter : (_rigidbodyConverter = GetComponent<RigidbodyConverter>());

        private TransformSplitter _transformSplitter;
        public TransformSplitter transformSplitter => _transformSplitter ? _transformSplitter : (_transformSplitter = GetComponent<TransformSplitter>());
        #endregion

        #region Properties
        public ConstantForce constantForceCopy { get; private set; }
        public ConstantForce2D constantForce2DCopy { get; private set; }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public ConstantForce constantForce { get; private set; }
        public ConstantForce2D constantForce2D { get; private set; }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        #endregion

        #region Add Methods
        /// <include file='../Documentation.xml' path='docs/ConstantForceConverter/AddConstantForce/*' />
        public ConstantForce AddConstantForce()
        {
            if(!rigidbodyConverter.rigidbody)
            {
                rigidbodyConverter.AddRigidbody();
            }

            constantForce = gameObject.AddComponent<ConstantForce>();
            return constantForce;
        }

        /// <include file='../Documentation.xml' path='docs/ConstantForceConverter/AddConstantForce2D/*' />
        public ConstantForce2D AddConstantForce2D()
        {
            if(!rigidbodyConverter.rigidbody2D)
            {
                rigidbodyConverter.AddRigidbody2D();
            }

            constantForce2D = gameObject.AddComponent<ConstantForce2D>();
            return constantForce2D;
        }

        /// <include file='../Documentation.xml' path='docs/ConstantForceConverter/AddConstantForce/*' />
        public ConstantForce AddConstantForce(ConstantForce copyOf)
        {
            AddConstantForce();
            copyOf.ToConstantForce(constantForce);

            return constantForce;
        }

        /// <include file='../Documentation.xml' path='docs/ConstantForceConverter/AddConstantForce2D/*' />
        public ConstantForce2D AddConstantForce2D(ConstantForce2D copyOf)
        {
            AddConstantForce2D();
            copyOf.ToConstantForce2D(constantForce2D);

            return constantForce2D;
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            constantForceCopy = rigidbodyConverter.rigidbodyCopy.gameObject.AddComponent<ConstantForce>();
            if(constantForce = GetComponent<ConstantForce>())
            {
                constantForce.ToConstantForce(constantForceCopy);
            }

            constantForce2DCopy = rigidbodyConverter.rigidbody2DCopy.gameObject.AddComponent<ConstantForce2D>();
            if(constantForce2D = GetComponent<ConstantForce2D>())
            {
                constantForce2D.ToConstantForce2D(constantForce2DCopy);
            }
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (constantForceCopy)
            {
                Destroy(constantForceCopy);
            }
            if (constantForce2DCopy)
            {
                Destroy(constantForce2DCopy);
            }
#else
            Destroy(constantForceCopy);
            Destroy(constantForce2DCopy);
#endif
        }
        #endregion

        #region Conversion
        public override void ConvertTo2D()
        {
            if(constantForce || (constantForce = GetComponent<ConstantForce>()))
            {
                constantForce2DCopy.transform.rotation = transformSplitter.transform2D.rotation;
                constantForce.ToConstantForce2D(constantForce2DCopy);
                
                DestroyImmediate(constantForce);
                rigidbodyConverter.onRigidbody2DAssigned.AddListener(OnRigidbody2DAssigned);
            }
        }

        private void OnRigidbody2DAssigned(Rigidbody2D rigidbody2D)
        {
            constantForce2D = gameObject.AddComponent<ConstantForce2D>();
            constantForce2DCopy.ToConstantForce2D(constantForce2D);

            rigidbodyConverter.onRigidbody2DAssigned.RemoveListener(OnRigidbody2DAssigned);
        }

        public override void ConvertTo3D()
        {
            if(constantForce2D || (constantForce2D = GetComponent<ConstantForce2D>()))
            {
                constantForceCopy.transform.rotation = transformSplitter.transform3D.rotation;

                var constantForce2DRotation = constantForce2D.transform.rotation;
                constantForce2D.transform.rotation = transformSplitter.transform2D.rotation;
                constantForce2D.ToConstantForce(constantForceCopy);
                constantForce2D.transform.rotation = constantForce2DRotation;
                
                DestroyImmediate(constantForce2D);
                rigidbodyConverter.onRigidbodyAssigned.AddListener(OnRigidbodyAssigned);
            }
        }

        private void OnRigidbodyAssigned(Rigidbody rigidbody)
        {
            constantForce = gameObject.AddComponent<ConstantForce>();
            constantForceCopy.ToConstantForce(constantForce);

            rigidbodyConverter.onRigidbodyAssigned.RemoveListener(OnRigidbodyAssigned);
        }
        #endregion
    }
}
