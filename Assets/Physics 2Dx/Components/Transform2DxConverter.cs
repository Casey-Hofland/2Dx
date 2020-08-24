using UnityEngine;

namespace Physics2DxSystem
{
    [AddComponentMenu(Physics2Dx.componentMenu + "Transform 2Dx Converter")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform2Dx))]
    public sealed class Transform2DxConverter : Module2Dx
    {
        #region Required Components
        private Transform2Dx _transform2Dx;
        public Transform2Dx transform2Dx => _transform2Dx ? _transform2Dx : (_transform2Dx = GetComponent<Transform2Dx>());
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
