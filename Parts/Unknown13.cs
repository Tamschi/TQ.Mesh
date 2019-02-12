using System;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out Unknown13 unknown13)
        {
            switch (@this.Id)
            {
                case 13:
                    unknown13 = new Unknown13(@this.Data);
                    return true;
                default:
                    unknown13 = default;
                    return false;
            }
        }
    }

    public readonly ref struct Unknown13
    {
        public Span<byte> Data { get; }

        public Unknown13(Span<byte> data) => Data = data;
    }
}
