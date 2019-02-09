using SpanUtils;
using System;
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
        fixed float _position1[3];
        fixed float _axes[9];
        fixed float _position2[3];
        fixed byte _unknown[4];

        public string Name
        {
            get
            {
                fixed (byte* namePtr = _name)
                { return Definitions.Encoding.GetString(namePtr, 32).TrimEnd('\0'); }
            }
        }

        public Span<float> Position1 { get { fixed (float* position1Ptr = _position1) { return MemoryMarshal.CreateSpan(ref *position1Ptr, 3); } } }
        public Span<float> Axes { get { fixed (float* axesPtr = _axes) { return MemoryMarshal.CreateSpan(ref *axesPtr, 9); } } }
        public Span<float> Position2 { get { fixed (float* position2Ptr = _position2) { return MemoryMarshal.CreateSpan(ref *position2Ptr, 3); } } }
        public Span<byte> Unknown { get { fixed (byte* unknownPtr = _unknown) { return MemoryMarshal.CreateSpan(ref *unknownPtr, 4); } } }
    }
}
