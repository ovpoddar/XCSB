using Xcsb;

using var f = Xcsb.XcsbClient.Initialized();
var c = f.Dismember();
c.Write();
f.Dispose();