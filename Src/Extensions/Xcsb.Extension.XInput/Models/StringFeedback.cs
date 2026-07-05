using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

public class StringFeedback : IFeedback
{
    internal readonly StrFeedback m_feedback;
    internal readonly uint[] m_keysyms;

    public StringFeedback(byte feedbackId, uint[] mKeysyms)
    {
        m_keysyms = mKeysyms;
        m_feedback = new StrFeedback(feedbackId, (ushort)mKeysyms.Length);
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