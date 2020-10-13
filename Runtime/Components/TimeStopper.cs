using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Time Stopper")]
    [DisallowMultipleComponent]
    public class TimeStopper : MonoBehaviour
    {
        public float afterConvertTimeScale { get; private set; }

        private void OnEnable()
        {
            afterConvertTimeScale = Time.timeScale;
            Dimension.onBeforeConvert += _ => StopTime();
            Dimension.onAfterConvert += _ => StartTime();
        }

        private void OnDisable()
        {
            Dimension.onBeforeConvert -= _ => StopTime();
            Dimension.onAfterConvert -= _ => StartTime();
        }

        public void StopTime()
        {
            afterConvertTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        public void StartTime()
        {
            Time.timeScale = afterConvertTimeScale;
        }
    }

}
