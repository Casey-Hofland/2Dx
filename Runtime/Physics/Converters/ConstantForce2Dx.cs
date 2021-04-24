using UnityEngine;

namespace Unity2Dx.Physics
{
    [AddComponentMenu("2Dx/Constant Force 2Dx")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2Dx))]
    [RequireComponent(typeof(Transform2Dx))]
    public sealed class ConstantForce2Dx : Converter
    {
        #region Required Components
        private Rigidbody2Dx _rigidbody2Dx;
        public Rigidbody2Dx rigidbody2Dx => _rigidbody2Dx ? _rigidbody2Dx : (_rigidbody2Dx = GetComponent<Rigidbody2Dx>());

        private Transform2Dx _transform2Dx;
        public Transform2Dx transform2Dx => _transform2Dx ? _transform2Dx : (_transform2Dx = GetComponent<Transform2Dx>());
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
        /// <include file='../Documentation.xml' path='docs/ConstantForce2Dx/AddConstantForce/*' />
        public ConstantForce AddConstantForce()
        {
            if(!rigidbody2Dx.rigidbody)
            {
                rigidbody2Dx.AddRigidbody();
            }

            constantForce = gameObject.AddComponent<ConstantForce>();
            return constantForce;
        }

        /// <include file='../Documentation.xml' path='docs/ConstantForce2Dx/AddConstantForce2D/*' />
        public ConstantForce2D AddConstantForce2D()
        {
            if(!rigidbody2Dx.rigidbody2D)
            {
                rigidbody2Dx.AddRigidbody2D();
            }

            constantForce2D = gameObject.AddComponent<ConstantForce2D>();
            return constantForce2D;
        }

        /// <include file='../Documentation.xml' path='docs/ConstantForce2Dx/AddConstantForce/*' />
        public ConstantForce AddConstantForce(ConstantForce copyOf)
        {
            AddConstantForce();
            copyOf.ToConstantForce(constantForce);

            return constantForce;
        }

        /// <include file='../Documentation.xml' path='docs/ConstantForce2Dx/AddConstantForce2D/*' />
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
            constantForceCopy = rigidbody2Dx.rigidbodyCopy.gameObject.AddComponent<ConstantForce>();
            if(constantForce = GetComponent<ConstantForce>())
            {
                constantForce.ToConstantForce(constantForceCopy);
            }

            constantForce2DCopy = rigidbody2Dx.rigidbody2DCopy.gameObject.AddComponent<ConstantForce2D>();
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
                constantForce2DCopy.transform.rotation = transform2Dx.transform2D.rotation;
                constantForce.ToConstantForce2D(constantForce2DCopy);
                
                DestroyImmediate(constantForce);
                rigidbody2Dx.onRigidbody2DAssigned.AddListener(OnRigidbody2DAssigned);
            }
        }

        private void OnRigidbody2DAssigned(Rigidbody2D rigidbody2D)
        {
            constantForce2D = gameObject.AddComponent<ConstantForce2D>();
            constantForce2DCopy.ToConstantForce2D(constantForce2D);

            rigidbody2Dx.onRigidbody2DAssigned.RemoveListener(OnRigidbody2DAssigned);
        }

        public override void ConvertTo3D()
        {
            if(constantForce2D || (constantForce2D = GetComponent<ConstantForce2D>()))
            {
                constantForceCopy.transform.rotation = transform2Dx.transform3D.rotation;

                var constantForce2DRotation = constantForce2D.transform.rotation;
                constantForce2D.transform.rotation = transform2Dx.transform2D.rotation;
                constantForce2D.ToConstantForce(constantForceCopy);
                constantForce2D.transform.rotation = constantForce2DRotation;
                
                DestroyImmediate(constantForce2D);
                rigidbody2Dx.onRigidbodyAssigned.AddListener(OnRigidbodyAssigned);
            }
        }

        private void OnRigidbodyAssigned(Rigidbody rigidbody)
        {
            constantForce = gameObject.AddComponent<ConstantForce>();
            constantForceCopy.ToConstantForce(constantForce);

            rigidbody2Dx.onRigidbodyAssigned.RemoveListener(OnRigidbodyAssigned);
        }
        #endregion
    }
}
