using SpanUtils;
using System;
using System.Collections.Generic;
using System.Text;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out Extents extents)
        {
            switch (@this.Id)
            {
                case 10:
                    extents = new Extents(@this.Data);
                    return true;
                default:
                    extents = default;
                    return false;
            }
        }
    }

    public readonly ref struct Extents
    {
        readonly Span<float> _data;
        public Extents(Span<byte> data) => _data = data.Cast<float>();

        public ref float MinX => ref _data[0];
        public ref float MinY => ref _data[1];
        public ref float MinZ => ref _data[2];
        public ref float MaxX => ref _data[3];
        public ref float MaxY => ref _data[4];
        public ref float MaxZ => ref _data[5];
    }
}
