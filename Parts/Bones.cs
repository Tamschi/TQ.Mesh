using System;
using System.Runtime.InteropServices;
using static TQ.Mesh.Mesh;

namespace TQ.Mesh.Parts
{
    public static partial class PartDestructuringExtensions
    {
        public static bool Is(this Part @this, out Bones bones)
        {
            switch (@this.Id)
            {
                case 6:
                    bones = new Bones(@this.Data);
                    return true;
                default:
                    bones = default;
                    return false;
            }
        }
    }

    public readonly ref struct Bones
    {
        readonly Span<byte> _data;

        public Bones(Span<byte> data) => _data = data;

        Span<BoneData> _bonesData => _data.Slice(sizeof(int)).Cast<BoneData>();
        public Enumerator GetEnumerator() => new Enumerator(_bonesData, skip: 0, take: Count);
        public int Count => _data.View<int>(0);

        public Bone this[int index] => new Bone(_bonesData, index);

        public ref struct Enumerator
        {
            readonly Span<BoneData> _data;
            int _current;

            readonly int _skip;
            readonly int _end;

            internal Enumerator(Span<BoneData> data, int skip, int take)
            {
                _data = data;
                _current = -1 + skip;
                _skip = skip;
                _end = skip + take;
            }
            public Bone Current => new Bone(_data, _current);
            public bool MoveNext()
            {
                _current++;
                switch (_current.CompareTo(_end))
                {
                    case var x when x < 0: return true;
                    case var x when x == 0: return false;
                    default /* var x when x > 0 */: throw new InvalidOperationException("Tried to move beyond end of data.");
                }
            }

            public void Reset() => _current = -1 + _skip;
        }

        public ref struct Bone
        {
            readonly Span<BoneData> _bonesData;
            readonly int _index;
            internal Bone(Span<BoneData> bonesData, int index)
            {
                _bonesData = bonesData;
                _index = index;
            }

            public unsafe string Name
            {
                get
                {
                    fixed (byte* namePtr = _bonesData[_index].Name)
                    { return Utils.Encoding.GetString(namePtr, 32).TrimEnd('\0'); }
                }
            }

            int _firstChild => _bonesData[_index].FirstChild;
            public int ChildCount => _bonesData[_index].ChildCount;
            public Enumerator GetEnumerator() => new Enumerator(_bonesData, skip: _firstChild, take: ChildCount);

            public unsafe Span<float> Axes
            {
                get
                {
                    fixed (float* axesPtr = _bonesData[_index].Axes)
                    { return MemoryMarshal.CreateSpan(ref *axesPtr, 9); }
                }
            }

            public unsafe Span<float> Position
            {
                get
                {
                    fixed (float* positionPtr = _bonesData[_index].Position)
                    { return MemoryMarshal.CreateSpan(ref *positionPtr, 3); }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct BoneData
        {
            public fixed byte Name[32];
            public readonly int FirstChild;
            public readonly int ChildCount;
            public fixed float Axes[9];
            public fixed float Position[3];
        }
    }
}
