using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "On After Convert")]
    public class OnAfterConvert : MonoBehaviour
    {
        public OnConvertFlags triggerWhen = ~OnConvertFlags.None;
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
            if(to2DNot3D)
            {
                if(!triggerWhen.HasFlag(OnConvertFlags.To2D))
                {
                    return;
                }
            }
            else if(!triggerWhen.HasFlag(OnConvertFlags.To3D))
            {
                return;
            }

            onAfterConvert.Invoke(to2DNot3D);
        }
    }
}
