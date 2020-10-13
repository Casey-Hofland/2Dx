using UnityEngine;

namespace DimensionConverter
{
    [AddComponentMenu(Settings.componentMenu + "Splitter Converter")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TransformSplitter))]
    public sealed class SplitterConverter : Converter
    {
        #region Required Components
        private TransformSplitter _transformSplitter;
        public TransformSplitter transformSplitter => _transformSplitter ? _transformSplitter : (_transformSplitter = GetComponent<TransformSplitter>());
        #endregion

        public override void ConvertTo2D()
        {
            transformSplitter.gameObject3D.SetActive(false);
            transformSplitter.gameObject2D.SetActive(true);
        }

        public override void ConvertTo3D()
        {
            transformSplitter.gameObject2D.SetActive(false);
            transformSplitter.gameObject3D.SetActive(true);
        }
    }
}
