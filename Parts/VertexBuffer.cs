using SpanUtils;
using System;
using System.Runtime.InteropServices;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out VertexBuffer vertexBuffer)
        {
            switch (@this.Id)
            {
                case 4:
                    vertexBuffer = new VertexBuffer(@this.Data);
                    return true;
                default:
                    vertexBuffer = default;
                    return false;
            }
        }
    }

    public readonly ref struct VertexBuffer
    {
        readonly Span<byte> _data;
        public VertexBuffer(Span<byte> data) => _data = data;

        public ref VertexBufferHeader Header => ref _data.View<VertexBufferHeader>(0);
        public readonly struct VertexBufferHeader
        {
            public readonly int ChunkCount;
            public readonly int Stride;
            public readonly int VertexCount;
        }

        unsafe static readonly int ATTRIBUTE_IDS_OFFSET = sizeof(VertexBufferHeader);

        public enum AttributeId : int
        {
            Position,
            Normal,
            Tangent,
            Bitangent,
            UV,
            Weights,
            Bones,
            Bytes = 14 //UNKNOWN
        }

        public static int GetAttributeSize(AttributeId chunkId)
        {
            switch (chunkId)
            {
                case AttributeId.Position:
                case AttributeId.Normal:
                case AttributeId.Tangent:
                case AttributeId.Bitangent:
                    return 3 * sizeof(float);
                case AttributeId.UV: return 2 * sizeof(float);
                case AttributeId.Weights: return 4 * sizeof(float);
                case AttributeId.Bones: return 4 * sizeof(byte);
                case AttributeId.Bytes: return 4;
                default: throw new ArgumentException($"Unknown chunk id: {chunkId}", nameof(chunkId));
            }
        }

        public Span<AttributeId> Attributes => MemoryMarshal.Cast<int, AttributeId>(_data.ViewRange<int>(ATTRIBUTE_IDS_OFFSET, Header.ChunkCount));
        public Span<byte> Buffer
        {
            get
            {
                ref var header = ref Header;
                return _data.Slice(ATTRIBUTE_IDS_OFFSET + sizeof(AttributeId) * header.ChunkCount, header.Stride * header.VertexCount);
            }
        }
    }
}
