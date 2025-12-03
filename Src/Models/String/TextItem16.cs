using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models.String
{
    public struct TextItem16
    {
        private static readonly Encoding _encoding = new UnicodeEncoding(true, false);
        private readonly string _content;

        public int Count { get; }

        public TextItem16(string content)
        {
            _content = content;
            Count = (content.Length + 1) * 2;
        }

        public static implicit operator TextItem16(string str) => new(str);

        public int CopyTo(Span<byte> destination)
        {
            destination[0] = (byte)_content.Length; // TODO: case if cross 255 what happend then
            destination[1] = 0; //TODO: CHECK DELTA
            return _encoding.GetBytes(_content, destination.Slice(2, _content.Length * 2)) + 2;
        }

        public byte[] ToArray()
        {
            var result = new byte[Count];
            this.CopyTo(result);
            return result;
        }
    }
}