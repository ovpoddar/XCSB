using Xcsb;
using Xcsb.Connection;
using Xcsb.Connection.Response;

using var client = XcsbClient.Connect();
var con = client.Initialized();
const int iterations = 100000;

Console.WriteLine("Void");
Console.WriteLine("Test 1");
var grabSlots = new ResponseProto[iterations];
for (var i = 0; i < iterations; i++)
{
    grabSlots[i] = con.GrabServer();
    Console.Write("\r{0} out of {1}", i, iterations);
}
Console.Write("\r                                                   ");

Console.WriteLine("\rTest 2");
var unGrabSlots = new ResponseProto[iterations];
for (var i = 0; i < iterations; i++)
{
    unGrabSlots[i] = con.UngrabServer();
    Console.Write("\r{0}", i);
}
Console.Write("\r                                                   ");
Console.WriteLine("\rTest 3");

var error = 0;
for (var i = 0; i < iterations; i++)
{
    var result = con.CheckResponseProtoResult(grabSlots[i]);
    if (result.HasValue) error++;
    Console.Write("\r{0}", i);
}
Console.Write("\r                                                   ");
Console.WriteLine($"\rInfo: error exist {error}");

Console.WriteLine("Test 4");
error = 0;
for (int i = 0; i < iterations; i++)
{
    var result = con.CheckResponseProtoResult(unGrabSlots[i]);
    if (result.HasValue) error++;
    Console.Write("\r{0}", i);
}
Console.Write("\r                                                   ");
Console.WriteLine($"\rInfo: error exist {error}");

Console.WriteLine();
Console.WriteLine("Test 5");
for (int i = 0; i < iterations; i++)
{
    con.GrabServerChecked();
    Console.Write("\r{0}", i);
}
Console.Write("\r                                                   ");

Console.WriteLine("NonVoid");
for (int i = 0; i < iterations; i++)
{
    var x = con.GetInputFocus();
    Console.Write("\r{0}", i);
    x.Dispose();
}

Console.WriteLine("Success");