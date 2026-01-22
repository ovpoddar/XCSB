using System;
using Xcsb;
using Xcsb.Models;
using Xcsb.Response.Replies;

using var f = Xcsb.XcsbClient.Initialized(1);  // connection
var c = f.SetUpXcsb();
f.Dispose();