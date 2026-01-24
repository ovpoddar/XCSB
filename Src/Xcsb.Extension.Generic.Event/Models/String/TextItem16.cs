using System.Text;

namespace Xcsb.Extension.Generic.Event.Models.String
{
    public struct TextItem16
    {
        private static readonly Encoding _encoding = new UnicodeEncoding(true, false);
        private readonly string _content;
        public int Count { get; }
        public byte Delta { get; set; } = 0;

        public TextItem16(string content, byte delta = 0)
        {
            _content = content;
            Count = (content.Length + 1) * 2;
            Delta = delta;
        }

        public static implicit operator TextItem16(string str) => new(str);

        public int CopyTo(Span<byte> destination)
        {
            destination[0] = (byte)_content.Length; // TODO: case if cross 255 what happend then
            destination[1] = Delta;
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