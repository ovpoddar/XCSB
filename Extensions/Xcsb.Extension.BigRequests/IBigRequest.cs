using System;
using System.Collections.Generic;
using System.Text;
using Xcsb.Extension.BigRequests.Response;

namespace Xcsb.Extension.BigRequests;

public interface IBigRequest
{
    BigReqEnableReply BigRequestsEnable();
}
