using Physics2DxSystem.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Physics2DxSystem
{
    [AddComponentMenu(Physics2Dx.componentMenu + "Rigidbody Converter")]
    [DisallowMultipleComponent]
    public sealed class RigidbodyConverter : Module2Dx
    {
        #region Copies Holder
        private static GameObject rigidbodyCopies;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            rigidbodyCopies = new GameObject(nameof(rigidbodyCopies));
            DontDestroyOnLoad(rigidbodyCopies);
            if(Physics2Dx.slimHierarchy)
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
        public HashSet<Collider2D> ignoredColliders = new HashSet<Collider2D>();
        #endregion

        #region Add Methods
        public Rigidbody AddRigidbody()
        {
            return rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        public Rigidbody2D AddRigidbody2D()
        {
            return rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        }

        public Rigidbody AddRigidbody(Rigidbody copyOf)
        {
            var rigidbody = AddRigidbody();
            copyOf.ToRigidbody(rigidbody);
            return rigidbody;
        }

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
        public void IgnoreOverlap()
        {
            rigidbody2D.GetAttachedColliders(attachedColliders);
            rigidbody2D.OverlapCollider(default, overlapColliders);
            ignoredColliders.UnionWith(overlapColliders);

            foreach(var attachedCollider in attachedColliders)
            {
                foreach(var ignoredCollider in ignoredColliders)
                {
                    Physics2D.IgnoreCollision(attachedCollider, ignoredCollider, true);
                }
            }
        }

        public void ClearOverlap()
        {
            foreach(var attachedCollider in attachedColliders)
            {
                foreach(var ignoredCollider in ignoredColliders)
                {
                    Physics2D.IgnoreCollision(attachedCollider, ignoredCollider, false);
                }
            }

            attachedColliders.Clear();
            overlapColliders.Clear();
            ignoredColliders.Clear();
        }

        public void UpdateOverlap()
        {
            if(ignoredColliders.Count > 0)
            {
                rigidbody2D.OverlapCollider(default, overlapColliders);

                ignoredColliders.ExceptWith(overlapColliders);

                if(ignoredColliders.Count > 0)
                {
                    for(int i = attachedColliders.Count - 1; i >= 0; i--)
                    {
                        if(attachedColliders[i])
                        {
                            foreach(var ignoredCollider in ignoredColliders)
                            {
                                if(ignoredCollider)
                                {
                                    Physics2D.IgnoreCollision(attachedColliders[i], ignoredCollider, false);
                                }
                            }
                        }
                        else
                        {
                            attachedColliders.RemoveAt(i);
                        }
                    }

                    ignoredColliders.Clear();
                }

                ignoredColliders.UnionWith(overlapColliders);
            }
        }
#endregion

#region Shared Properties
        public float angularDrag
        {
            get => Physics2Dx.is2DNot3D ? rigidbody2D.angularDrag : rigidbody.angularDrag;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.angularDrag = value;
                }
                else
                {
                    rigidbody.angularDrag = value;
                }
            }
        }

        public Vector3 angularVelocity
        {
            get => Physics2Dx.is2DNot3D ? rigidbody2D.angularVelocity * Vector3.forward : rigidbody.angularVelocity;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.angularVelocity = value.z;
                }
                else
                {
                    rigidbody.angularVelocity = value;
                }
            }
        }

        public Vector3 centerOfMass
        {
            get => Physics2Dx.is2DNot3D ? (Vector3)rigidbody2D.centerOfMass : rigidbody.centerOfMass;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.centerOfMass = value;
                }
                else
                {
                    rigidbody.centerOfMass = value;
                }
            }
        }

        public RigidbodyConstraints constraints
        {
            get
            {
                if(Physics2Dx.is2DNot3D)
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
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.constraints = value.ToRigidbodyConstraints2D();
                }
                else
                {
                    rigidbody.constraints = value;
                }
            }
        }

        public float drag
        {
            get => Physics2Dx.is2DNot3D ? rigidbody2D.drag : rigidbody.drag;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.drag = value;
                }
                else
                {
                    rigidbody.drag = value;
                }
            }
        }

        public bool freezeRotation
        {
            get => Physics2Dx.is2DNot3D ? rigidbody2D.freezeRotation : rigidbody.freezeRotation;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.freezeRotation = value;
                }
                else
                {
                    rigidbody.freezeRotation = value;
                }
            }
        }

        public RigidbodyInterpolation interpolation
        {
            get => Physics2Dx.is2DNot3D ? (RigidbodyInterpolation)rigidbody2D.interpolation : rigidbody.interpolation;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.interpolation = (RigidbodyInterpolation2D)value;
                }
                else
                {
                    rigidbody.interpolation = value;
                }
            }
        }

        public bool isKinematic
        {
            get => Physics2Dx.is2DNot3D ? rigidbody2D.isKinematic : rigidbody.isKinematic;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.isKinematic = value;
                }
                else
                {
                    rigidbody.isKinematic = value;
                }
            }
        }

        public float mass
        {
            get => Physics2Dx.is2DNot3D ? rigidbody2D.mass : rigidbody.mass;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.mass = value;
                }
                else
                {
                    rigidbody.mass = value;
                }
            }
        }

        public Vector3 position
        {
            get => Physics2Dx.is2DNot3D ? (Vector3)rigidbody2D.position : rigidbody.position;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.position = value;
                }
                else
                {
                    rigidbody.position = value;
                }
            }
        }

        public Quaternion rotation
        {
            get => Physics2Dx.is2DNot3D ? Quaternion.AngleAxis(rigidbody2D.rotation, Vector3.forward) : rigidbody.rotation;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.rotation = value.eulerAngles.z;
                }
                else
                {
                    rigidbody.rotation = value;
                }
            }
        }

        public Vector3 velocity
        {
            get => Physics2Dx.is2DNot3D ? (Vector3)rigidbody2D.velocity : rigidbody.velocity;
            set
            {
                if(Physics2Dx.is2DNot3D)
                {
                    rigidbody2D.velocity = value;
                }
                else
                {
                    rigidbody.velocity = value;
                }
            }
        }

        public Vector3 worldCenterOfMass => Physics2Dx.is2DNot3D ? (Vector3)rigidbody2D.worldCenterOfMass : rigidbody.worldCenterOfMass;
