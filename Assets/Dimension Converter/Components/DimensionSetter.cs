using UnityEngine;
using UnityEngine.SceneManagement;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Dimension Setter")]
    [DisallowMultipleComponent]
    public class DimensionSetter : MonoBehaviour
    {
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //private static void Init()
        //{
        //    SceneManager.sceneLoaded += OnSceneLoaded;
        //}

        //private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        //{
        //    foreach(var gameObject in scene.GetRootGameObjects())
        //    {
        //        foreach(var setter in gameObject.GetComponentsInChildren<DimensionSetter>())
        //        {
        //            Debug.Log($"Setter");
        //            Dimension.is2DNot3D = setter.is2DNot3D;
        //        }
        //    }
        //}

        [Header("Settings on Awake")]
        public bool is2DNot3D;

        private void Awake()
        {
            Debug.Log($"Setter");
            Dimension.onBeforeConvert += _ => Debug.Log("Before Convert");
            Dimension.onAfterConvert += _ => Debug.Log("After Convert");

            var conversionTime = Dimension.conversionTime;
            Dimension.conversionTime = 0f;

            Dimension.is2DNot3D = is2DNot3D;
            
            Dimension.conversionTime = conversionTime;

        }
}
}
