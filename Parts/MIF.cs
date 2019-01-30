using System;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out MIF mif)
        {
            switch (@this.Id)
            {
                case 0:
                    mif = new MIF(@this.Data);
                    return true;
                default:
                    mif = default;
                    return false;
            }
        }
    }

    public readonly ref struct MIF
    {
        readonly Span<byte> _data;

        public MIF(Span<byte> data) => _data = data;

        public string Text
            => Utils.Encoding.GetString(_data.Slice(
                start: sizeof(int),
                length: _data.View<int>(0)
            ));
    }
}
