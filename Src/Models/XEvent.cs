using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Event;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;

namespace Xcsb.Models;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
public unsafe struct XEvent
{
    [FieldOffset(0)] private XResponse _response;
    [FieldOffset(0)] private XEventType _eventType;


    public readonly XEventType Reply =>
         _response.GetResponseType() switch
         {
             XResponseType.Error => XEventType.Error,
             XResponseType.Event or XResponseType.Notify => _eventType,
             _ => throw new InvalidOperationException(),
         };

    public readonly unsafe ref T As<T>() where T : struct => 
        ref _response.As<T>();
}
