using System;
using System.Runtime.InteropServices;

namespace TQ.Mesh
{
    internal static class UtilExtensions
    {
        public static ref T View<T>(this Span<byte> @this, int offset) where T : struct => ref MemoryMarshal.Cast<byte, T>(@this.Slice(offset, Marshal.SizeOf<T>()))[0];
        public static Span<T> ViewRange<T>(this Span<byte> @this, int offset, int count) where T : struct => MemoryMarshal.Cast<byte, T>(@this.Slice(offset, Marshal.SizeOf<T>() * count));
        public static Span<R> Cast<R>(this Span<byte> @this) where R : struct => MemoryMarshal.Cast<byte, R>(@this);
    }
}
