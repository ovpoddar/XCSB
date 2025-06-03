using System.Text;
using Xcsb;
using Xcsb.Models;

int screen_num;
var x = XcsbClient.Initialized();

// Get screen and root
var screen = x.HandshakeSuccessResponseBody.Screens[0];
var root = screen.Root;

// Create a basic window
var win = x.NewId();
x.CreateWindow(
    win,
    root,
    0, 0, 300, 200,
    1, Xcsb.Models.ClassType.InputOutput, screen.RootVisualId,
    0, []);
x.MapWindow(win);

// Create three properties

string[] names = ["A_PROP", "B_PROP", "C_PROP"];
var atoms = new uint[3];
for (var i = 0; i < 3; i++)
{
    var atom = x.InternAtom(false, names[i]);
    atoms[i] = atom.Atom;

    x.ChangeProperty<byte>(PropertyMode.Replace,
        win,
        atoms[i],
        31,
        Encoding.UTF8.GetBytes($"1{i}"));
}


Console.WriteLine("Rotating properties in 2 seconds...");
Thread.Sleep(2000);
// Rotate left: A → B, B → C, C → A
x.RotateProperties(win, 1, atoms);
Thread.Sleep(5000);
x.Dispose();
return 0;