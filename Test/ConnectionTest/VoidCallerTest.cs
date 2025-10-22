using ConnectionTest.TestFunctionBuilder;
using System.Diagnostics;

namespace ConnectionTest;

public class VoidCallerTest
{
    [Theory]
    [InlineData("CreateWindow", true)]
    [InlineData("ChangeWindowAttributes", true)]
    [InlineData("DestroyWindow", true)]
    [InlineData("DestroySubwindows", true)]
    [InlineData("ChangeSaveSet", true)]
    [InlineData("ReparentWindow", true)]
    [InlineData("MapWindow", true)]
    [InlineData("MapSubwindows", true)]
    [InlineData("UnmapWindow", true)]
    [InlineData("UnmapSubwindows", true)]
    [InlineData("ConfigureWindow", true)]
    [InlineData("CirculateWindow", true)]
    [InlineData("ChangeProperty", true)]
    [InlineData("DeleteProperty", true)]
    [InlineData("RotateProperties", true)]
    [InlineData("SetSelectionOwner", true)]
    [InlineData("ConvertSelection", true)]
    [InlineData("SendEvent", true)]
    [InlineData("UngrabPointer", true)]
    [InlineData("GrabButton", true)]
    [InlineData("UngrabButton", true)]
    [InlineData("ChangeActivePointerGrab", true)]
    [InlineData("UngrabKeyboard", true)]
    [InlineData("GrabKey", true)]
    [InlineData("UngrabKey", true)]
    [InlineData("AllowEvents", true)]
    [InlineData("GrabServer", true)]
    [InlineData("UngrabServer", true)]
    [InlineData("WarpPointer", true)]
    [InlineData("SetInputFocus", true)]
    [InlineData("OpenFont", true)]
    [InlineData("CloseFont", true)]
    [InlineData("SetFontPath", true)]
    [InlineData("CreatePixmap", true)]
    [InlineData("FreePixmap", true)]
    [InlineData("CreateGC", true)]
    [InlineData("ChangeGC", true)]
    [InlineData("CopyGC", true)]
    [InlineData("SetDashes", true)]
    [InlineData("SetClipRectangles", true)]
    [InlineData("FreeGC", true)]
    [InlineData("ClearArea", true)]
    [InlineData("CopyArea", true)]
    [InlineData("CopyPlane", true)]
    [InlineData("PolyPoint", true)]
    [InlineData("PolyLine", true)]
    [InlineData("PolySegment", true)]
    [InlineData("PolyRectangle", true)]
    [InlineData("PolyArc", true)]
    [InlineData("FillPoly", true)]
    [InlineData("PolyFillRectangle", true)]
    [InlineData("PolyFillArc", true)]
    [InlineData("PutImage", true)]
    [InlineData("ImageText8", true)]
    [InlineData("ImageText16", true)]
    [InlineData("CreateColormap", true)]
    [InlineData("FreeColormap", true)]
    [InlineData("CopyColormapAndFree", true)]
    [InlineData("InstallColormap", true)]
    [InlineData("UninstallColormap", true)]
    [InlineData("FreeColors", true)]
    [InlineData("StoreColors", true)]
    [InlineData("StoreNamedColor", true)]
    [InlineData("CreateCursor", true)]
    [InlineData("CreateGlyphCursor", true)]
    [InlineData("FreeCursor", true)]
    [InlineData("RecolorCursor", true)]
    [InlineData("ChangeKeyboardMapping", true)]
    [InlineData("Bell", true)]
    [InlineData("ChangeKeyboardControl", true)]
    [InlineData("ChangePointerControl", true)]
    [InlineData("SetScreenSaver", true)]
    [InlineData("ForceScreenSaver", true)]
    [InlineData("ChangeHosts", true)]
    [InlineData("SetAccessControl", true)]
    [InlineData("SetCloseDownMode", true)]
    [InlineData("KillClient", true)]
    [InlineData("NoOperation", true)]
    [InlineData("PolyText8", true)]
    [InlineData("PolyText16", true)]
    public void Test1(string methodName, bool isVoidReturn, params int[] args)
    {
        if (!OperatingSystem.IsLinux())
            return;

        // arrange
        using var cProcessBuilder = new CFunctionBuilder();

        // act
        
        var cProcess = cProcessBuilder.GetApplicationProcess(methodName, isVoidReturn, args);
        cProcess.Start();

        // assert

        Assert.NotNull(methodName);
        Assert.True(isVoidReturn);
        Assert.Null(args);
        Assert.Null(cProcess.StandardError.ReadToEnd());
    }

}