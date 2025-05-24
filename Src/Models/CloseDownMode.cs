using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models;
public enum CloseDownMode :byte
{
    Destroy,
    RetainPermanent,
    RetainTemporary
}
