using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "On Before Convert")]
    public class OnBeforeConvert : MonoBehaviour
    {
        public ConvertEvent onBeforeConvert = new ConvertEvent();

        private void OnEnable()
        {
            Dimension.onBeforeConvert += InvokeEvent;
        }

        private void OnDisable()
        {
            Dimension.onBeforeConvert -= InvokeEvent;
        }

        private void InvokeEvent(bool to2DNot3D)
        {
            onBeforeConvert.Invoke(to2DNot3D);
        }
    }
}
