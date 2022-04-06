#nullable enable
namespace Unity2Dx
{
    public interface IConvertible : IConverter
    {
        public bool is2DNot3D { get; }
        public void Convert(bool convertTo2DNot3D);
    }
}
