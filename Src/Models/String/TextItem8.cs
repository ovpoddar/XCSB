using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models.String
{
    public struct TextItem8
    {
        private readonly byte[] _content;
        public int Count { get; }
        public TextItem8(ReadOnlySpan<byte> str)
        {
            _content = str.ToArray();
            Count = str.Length + 2;
        }
        public TextItem8(string str)
        {
            _content = Encoding.UTF8.GetBytes(str);
            Count = str.Length + 2;
        }

        public static implicit operator TextItem8(ReadOnlySpan<byte> text) => new(text);
        public static implicit operator TextItem8(string text) => new(text);

        public readonly int CopyTo(Span<byte> destination)
        {
            destination[0] = (byte)_content.Length;// TODO: case if cross 255 what happend then
            destination[1] = 0; //TODO: CHECK DELTA
            _content.CopyTo(destination[2..]);
            return Count;
        }

        public readonly byte[] ToArray()
        {
            var result = new byte[Count];
            this.CopyTo(result);
            return result;
        }
    }
}