using System;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out Shaders shaders)
        {
            switch (@this.Id)
            {
                case 7:
                    shaders = new Shaders(@this.Data);
                    return true;
                default:
                    shaders = default;
                    return false;
            }
        }
    }

    public readonly ref struct Shaders
    {
        readonly Span<byte> _data;

        public Shaders(Span<byte> data) => _data = data;

        public Enumerator GetEnumerator() => new Enumerator(
            data: _data.Slice(sizeof(int)),
            count: _data.View<int>(0));

        public ref struct Enumerator
        {
            readonly Span<byte> _data;
            readonly int _count;
            int _offset;
            int _index;

            public Enumerator(Span<byte> data, int count)
            {
                _data = data;
                _count = count;
                _offset = -1;
                _index = -1;
            }

            public bool MoveNext()
            {
                if (_offset == -1) _offset = 0;
                else _offset += Current.BinarySize;
                _index++;
                switch (_index)
                {
                    case var x when x < _count: return true;
                    case var x when x == _count: return false;
                    default /* var x when x > _count */: throw new InvalidOperationException("Tried to move to or beyond shader count.");
                }
            }
            public Shader Current => new Shader(_data.Slice(_offset));
        }
    }

    public readonly ref struct Shader
    {
        readonly Span<byte> _data;
        public Shader(Span<byte> data) => _data = data;

        public string FileName => Utils.Encoding.GetString(_data.Slice(sizeof(int), _data.View<int>(0)));
        public Enumerator GetEnumerator() => new Enumerator(
            data: _data.Slice(sizeof(int) + _data.View<int>(0) + sizeof(int)),
            count: _data.View<int>(sizeof(int) + _data.View<int>(0)));

        public ref struct Enumerator
        {
            readonly Span<byte> _data;
            readonly int _count;
            int _offset;
            int _index;

            public Enumerator(Span<byte> data, int count)
            {
                _data = data;
                _count = count;
                _offset = -1;
                _index = -1;
            }

            public bool MoveNext()
            {
                if (_offset == -1) _offset = 0;
                else _offset += Current.BinarySize;
                _index++;
                switch (_index)
                {
                    case var x when x < _count: return true;
                    case var x when x == _count: return false;
                    default /* var x when x > _count */: throw new InvalidOperationException("Tried to move to or beyond parameter count.");
                }
            }
            public Parameter Current => new Parameter(_data.Slice(_offset));
        }

        public int BinarySize => sizeof(int) + _data.View<int>(0) + sizeof(int) + _ParametersSize;
        int _ParametersSize
        {
            get
            {
                int total = 0;
                foreach (var parameter in this) total += parameter.BinarySize;
                return total;
            }
        }
    }

    public readonly ref struct Parameter
    {
        readonly Span<byte> _data;
        public Parameter(Span<byte> data) => _data = data;

        public int BinarySize => _ValueOffset + (
            _Type == Type.String ? sizeof(int) + _data.View<int>(_ValueOffset)
                : ValueIs(out Span<float> value) ? value.Length * sizeof(float)
                : throw new NotImplementedException("Unknown shader parameter type: " + _Type)
            );

        int _NameLength => _data.View<int>(0);
        public string Name => Utils.Encoding.GetString(_data.Slice(sizeof(int), _data.View<int>(0)));

        enum Type : int
        {
            String = 7,
            TwoFloats = 8,
            ThreeFloats = 9,
            FloatId = 10
        }

        int _TypeOffset => sizeof(int) + _NameLength;
        Type _Type => (Type)_data.View<int>(_TypeOffset);
        int _ValueOffset => _TypeOffset + sizeof(int);
        public bool ValueIs(out string value)
        {
            if (_Type == Type.String)
            {
                value = Utils.Encoding.GetString(_data.Slice(_ValueOffset + sizeof(int), _data.View<int>(_ValueOffset)));
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
        public bool ValueIs(out Span<float> value)
        {
            switch (_Type)
            {
                case Type.TwoFloats:
                    value = _data.ViewRange<float>(_ValueOffset, 2);
                    return true;
                case Type.ThreeFloats:
                    value = _data.ViewRange<float>(_ValueOffset, 3);
                    return true;
                case Type.FloatId:
                    value = _data.ViewRange<float>(_ValueOffset, 1);
                    return true;
                default:
                    value = default;
                    return false;
            }
        }
    }
}
