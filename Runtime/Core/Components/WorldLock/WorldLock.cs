#nullable enable
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.SceneManagement;

namespace Unity2Dx
{
    public sealed class WorldLock : MonoBehaviour
    {
        public static Vector2 lastGravity2D { get; set; }
        public static Vector3 rotatedGravity2D { get; set; }

        private static readonly int _Rotation = Shader.PropertyToID(nameof(_Rotation));
        private static HashSet<Material> rotatedSkyboxes = new HashSet<Material>();
        private static List<GameObject> rootGameObjects = new List<GameObject>();
        private static JobHandle rotateAroundHandle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration()
        {
            rotatedSkyboxes = new HashSet<Material>();
            rootGameObjects = new List<GameObject>();
        }

        [field: SerializeField] public Vector3 forward { get; set; } = Vector3.forward;
        [field: SerializeField] public bool rotateGravity { get; set; } = true;
        [field: SerializeField] public bool rotateSkybox { get; set; } = true;

        public Quaternion totalRotation { get; private set; } = Quaternion.identity;

        public static event OnRotateWorld? worldRotated;

        private TransformAccessArray transformAccessArray;

        private void Awake()
        {
            transformAccessArray = new TransformAccessArray(1);
        }

        private bool started;

        private void OnEnable()
        {
            RotateWorld();

            if (started)
            {
                SceneManager.sceneLoaded += SceneLoaded;
            }
        }

        private void Start()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            started = true;
        }

        private void Update()
        {
            rotateAroundHandle.Complete();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RotateScene(totalRotation, scene);
        }

        private void OnDestroy()
        {
            transformAccessArray.Dispose();
        }

        public void RotateWorld()
        {
            if (Quaternion.Angle(transform.rotation, Quaternion.identity) <= Quaternion.kEpsilon)
            {
                return;
            }
            var worldRotation = Quaternion.LookRotation(forward) * Quaternion.Inverse(transform.rotation);

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                RotateScene(worldRotation, scene);
            }

            if (rotateGravity)
            {
                RotateGravity(worldRotation);
            }

            if (rotateSkybox)
            {
                RotateSkybox(worldRotation);
            }

            totalRotation = worldRotation * totalRotation;

            // Send event.
            worldRotated?.Invoke(transform.position, worldRotation);
        }

        private void RotateScene(Quaternion rotation, Scene scene)
        {
            scene.GetRootGameObjects(rootGameObjects);

            Transform[] rootTransforms = new Transform[rootGameObjects.Count];
            for (int i = 0; i < rootGameObjects.Count; i++)
            {
                rootTransforms[i] = rootGameObjects[i].transform;
            }
            RotateTransforms(rotation, rootTransforms);
        }

        private void RotateTransforms(Quaternion rotation, params Transform[] transforms)
        {
            transformAccessArray.capacity = Mathf.Max(transformAccessArray.capacity, transforms.Length);

            for (int i = 0; i < transforms.Length; i++)
            {
                var transform = transforms[i];

                if (i < transformAccessArray.length)
                {
                    transformAccessArray[i] = transform;
                }
                else
                {
                    transformAccessArray.Add(transform);
                }
            }

            var rotateAroundJob = new RotateAroundJob(transform.position, rotation);
            var jobHandle = rotateAroundJob.Schedule(transformAccessArray);

            rotateAroundHandle = JobHandle.CombineDependencies(rotateAroundHandle, jobHandle);
        }

        private static void RotateGravity(Quaternion rotation)
        {
            // Rotate 3D gravity.
#if PHYSICS_MODULE
            Physics.gravity = rotation * Physics.gravity;
#endif

#if CLOTH_MODULE
            Physics.clothGravity = rotation * Physics.clothGravity;
#endif

#if PHYSICS2D_MODULE
            // Rotate 2D gravity.
            if (Physics2D.gravity != lastGravity2D)
            {
                rotatedGravity2D = Physics2D.gravity;
            }

            lastGravity2D = Physics2D.gravity = rotatedGravity2D = rotation * rotatedGravity2D;
#endif
        }

        private void RotateSkybox(Quaternion rotation)
        {
            if (!RenderSettings.skybox
                || !RenderSettings.skybox.HasProperty(_Rotation))
            {
                return;
            }

            // TODO: Check to see how skybox should be reset. Think about scene switching. Strongly prefer a solution that removes Application.quitting binding!
            // TODO: Add more 360 skybox shaders.
            var vector = RenderSettings.skybox.GetVector(_Rotation);
            if (rotatedSkyboxes.Add(RenderSettings.skybox))
            {
                var skybox = RenderSettings.skybox;
                var startVector = vector;
                Application.quitting -= ResetVectorOnQuit;
                Application.quitting += ResetVectorOnQuit;

                void ResetVectorOnQuit()
                {
                    skybox.SetVector(_Rotation, startVector);
                }
            }

            var vectorRotation = new Quaternion(vector.x, vector.y, vector.z, vector.w);
            rotation *= vectorRotation;
            vector.Set(rotation.x, rotation.y, rotation.z, rotation.w);

            RenderSettings.skybox.SetVector(_Rotation, vector);
        }
    }
}
