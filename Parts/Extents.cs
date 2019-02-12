using SpanUtils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out Span<Extents> extents)
        {
            switch (@this.Id)
            {
                case 10:
                    extents = @this.Data.ViewRange<Extents>(0, 1);
                    return true;
                default:
                    extents = default;
                    return false;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Extents
    {
        public Vector3 Min;
        public Vector3 Max;
    }
}
