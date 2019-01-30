using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace TQ.Mesh
{
    public readonly ref struct Mesh
    {
        public Span<byte> Data { get; }
        public Mesh(Span<byte> data) => Data = data;

        [EditorBrowsable(EditorBrowsableState.Never)] public override bool Equals(object obj) => throw new NotSupportedException();
        [EditorBrowsable(EditorBrowsableState.Never)] public override int GetHashCode() => throw new NotSupportedException();
        [EditorBrowsable(EditorBrowsableState.Never)] public override string ToString() => throw new NotImplementedException();

        private static readonly byte[] Magic = Utils.Encoding.GetBytes("MSH");

        public Enumerator GetEnumerator() => new Enumerator(Data.Slice(4)); //TODO: Do this more nicely.

        public ref struct Enumerator
        {

            private readonly Span<byte> _data;
            private int _offset;

            public Enumerator(Span<byte> data)
            {
                _data = data;
                _offset = 0;
            }

            static unsafe readonly int PartHeaderSize = sizeof(Part.Header);

            public Part Current => new Part(_currentId, _data.Slice(_offset + PartHeaderSize, _currentLength));

            private ref Part.Header _currentPartHeader => ref _data.View<Part.Header>(_offset);
            private PartId _currentId => _currentPartHeader.id;
            private int _currentLength => _currentPartHeader.length;

            public bool MoveNext()
            {
                unsafe { _offset += sizeof(Part.Header) + _currentLength; }
                if (_offset < _data.Length) return true;
                else if (_offset == _data.Length) return false;
                else /* _offset > _data.Length */ throw new InvalidOperationException("Tried to move beyond end of data.");
            }

            public void Reset() => _offset = 0;
        }

        public readonly ref struct Part
        {
            internal PartId Id { get; }
            internal Span<byte> Data { get; }

            public Part(PartId id, Span<byte> data)
            {
                Id = id;
                Data = data;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal readonly struct Header
            {
                public readonly PartId id;
                public readonly int length;
            }
        }

        public enum PartId : int
        {
            MIF = 0
        }
    }
}
