

using Xcsb;
using Xcsb.Extension.XInput;
using Xcsb.Extension.XInput.Models;

Console.WriteLine("Hello World!");
using var con = Xcsb.Connection.XcsbClient.Connect();
var main = con.Initialized();
var input = con.Extension.XInput();
using var items = con.Extension.ListExtensions();
foreach (var item in items.Reply.Names)
{
    Console.WriteLine(item);
}
items.Dispose();

// using var a = input.ChangeDeviceControl(DeviceControl.Enable, 0);
//
// Console.WriteLine(a.Reply.Status);
// a.Dispose();

using var b = main.GetPointerMapping();
Console.WriteLine(b.Reply.Reply);