using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Splitter Converter")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TransformSplitter))]
    public sealed class SplitterConverter : Converter
    {
        #region Required Components
        private TransformSplitter _transform2Dx;
        public TransformSplitter transform2Dx => _transform2Dx ? _transform2Dx : (_transform2Dx = GetComponent<TransformSplitter>());
        #endregion

        public override void ConvertTo2D()
        {
            transform2Dx.gameObject3D.SetActive(false);
            transform2Dx.gameObject2D.SetActive(true);
        }

        public override void ConvertTo3D()
        {
            transform2Dx.gameObject2D.SetActive(false);
            transform2Dx.gameObject3D.SetActive(true);
        }
    }
}
