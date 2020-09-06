using DimensionConverter.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Rigidbody Converter")]
    [DisallowMultipleComponent]
    public sealed class RigidbodyConverter : Converter
    {
        #region Copies Holder
        private static GameObject rigidbodyCopies;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            rigidbodyCopies = new GameObject(nameof(rigidbodyCopies));
            DontDestroyOnLoad(rigidbodyCopies);
            if(Dimension.slimHierarchy)
            {
                rigidbodyCopies.hideFlags |= HideFlags.HideInHierarchy;
            }
        }
        #endregion

        #region Properties
        [Tooltip("When converting to 2D, ignore collisions of overlapping Collider2Ds until they aren't overlapping anymore.")] public bool ignoreOverlap = true;

        private Rigidbody rigidbodyCopy;
        private Rigidbody2D rigidbody2DCopy;

        public new Rigidbody rigidbody { get; private set; }
        public new Rigidbody2D rigidbody2D { get; private set; }

        private List<Collider2D> attachedColliders = new List<Collider2D>();
        private List<Collider2D> overlapColliders = new List<Collider2D>();
        private List<Collider2D> trackedColliders = new List<Collider2D>();

        public ReadOnlyCollection<Collider2D> ignoredColliders => trackedColliders.AsReadOnly();
        #endregion

        #region Add Methods
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddRigidbody/*' />
        public Rigidbody AddRigidbody()
        {
            return rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddRigidbody2D/*' />
        public Rigidbody2D AddRigidbody2D()
        {
            return rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddRigidbody/*' />
        public Rigidbody AddRigidbody(Rigidbody copyOf)
        {
            var rigidbody = AddRigidbody();
            copyOf.ToRigidbody(rigidbody);
            return rigidbody;
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddRigidbody2D/*' />
        public Rigidbody2D AddRigidbody2D(Rigidbody2D copyOf)
        {
            var rigidbody2D = AddRigidbody2D();
            copyOf.ToRigidbody2D(rigidbody2D);
            return rigidbody2D;
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            rigidbodyCopy = new GameObject($"{name} {nameof(rigidbodyCopy)}").AddComponent<Rigidbody>();
            rigidbodyCopy.gameObject.SetActive(false);
            rigidbodyCopy.transform.SetParent(rigidbodyCopies.transform, false);

            rigidbody2DCopy = new GameObject($"{name} {nameof(rigidbody2DCopy)}").AddComponent<Rigidbody2D>();
            rigidbody2DCopy.gameObject.SetActive(false);
            rigidbody2DCopy.transform.SetParent(rigidbodyCopies.transform, false);
        }

#if UNITY_2020_1_OR_NEWER
        private void FixedUpdate()
        {
            if(Physics2D.simulationMode == Physics2D.FixedUpdate)
            {
                UpdateOverlap();
            }
        }

        private void Update()
        {
            if(Physics2D.simulationMode == Physics2D.Update)
            {
                UpdateOverlap();
            }
        }
#else
        private void FixedUpdate()
        {
            UpdateOverlap();
        }
#endif

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if(rigidbodyCopy)
            {
                Destroy(rigidbodyCopy.gameObject);
            }
            if(rigidbody2DCopy)
            {
                Destroy(rigidbody2DCopy.gameObject);
            }
#else
            Destroy(rigidbodyCopy.gameObject);
            Destroy(rigidbody2DCopy.gameObject);
#endif
        }
#endregion

        #region Conversion
        public override void ConvertTo2D()
        {
            if(rigidbody || (rigidbody = GetComponent<Rigidbody>()))
            {
                rigidbody2DCopy.gameObject.SetActive(true);
                rigidbody.ToRigidbody2D(rigidbody2DCopy);

                DestroyImmediate(rigidbody);
                rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
                rigidbody2DCopy.ToRigidbody2D(rigidbody2D);

                rigidbody2DCopy.gameObject.SetActive(false);

                if(ignoreOverlap)
                {
                    IgnoreOverlap();
                }
            }
        }

        public override void ConvertTo3D()
        {
            if(rigidbody2D || (rigidbody2D = GetComponent<Rigidbody2D>()))
            {
                rigidbodyCopy.gameObject.SetActive(true);
                rigidbody2D.ToRigidbody(rigidbodyCopy);

                DestroyImmediate(rigidbody2D);
                rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbodyCopy.ToRigidbody(rigidbody);

                rigidbodyCopy.gameObject.SetActive(false);

                ClearOverlap();
            }
        }
        #endregion

        #region Overlap
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/IgnoreOverlap/*' />
        public void IgnoreOverlap()
        {
            rigidbody2D.GetAttachedColliders(attachedColliders);
            rigidbody2D.OverlapCollider(default, trackedColliders);

            foreach(var attachedCollider in attachedColliders)
            {
                foreach(var trackedCollider in trackedColliders)
                {
                    Physics2D.IgnoreCollision(attachedCollider, trackedCollider, true);
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/ClearOverlap/*' />
        public void ClearOverlap()
        {
            foreach(var attachedCollider in attachedColliders)
            {
                foreach(var trackedCollider in trackedColliders)
                {
                    Physics2D.IgnoreCollision(attachedCollider, trackedCollider, false);
                }
            }

            attachedColliders.Clear();
            overlapColliders.Clear();
            trackedColliders.Clear();
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/UpdateOverlap/*' />
        public void UpdateOverlap()
        {
            var trackedCollidersCount = trackedColliders.Count;
            if(trackedCollidersCount > 0)
            {
                Collider2D trackedCollider;
                int attachedCollidersCount = 0;
                for(int i = trackedCollidersCount - 1; i >= 0; i--)
                {
                    trackedCollider = trackedColliders[i];
                    if(!trackedCollider)
                    {
                        trackedColliders.RemoveAt(i);
                        continue;
                    }
                    
                    if(attachedCollidersCount == 0)
                    {
                        attachedColliders.RemoveAll(collider => !collider);
                        if((attachedCollidersCount = attachedColliders.Count) == 0)
                        {
                            ClearOverlap();
                            break;
                        }
                        rigidbody2D.OverlapCollider(default, overlapColliders);
                    }
                    
                    if(!overlapColliders.Contains(trackedCollider))
                    {
                        for(int a = attachedCollidersCount - 1; a >= 0; a--)
                        {
                            Physics2D.IgnoreCollision(attachedColliders[a], trackedCollider, false);
                            trackedColliders.RemoveAt(i);
                        }
                    }
                }
            }
        }
        #endregion

        #region Shared Properties
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/angularDrag/*' />
        public float angularDrag
        {
            get => Dimension.is2DNot3D ? rigidbody2D.angularDrag : rigidbody.angularDrag;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.angularDrag = value;
                }
                else
                {
                    rigidbody.angularDrag = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/angularVelocity/*' />
        public Vector3 angularVelocity
        {
            get => Dimension.is2DNot3D ? rigidbody2D.angularVelocity * Vector3.forward : rigidbody.angularVelocity;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.angularVelocity = value.z;
                }
                else
                {
                    rigidbody.angularVelocity = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/centerOfMass/*' />
        public Vector3 centerOfMass
        {
            get => Dimension.is2DNot3D ? (Vector3)rigidbody2D.centerOfMass : rigidbody.centerOfMass;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.centerOfMass = value;
                }
                else
                {
                    rigidbody.centerOfMass = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/constraints/*' />
        public RigidbodyConstraints constraints
        {
            get
            {
                if(Dimension.is2DNot3D)
                {
                    var constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    return rigidbody2D.constraints.ToRigidbodyConstraints(constraints);
                }
                else
                {
                    return rigidbody.constraints;
                }
            }
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.constraints = value.ToRigidbodyConstraints2D();
                }
                else
                {
                    rigidbody.constraints = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/drag/*' />
        public float drag
        {
            get => Dimension.is2DNot3D ? rigidbody2D.drag : rigidbody.drag;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.drag = value;
                }
                else
                {
                    rigidbody.drag = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/freezeRotation/*' />
        public bool freezeRotation
        {
            get => Dimension.is2DNot3D ? rigidbody2D.freezeRotation : rigidbody.freezeRotation;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.freezeRotation = value;
                }
                else
                {
                    rigidbody.freezeRotation = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/interpolation/*' />
        public RigidbodyInterpolation interpolation
        {
            get => Dimension.is2DNot3D ? (RigidbodyInterpolation)rigidbody2D.interpolation : rigidbody.interpolation;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.interpolation = (RigidbodyInterpolation2D)value;
                }
                else
                {
                    rigidbody.interpolation = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/isKinematic/*' />
        public bool isKinematic
        {
            get => Dimension.is2DNot3D ? rigidbody2D.isKinematic : rigidbody.isKinematic;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.isKinematic = value;
                }
                else
                {
                    rigidbody.isKinematic = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/mass/*' />
        public float mass
        {
            get => Dimension.is2DNot3D ? rigidbody2D.mass : rigidbody.mass;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.mass = value;
                }
                else
                {
                    rigidbody.mass = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/position/*' />
        public Vector3 position
        {
            get => Dimension.is2DNot3D ? (Vector3)rigidbody2D.position : rigidbody.position;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.position = value;
                }
                else
                {
                    rigidbody.position = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/rotation/*' />
        public Quaternion rotation
        {
            get => Dimension.is2DNot3D ? Quaternion.AngleAxis(rigidbody2D.rotation, Vector3.forward) : rigidbody.rotation;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.rotation = value.eulerAngles.z;
                }
                else
                {
                    rigidbody.rotation = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/velocity/*' />
        public Vector3 velocity
        {
            get => Dimension.is2DNot3D ? (Vector3)rigidbody2D.velocity : rigidbody.velocity;
            set
            {
                if(Dimension.is2DNot3D)
                {
                    rigidbody2D.velocity = value;
                }
                else
                {
                    rigidbody.velocity = value;
                }
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/worldCenterOfMass/*' />
        public Vector3 worldCenterOfMass => Dimension.is2DNot3D ? (Vector3)rigidbody2D.worldCenterOfMass : rigidbody.worldCenterOfMass;
        #endregion

        #region Shared Methods
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddForce/*' />
        public void AddForce(Vector3 force) => AddForce(force, ForceMode.Force);
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddForce/*' />
        public void AddForce(Vector3 force, ForceMode mode)
        {
            if(Dimension.is2DNot3D)
            {
                switch(mode)
                {
                    case ForceMode.Force:
                        rigidbody2D.AddForce(force, ForceMode2D.Force);
                        break;
                    case ForceMode.Impulse:
                        rigidbody2D.AddForce(force, ForceMode2D.Impulse);
                        break;
                    case ForceMode.VelocityChange:
                        rigidbody2D.AddForce(force * rigidbody2D.mass, ForceMode2D.Impulse);
                        break;
                    case ForceMode.Acceleration:
                        rigidbody2D.AddForce(force * rigidbody2D.mass, ForceMode2D.Force);
                        break;
                }
            }
            else
            {
                rigidbody.AddForce(force, mode);
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddForceAtPosition/*' />
        public void AddForceAtPosition(Vector3 force, Vector3 position) => AddForceAtPosition(force, position, ForceMode.Force);
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddForceAtPosition/*' />
        public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode)
        {
            if(Dimension.is2DNot3D)
            {
                switch(mode)
                {
                    case ForceMode.Force:
                        rigidbody2D.AddForceAtPosition(force, position, ForceMode2D.Force);
                        break;
                    case ForceMode.Impulse:
                        rigidbody2D.AddForceAtPosition(force, position, ForceMode2D.Impulse);
                        break;
                    case ForceMode.VelocityChange:
                        rigidbody2D.AddForceAtPosition(force * rigidbody2D.mass, position, ForceMode2D.Impulse);
                        break;
                    case ForceMode.Acceleration:
                        rigidbody2D.AddForceAtPosition(force * rigidbody2D.mass, position, ForceMode2D.Force);
                        break;
                }
            }
            else
            {
                rigidbody.AddForceAtPosition(force, position, mode);
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddRelativeForce/*' />
        public void AddRelativeForce(Vector3 force) => AddRelativeForce(force, ForceMode.Force);
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddRelativeForce/*' />
        public void AddRelativeForce(Vector3 force, ForceMode mode)
        {
            if(Dimension.is2DNot3D)
            {
                switch(mode)
                {
                    case ForceMode.Force:
                        rigidbody2D.AddRelativeForce(force, ForceMode2D.Force);
                        break;
                    case ForceMode.Impulse:
                        rigidbody2D.AddRelativeForce(force, ForceMode2D.Impulse);
                        break;
                    case ForceMode.VelocityChange:
                        rigidbody2D.AddRelativeForce(force * rigidbody2D.mass, ForceMode2D.Impulse);
                        break;
                    case ForceMode.Acceleration:
                        rigidbody2D.AddRelativeForce(force * rigidbody2D.mass, ForceMode2D.Force);
                        break;
                }
            }
            else
            {
                rigidbody.AddRelativeForce(force, mode);
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddTorque/*' />
        public void AddTorque(float torque) => AddTorque(Vector3.forward * torque);
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddTorque/*' />
        public void AddTorque(Vector3 torque) => AddTorque(torque, ForceMode.Force);
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/AddTorque/*' />
        public void AddTorque(Vector3 torque, ForceMode mode)
        {
            if(Dimension.is2DNot3D)
            {
                switch(mode)
                {
                    case ForceMode.Force:
                        rigidbody2D.AddTorque(torque.z, ForceMode2D.Force);
                        break;
                    case ForceMode.Impulse:
                        rigidbody2D.AddTorque(torque.z, ForceMode2D.Impulse);
                        break;
                    case ForceMode.VelocityChange:
                        rigidbody2D.AddTorque(torque.z * rigidbody2D.mass, ForceMode2D.Impulse);
                        break;
                    case ForceMode.Acceleration:
                        rigidbody2D.AddTorque(torque.z * rigidbody2D.mass, ForceMode2D.Force);
                        break;
                }
            }
            else
            {
                rigidbody.AddTorque(torque, mode);
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/GetPointVelocity/*' />
        public Vector3 GetPointVelocity(Vector3 worldPoint) => Dimension.is2DNot3D ? (Vector3)rigidbody2D.GetPointVelocity(worldPoint) : rigidbody.GetPointVelocity(worldPoint);
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/GetRelativePointVelocity/*' />
        public Vector3 GetRelativePointVelocity(Vector3 relativePoint) => Dimension.is2DNot3D ? (Vector3)rigidbody2D.GetRelativePointVelocity(relativePoint) : rigidbody.GetRelativePointVelocity(relativePoint);
        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/IsSleeping/*' />
        public bool IsSleeping() => Dimension.is2DNot3D ? rigidbody2D.IsSleeping() : rigidbody.IsSleeping();

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/MovePosition/*' />
        public void MovePosition(Vector3 position)
        {
            if(Dimension.is2DNot3D)
            {
                rigidbody2D.MovePosition(position);
            }
            else
            {
                rigidbody.MovePosition(position);
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/MoveRotation/*' />
        public void MoveRotation(Quaternion rotation)
        {
            if(Dimension.is2DNot3D)
            {
                rigidbody2D.MoveRotation(rotation);
            }
            else
            {
                rigidbody.MoveRotation(rotation);
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/Sleep/*' />
        public void Sleep()
        {
            if(Dimension.is2DNot3D)
            {
                rigidbody2D.Sleep();
            }
            else
            {
                rigidbody.Sleep();
            }
        }

        /// <include file='../Documentation.xml' path='docs/RigidbodyConverter/WakeUp/*' />
        public void WakeUp()
        {
            if(Dimension.is2DNot3D)
            {
                rigidbody2D.WakeUp();
            }
            else
            {
                rigidbody.WakeUp();
            }
        }
        #endregion
    }
}