#endregion

#region Shared Methods
        public void AddForce(Vector3 force) => AddForce(force, ForceMode.Force);
        public void AddForce(Vector3 force, ForceMode mode)
        {
            if(Physics2Dx.is2DNot3D)
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

        public void AddForceAtPosition(Vector3 force, Vector3 position) => AddForceAtPosition(force, position, ForceMode.Force);
        public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode)
        {
            if(Physics2Dx.is2DNot3D)
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

        public void AddRelativeForce(Vector3 force) => AddRelativeForce(force, ForceMode.Force);
        public void AddRelativeForce(Vector3 force, ForceMode mode)
        {
            if(Physics2Dx.is2DNot3D)
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

        public void AddTorque(float torque) => AddTorque(Vector3.forward * torque);
        public void AddTorque(Vector3 torque) => AddTorque(torque, ForceMode.Force);
        public void AddTorque(Vector3 torque, ForceMode mode)
        {
            if(Physics2Dx.is2DNot3D)
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

        public Vector3 GetPointVelocity(Vector3 worldPoint) => Physics2Dx.is2DNot3D ? (Vector3)rigidbody2D.GetPointVelocity(worldPoint) : rigidbody.GetPointVelocity(worldPoint);
        public Vector3 GetRelativePointVelocity(Vector3 relativePoint) => Physics2Dx.is2DNot3D ? (Vector3)rigidbody2D.GetRelativePointVelocity(relativePoint) : rigidbody.GetRelativePointVelocity(relativePoint);
        public bool IsSleeping() => Physics2Dx.is2DNot3D ? rigidbody2D.IsSleeping() : rigidbody.IsSleeping();

        public void MovePosition(Vector3 position)
        {
            if(Physics2Dx.is2DNot3D)
            {
                rigidbody2D.MovePosition(position);
            }
            else
            {
                rigidbody.MovePosition(position);
            }
        }

        public void MoveRotation(Quaternion rotation)
        {
            if(Physics2Dx.is2DNot3D)
            {
                rigidbody2D.MoveRotation(rotation);
            }
            else
            {
                rigidbody.MoveRotation(rotation);
            }
        }

        public void Sleep()
        {
            if(Physics2Dx.is2DNot3D)
            {
                rigidbody2D.Sleep();
            }
            else
            {
                rigidbody.Sleep();
            }
        }

        public void WakeUp()
        {
            if(Physics2Dx.is2DNot3D)
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
