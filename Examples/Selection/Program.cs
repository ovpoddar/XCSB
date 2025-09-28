using Xcsb;
using Xcsb.Models;

var x = XcsbClient.Initialized();
var root = x.HandshakeSuccessResponseBody.Screens[0].Root;
x.SetSelectionOwnerChecked(root, ATOM.Primary, 0);
var owner = x.GetSelectionOwner(ATOM.Primary);
Console.WriteLine($"{owner.Value.Owner} == {root} ");

var resultListInstalledColormaps = x.ListInstalledColormaps(x.NewId());
Console.WriteLine(string.Join(", ", resultListInstalledColormaps.Value.Colormap));
