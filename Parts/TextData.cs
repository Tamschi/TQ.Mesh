using System;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out TextData textData)
        {
            switch (@this.Id)
            {
                case 3:
                    textData = new TextData(@this.Data);
                    return true;
                default:
                    textData = default;
                    return false;
            }
        }
    }

    public readonly ref struct TextData
    {
        public Span<byte> Data { get; }

        public TextData(Span<byte> data) => Data = data;

        public string Text => Utils.Encoding.GetString(Data);
    }
}
