using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

public class StringFeedback : IFeedback
{
    private readonly uint[] _keysyms;
    internal readonly StrFeedback m_feedback;

    public StringFeedback(byte feedbackId, uint[] keysyms)
    {
        _keysyms = keysyms;
        m_feedback = new StrFeedback(feedbackId, (ushort)keysyms.Length);
        ClassId = m_feedback.ClassId;
        FeedbackId = m_feedback.FeedbackId;
        Length = m_feedback.Length;
    }

    public FeedbackClass ClassId { get; }
    public byte FeedbackId { get; }
    public ushort Length { get; }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal readonly struct StrFeedback(byte feedbackId, ushort numKeysyms) : IFeedback
    {
        public readonly FeedbackClass ClassId { get; } = FeedbackClass.String;
        public readonly byte FeedbackId { get; } = feedbackId;
        public readonly ushort Length { get; } = (ushort)(2 + numKeysyms);
        private readonly ushort _pad0 = 0;
        public readonly ushort NumKeysyms = numKeysyms;
    }
}