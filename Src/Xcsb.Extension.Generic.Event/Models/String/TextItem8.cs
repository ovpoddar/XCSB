using System.Text;

namespace Xcsb.Extension.Generic.Event.Models.String
{
    public struct TextItem8
    {
        private readonly byte[] _content;
        public int Count { get; }
        public byte Delta { get; set; } = 0;
        public TextItem8(ReadOnlySpan<byte> str, byte delta = 0)
        {
            _content = str.ToArray();
            Count = str.Length + 2;
            Delta = delta;
        }
        public TextItem8(string str, byte delta = 0)
        {
            _content = Encoding.UTF8.GetBytes(str);
            Count = str.Length + 2;
            Delta = delta;
        }

        public static implicit operator TextItem8(ReadOnlySpan<byte> text) => new(text);
        public static implicit operator TextItem8(string text) => new(text);

        public readonly int CopyTo(Span<byte> destination)
        {
            destination[0] = (byte)_content.Length;// TODO: case if cross 255 what happend then
            destination[1] = Delta;
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