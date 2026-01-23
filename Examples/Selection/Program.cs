using Xcsb;
using Xcsb.Models;

using var connection = XcsbClient.Connect();
var x = XcsbClient.Initialized(connection);
var root = connection.HandshakeSuccessResponseBody.Screens[0].Root;
x.SetSelectionOwnerChecked(root, ATOM.Primary, 0);
var owner = x.GetSelectionOwner(ATOM.Primary);
Console.WriteLine($"{owner.Owner} == {root} ");

var resultListInstalledColormaps = x.ListInstalledColormaps(connection.NewId());
Console.WriteLine(string.Join(", ", resultListInstalledColormaps.Colormap));
