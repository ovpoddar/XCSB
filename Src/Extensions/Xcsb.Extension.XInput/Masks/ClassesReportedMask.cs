using System;

namespace Xcsb.Extension.XInput.Masks;

[Flags]
public enum ClassesReportedMask : byte
{
    ReportingKeys = 1,
    ReportingButtons = 2,
    ReportingValuators = 4,
    DeviceModeAbsolute = 64,
    OutOfProximity = 128,
}