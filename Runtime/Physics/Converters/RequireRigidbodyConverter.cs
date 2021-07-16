using UnityEngine;

namespace Unity2Dx.Physics
{
    [RequireComponent(typeof(Rigidbody2Dx))]
    public abstract class RequireRigidbodyConverter<R, R2D> : Converter where R : Component where R2D : Component
    {
        #region Required Components
        private Rigidbody2Dx _rigidbody2Dx;
        public Rigidbody2Dx rigidbody2Dx => _rigidbody2Dx ? _rigidbody2Dx : (_rigidbody2Dx = GetComponent<Rigidbody2Dx>());
        #endregion

        #region Properties
        public R componentCopy { get; private set; }
        public R2D component2DCopy { get; private set; }

        public R component { get; private set; }
        public R2D component2D { get; private set; }
        #endregion

        #region Add Methods
        /// <include file='../Documentation.xml' path='docs/RequireRigidbodyConverter/AddComponent/*'/>
        public R AddComponent()
        {
            if (!rigidbody2Dx.rigidbody)
            {
                rigidbody2Dx.AddRigidbody();
            }

            component = gameObject.AddComponent<R>();
            return component;
        }

        /// <include file='../Documentation.xml' path='docs/RequireRigidbodyConverter/AddComponent2D/*'/>
        public R2D AddComponent2D()
        {
            if (!rigidbody2Dx.rigidbody2D)
            {
                rigidbody2Dx.AddRigidbody2D();
            }

            component2D = gameObject.AddComponent<R2D>();
            return component2D;
        }

        /// <include file='../Documentation.xml' path='docs/RequireRigidbodyConverter/AddComponent/*'/>
        public R AddComponent(R copyOf)
        {
            var component = AddComponent();
            ComponentToComponent(copyOf, component);

            return component;
        }

        /// <include file='../Documentation.xml' path='docs/RequireRigidbodyConverter/AddComponent2D/*'/>
        public R2D AddComponent2D(R2D copyOf)
        {
            var component2D = AddComponent2D();
            Component2DToComponent2D(copyOf, component2D);

            return component2D;
        }
        #endregion

        #region Unity Methods
        protected virtual void Awake()
        {
            componentCopy = rigidbody2Dx.rigidbodyCopy.gameObject.AddComponent<R>();
            if (component = GetComponent<R>())
            {
                ComponentToComponent(component, componentCopy);
            }

            component2DCopy = rigidbody2Dx.rigidbody2DCopy.gameObject.AddComponent<R2D>();
            if (component2D = GetComponent<R2D>())
            {
                Component2DToComponent2D(component2D, component2DCopy);
            }
        }

        protected virtual void OnDestroy()
        {
#if UNITY_EDITOR
            if (componentCopy)
            {
                Destroy(componentCopy);
            }
            if (component2DCopy)
            {
                Destroy(component2DCopy);
            }
#else
            Destroy(componentCopy);
            Destroy(component2DCopy);
#endif
        }
        #endregion

        #region Conversion
        /// <include file='../Documentation.xml' path='docs/RequireRigidbodyConverter/ComponentToComponent/*'/>
        protected abstract void ComponentToComponent(R component, R other);
        /// <include file='../Documentation.xml' path='docs/RequireRigidbodyConverter/Component2DToComponent2D/*'/>
        protected abstract void Component2DToComponent2D(R2D component2D, R2D other);

        /// <include file='../Documentation.xml' path='docs/RequireRigidbodyConverter/ComponentToComponent2D/*'/>
        protected abstract void ComponentToComponent2D(R component, R2D component2D);
        /// <include file='../Documentation.xml' path='docs/RequireRigidbodyConverter/Component2DToComponent/*'/>
        protected abstract void Component2DToComponent(R2D component2D, R component);

        public override void ConvertTo2D()
        {
            rigidbody2Dx.onRigidbodyAssigned.RemoveListener(OnRigidbodyAssigned);

            if (component || (component = GetComponent<R>()))
            {
                ComponentToComponent2D(component, component2DCopy);

                DestroyImmediate(component);
                rigidbody2Dx.onRigidbody2DAssigned.AddListener(OnRigidbody2DAssigned);
            }
        }

        private void OnRigidbody2DAssigned(Rigidbody2D rigidbody2D)
        {
            component2D = gameObject.AddComponent<R2D>();
            Component2DToComponent2D(component2DCopy, component2D);

            rigidbody2Dx.onRigidbody2DAssigned.RemoveListener(OnRigidbody2DAssigned);
        }

        public override void ConvertTo3D()
        {
            rigidbody2Dx.onRigidbody2DAssigned.RemoveListener(OnRigidbody2DAssigned);

            if (component2D || (component2D = GetComponent<R2D>()))
            {
                Component2DToComponent(component2D, componentCopy);

                DestroyImmediate(component2D);
                rigidbody2Dx.onRigidbodyAssigned.AddListener(OnRigidbodyAssigned);
            }
        }

        private void OnRigidbodyAssigned(Rigidbody rigidbody)
        {
            component = gameObject.AddComponent<R>();
            ComponentToComponent(componentCopy, component);

            rigidbody2Dx.onRigidbodyAssigned.RemoveListener(OnRigidbodyAssigned);
        }
        #endregion
    }
}
