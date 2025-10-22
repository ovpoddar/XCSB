using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionTest.TestFunctionBuilder;
internal class CSFunctionBuilder : BaseTestBuilder
{
    public override string GetWorkingFolder => base.GetWorkingFolder + "csdir";
    
    void GenerateTestXCB()
    {

    }

    public override Process GetApplicationProcess(string functionName, bool isVoidReturn, params int[] arguments)
    {

    }
}
