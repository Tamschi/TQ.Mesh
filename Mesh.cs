using SpanUtils;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TQ.Common;

namespace TQ.Mesh
{
    public readonly ref struct Mesh
    {
        internal Span<byte> Data { get; }
        public Mesh(Span<byte> data)
        {
            Data = data;
            for (int i = 0; i < Magic.Length; i++)
            {
                if (Data[i] != Magic[i]) throw new ArgumentException("Wrong magic.", nameof(data));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)] public override bool Equals(object obj) => throw new NotSupportedException();
        [EditorBrowsable(EditorBrowsableState.Never)] public override int GetHashCode() => throw new NotSupportedException();
        [EditorBrowsable(EditorBrowsableState.Never)] public override string ToString() => throw new NotImplementedException();

        private static readonly byte[] Magic = Definitions.Encoding.GetBytes("MSH");
        public ref byte Version => ref Data.View<byte>(Magic.Length);

        public Enumerator GetEnumerator() => new Enumerator(Data.Slice(4), Version); //TODO: Do this more nicely.

        public ref struct Enumerator
        {
            private readonly Span<byte> _data;
            private readonly byte _version;
            private int _offset;

            public Enumerator(Span<byte> data, byte version)
            {
                _data = data;
                _version = version;
                _offset = -1;
            }

            static unsafe readonly int PartHeaderSize = sizeof(Part.Header);

            public Part Current => new Part(_currentPartHeader.id, _data.Slice(_offset + PartHeaderSize, _currentPartHeader.length), _version);

            private ref Part.Header _currentPartHeader => ref _data.View<Part.Header>(_offset);

            public bool MoveNext()
            {
                if (_offset == -1) _offset = 0;
                else unsafe { _offset += sizeof(Part.Header) + _currentPartHeader.length; }
                switch (_offset.CompareTo(_data.Length))
                {
                    case var x when x < 0: return true;
                    case var x when x == 0: return false;
                    default /* var x when x > 0 */: throw new InvalidOperationException("Tried to move beyond end of data.");
                }
            }

            public void Reset() => _offset = -1;
        }

        public readonly ref struct Part
        {
            public int Id { get; }
            public Span<byte> Data { get; }
            public byte Version { get; }

            public Part(int id, Span<byte> data, byte version)
            {
                Id = id;
                Data = data;
                Version = version;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal readonly struct Header
            {
                public readonly int id;
                public readonly int length;
            }
        }
    }
}
