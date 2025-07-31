using Xcsb;

const int primaryAtom = 1;
var x = XcsbClient.Initialized();
var root = x.HandshakeSuccessResponseBody.Screens[0].Root;
x.SetSelectionOwnerChecked(root, primaryAtom, 0);
var owner = x.GetSelectionOwner(primaryAtom);
Console.WriteLine($"{owner.Owner} == {root} ");



var resultSetPointerMapping = x.SetPointerMapping([3, 2, 1]);
Console.WriteLine("resultSetPointerMapping");


var resultListInstalledColormaps = x.ListInstalledColormaps(root);
Console.WriteLine("resultListInstalledColormaps");