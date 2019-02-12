using SpanUtils;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using TQ.Common;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out Span<Hitbox> hitboxes)
        {
            switch (@this.Id)
            {
                case 8:
                    hitboxes = @this.Data.ViewRange<Hitbox>(sizeof(int), @this.Data.View<int>(0));
                    return true;
                default:
                    hitboxes = default;
                    return false;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Hitbox
    {
        fixed byte _name[32];
        public Vector3 Position1;
        fixed float _axes[9];
        public Vector3 Position2;
        fixed byte _unknown[4];

        public string Name
        {
            get
            {
                fixed (byte* namePtr = _name)
                { return Definitions.Encoding.GetString(namePtr, 32).TrimEnd('\0'); }
            }
        }

        public Span<Vector3> Axes { get { fixed (float* axesPtr = _axes) { return MemoryMarshal.Cast<float, Vector3>(MemoryMarshal.CreateSpan(ref *axesPtr, 9)); } } }
        public Span<byte> Unknown { get { fixed (byte* unknownPtr = _unknown) { return MemoryMarshal.CreateSpan(ref *unknownPtr, 4); } } }
    }
}
