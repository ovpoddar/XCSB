using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

public interface IFeedback
{
    FeedbackClass ClassId { get; }
    byte FeedbackId { get; }
    ushort Length { get; }
}