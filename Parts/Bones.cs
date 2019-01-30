using System;
using System.Runtime.InteropServices;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out Span<Bone> bones)
        {
            switch (@this.Id)
            {
                case 6:
                    bones = @this.Data.ViewRange<Bone>(
                        count: @this.Data.View<int>(0),
                        offset: sizeof(int));
                    return true;
                default:
                    bones = default;
                    return false;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Bone
    {
        fixed byte _name[32];
        public string Name
        {
            get
            {
                fixed (byte* ptr = _name)
                { return Utils.Encoding.GetString(ptr, 32).TrimEnd('\0'); }
            }
        }

        public readonly int FirstChild;
        public readonly int ChildCount;
        public fixed float Axes[9];
        public fixed float Position[3];
    }
}
