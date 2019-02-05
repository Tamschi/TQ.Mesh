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

        readonly struct Header
        {
            public readonly int ChunkCount;
            public readonly int Stride;
            public readonly int VertexCount;
        }

        unsafe static readonly int CHUNK_IDS_OFFSET = sizeof(Header);

        public enum ChunkId : int
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

        public static int GetChunkSize(ChunkId chunkId)
        {
            switch (chunkId)
            {
                case ChunkId.Position:
                case ChunkId.Normal:
                case ChunkId.Tangent:
                case ChunkId.Bitangent:
                    return 3 * sizeof(float);
                case ChunkId.UV: return 2 * sizeof(float);
                case ChunkId.Weights: return 4 * sizeof(float);
                case ChunkId.Bones: return 4 * sizeof(byte);
                case ChunkId.Bytes: return 4;
                default: throw new ArgumentException($"Unknown chunk id: {chunkId}", nameof(chunkId));
            }
        }

        public Span<ChunkId> Chunks => MemoryMarshal.Cast<int, ChunkId>(_data.ViewRange<int>(CHUNK_IDS_OFFSET, _data.View<Header>(0).ChunkCount));
        public Span<byte> Buffer
        {
            get
            {
                ref var header = ref _data.View<Header>(0);
                return _data.Slice(CHUNK_IDS_OFFSET + sizeof(ChunkId) * header.ChunkCount, header.Stride * header.VertexCount);
            }
        }
    }
}
