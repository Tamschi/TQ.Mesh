using SpanUtils;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out IndexBuffer vertexBuffer)
        {
            switch (@this.Id)
            {
                case 5:
                    vertexBuffer = new IndexBuffer(@this.Data, @this.Version);
                    return true;
                default:
                    vertexBuffer = default;
                    return false;
            }
        }
    }

    public readonly ref struct IndexBuffer
    {
        readonly Span<byte> _data;
        readonly byte _version;
        public IndexBuffer(Span<byte> data, byte version)
        {
            _data = data;
            _version = version;
        }

        public ref int TriangleCount => ref _data.View<int>(0);
        public ref int DrawCallCount => ref _data.View<int>(sizeof(int));
        public Span<TriangleIndices> TriangleIndices => _data.ViewRange<TriangleIndices>(2 * sizeof(int), TriangleCount);

        unsafe int _DrawCallOffset => 2 * sizeof(int) + sizeof(TriangleIndices) * TriangleCount;
        public Enumerator GetEnumerator() => new Enumerator(_data.Slice(_DrawCallOffset), _version);
        public ref struct Enumerator
        {
            readonly Span<byte> _data;
            readonly byte _version;
            int _offset;

            public Enumerator(Span<byte> data, byte version)
            {
                _data = data;
                _version = version;
                _offset = -1;
            }

            public DrawCall Current => new DrawCall(_data.Slice(_offset, _CurrentSize), _version);

            public bool MoveNext()
            {
                if (_offset == -1) _offset = 0;
                else _offset += _CurrentSize;
                switch (_offset.CompareTo(_data.Length))
                {
                    case var x when x < 0: return true;
                    case var x when x == 0: return false;
                    default /* var x when x > 0 */: throw new InvalidOperationException("Tried to move beyond end of data.");
                }
            }

            unsafe int _BoneCountInnerOffset => sizeof(DrawCallCommon) + (_version == 11 ? sizeof(DrawCall11) : 0);
            int _CurrentBoneCount => _data.View<int>(_offset + _BoneCountInnerOffset);
            int _CurrentSize => _BoneCountInnerOffset + sizeof(int) + _CurrentBoneCount * sizeof(int);
        }
    }

    public readonly ref struct DrawCall
    {
        readonly Span<byte> _data;
        readonly byte _version;

        public DrawCall(Span<byte> data, byte version)
        {
            _data = data;
            _version = version;
        }

        public ref DrawCallCommon Common => ref _data.View<DrawCallCommon>(0);
        public ref DrawCall11 At11
        {
            get
            {
                if (_version == 11) return ref _data.View<DrawCall11>(Marshal.SizeOf<DrawCallCommon>());
                else throw new InvalidOperationException("Only available at version 11.");
            }
        }
        unsafe int _BoneCountInnerOffset => sizeof(DrawCallCommon) + (_version == 11 ? sizeof(DrawCall11) : 0);
        public Span<int> BoneMap => _data.ViewRange<int>(_BoneCountInnerOffset + sizeof(int), _data.View<int>(_BoneCountInnerOffset));
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct TriangleIndices
    { public readonly ushort A, B, C; }

    [StructLayout(LayoutKind.Sequential)]
    public struct DrawCallCommon
    {
        public int ShaderID;
        public int StartFaceIndex;
        public int FaceCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DrawCall11
    {
        public int SubShader;
        public Vector3 Min;
        public Vector3 Max;
    }
}
