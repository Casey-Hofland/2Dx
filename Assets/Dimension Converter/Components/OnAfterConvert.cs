using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "On After Convert")]
    public class OnAfterConvert : MonoBehaviour
    {
        public ConvertEvent onAfterConvert = new ConvertEvent();

        private void OnEnable()
        {
            Dimension.onAfterConvert += InvokeEvent;
        }

        private void OnDisable()
        {
            Dimension.onAfterConvert -= InvokeEvent;
        }

        private void InvokeEvent(bool to2DNot3D)
        {
            onAfterConvert.Invoke(to2DNot3D);
        }
    }
}
