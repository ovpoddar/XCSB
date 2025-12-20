using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xcsb;

namespace MethodRequestBuilder.Test;

public class HandShakeResponseTest
{
    private const string SetUpResponseCBody =
    """
        #include <xcb/xcb.h>
        #include <stdio.h>

        int main()
        {
            xcb_connection_t *connection = xcb_connect(NULL, NULL);
            if (xcb_connection_has_error(connection))
            {
                return -1;
            }
            const xcb_setup_t *setup = xcb_get_setup(connection);
            printf("{\n");
            printf("\t\"status\": %d,\n", setup->status);
            printf("\t\"protocol_major_version\": %d,\n", setup->protocol_major_version);
            printf("\t\"protocol_minor_version\": %d,\n", setup->protocol_minor_version);
            printf("\t\"length\": %d,\n", setup->length);
            printf("\t\"release_number\": %d,\n", setup->release_number);
            printf("\t\"resource_id_base\": %d,\n", setup->resource_id_base);
            printf("\t\"resource_id_mask\": %d,\n", setup->resource_id_mask);
            printf("\t\"motion_buffer_size\": %d,\n", setup->motion_buffer_size);
            printf("\t\"vendor_len\": %d,\n", setup->vendor_len);
            printf("\t\"maximum_request_length\": %d,\n", setup->maximum_request_length);
            printf("\t\"roots_len\": %d,\n", setup->roots_len);
            printf("\t\"pixmap_formats_len\": %d,\n", setup->pixmap_formats_len);
            printf("\t\"image_byte_order\": %d,\n", setup->image_byte_order);
            printf("\t\"bitmap_format_bit_order\": %d,\n", setup->bitmap_format_bit_order);
            printf("\t\"bitmap_format_scanline_unit\": %d,\n", setup->bitmap_format_scanline_unit);
            printf("\t\"bitmap_format_scanline_pad\": %d,\n", setup->bitmap_format_scanline_pad);
            printf("\t\"min_keycode\": %d,\n", setup->min_keycode);
            printf("\t\"max_keycode\": %d\n", setup->max_keycode);
            printf("}\n");
            return 0;
        }
        """;
    private const string ScreenResponseCBody =
    """
    #include <xcb/xcb.h>
    #include <stdio.h>

    int main()
    {
        xcb_connection_t *connection = xcb_connect(NULL, NULL);
        if (xcb_connection_has_error(connection))
        {
            fprintf(stderr, "Cannot open display\n");
            return 1;
        }
        const xcb_setup_t *setup = xcb_get_setup(connection);
        xcb_screen_iterator_t screen_iter = xcb_setup_roots_iterator(setup);
        xcb_screen_t *screen;
        printf("[\n");
        while (screen_iter.rem != 0)
        {
            screen = screen_iter.data;
            printf("\t{\n");
            printf("\t\t\"root\": %d,\n", screen->root);
            printf("\t\t\"default_colormap\": %d,\n", screen->default_colormap);
            printf("\t\t\"white_pixel\": %d,\n", screen->white_pixel);
            printf("\t\t\"black_pixel\": %d,\n", screen->black_pixel);
            printf("\t\t\"current_input_masks\": %d,\n", screen->current_input_masks);
            printf("\t\t\"width_in_pixels\": %d,\n", screen->width_in_pixels);
            printf("\t\t\"height_in_pixels\": %d,\n", screen->height_in_pixels);
            printf("\t\t\"width_in_millimeters\": %d,\n", screen->width_in_millimeters);
            printf("\t\t\"height_in_millimeters\": %d,\n", screen->height_in_millimeters);
            printf("\t\t\"min_installed_maps\": %d,\n", screen->min_installed_maps);
            printf("\t\t\"max_installed_maps\": %d,\n", screen->max_installed_maps);
            printf("\t\t\"root_visual\": %d,\n", screen->root_visual);
            printf("\t\t\"backing_stores\": %d,\n", screen->backing_stores);
            printf("\t\t\"save_unders\": %d,\n", screen->save_unders);
            printf("\t\t\"root_depth\": %d,\n", screen->root_depth);
            printf("\t\t\"allowed_depths_len\": %d\n", screen->allowed_depths_len);
            xcb_screen_next(&screen_iter);
            if (screen_iter.rem == 0)
                printf("\t}\n");
            else
                printf("\t},\n");
        }
        printf("]\n");
        return 0;
    }
    """;
    private const string FormatResponseCBody =
    """
    #include <xcb/xcb.h>
    #include <stdio.h>

    int main()
    {
        xcb_connection_t *connection = xcb_connect(NULL, NULL);
        if (xcb_connection_has_error(connection))
        {
            fprintf(stderr, "Cannot open display\n");
            return 1;
        }
        const xcb_setup_t *setup = xcb_get_setup(connection);
        xcb_format_iterator_t format_iter = xcb_setup_pixmap_formats_iterator(setup);
        xcb_format_t *format;
        printf("[\n");
        while (format_iter.rem != 0)
        {
            format = format_iter.data;
            printf("\t{\n");
            printf("\t\t\"depth\": %d,\n", format->depth);
            printf("\t\t\"bits_per_pixel\": %d,\n", format->bits_per_pixel);
            printf("\t\t\"scanline_pad\": %d\n", format->scanline_pad);
            xcb_format_next(&format_iter);
            if (format_iter.rem == 0)
                printf("\t}\n");
            else
                printf("\t},\n");
        }
        printf("]\n");
        return 0;
    }
    """;
    private const string DepthResponseCBody =
    """
    #include <xcb/xcb.h>
    #include <stdio.h>

    int main()
    {
        xcb_connection_t *connection = xcb_connect(NULL, NULL);
        if (xcb_connection_has_error(connection))
        {
            fprintf(stderr, "Cannot open display\n");
            return 1;
        }
        const xcb_setup_t *setup = xcb_get_setup(connection);
        xcb_screen_t *screen;
        xcb_depth_t *depth;
        printf("[\n");
        xcb_screen_iterator_t screen_iter = xcb_setup_roots_iterator(setup);
        while (screen_iter.rem != 0)
        {
            screen = screen_iter.data;
            xcb_depth_iterator_t depth_iter = xcb_screen_allowed_depths_iterator(screen);
            while (depth_iter.rem != 0)
            {
                depth = depth_iter.data;
                printf("{\n");
                printf("\t\"depth\": %d,\n", depth->depth);
                printf("\t\"visuals_len\": %d\n", depth->visuals_len);
                xcb_depth_next(&depth_iter);
                if (depth_iter.rem == 0)
                    printf("}\n");
                else
                    printf("},\n");
            }
            xcb_screen_next(&screen_iter);
            if (depth_iter.rem != 0)
                printf(",");
        }

        printf("]\n");
        return 0;
    }
    """;
    private const string VisualResponseCBody =
    """
    #include <xcb/xcb.h>
    #include <stdio.h>

    int main()
    {
        xcb_connection_t *connection = xcb_connect(NULL, NULL);
        if (xcb_connection_has_error(connection))
        {
            fprintf(stderr, "Cannot open display\n");
            return 1;
        }
        const xcb_setup_t *setup = xcb_get_setup(connection);
        xcb_screen_t *screen;
        xcb_depth_t *depth;
        xcb_visualtype_t *visual;
        printf("[\n");
        xcb_screen_iterator_t screen_iter = xcb_setup_roots_iterator(setup);
        while (screen_iter.rem != 0)
        {
            screen = screen_iter.data;
            xcb_depth_iterator_t depth_iter = xcb_screen_allowed_depths_iterator(screen);
            while (depth_iter.rem != 0)
            {
                depth = depth_iter.data;
                xcb_visualtype_iterator_t visual_iter = xcb_depth_visuals_iterator(depth);

                while (visual_iter.rem != 0)
                {
                    visual = visual_iter.data;

                    printf("{\n");
                    printf("\t\"visual_id\": %d,\n", visual->visual_id);
                    printf("\t\"_class\": %d,\n", visual->_class);
                    printf("\t\"bits_per_rgb_value\": %d,\n", visual->bits_per_rgb_value);
                    printf("\t\"colormap_entries\": %d,\n", visual->colormap_entries);
                    printf("\t\"red_mask\": %d,\n", visual->red_mask);
                    printf("\t\"green_mask\": %d,\n", visual->green_mask);
                    printf("\t\"blue_mask\": %d\n", visual->blue_mask);
                    xcb_visualtype_next(&visual_iter);
                    if (visual_iter.rem == 0)
                        printf("}\n");
                    else
                        printf("},\n");
                }

                xcb_depth_next(&depth_iter);

                if (depth_iter.rem != 0)
                    printf(",");
            }
            xcb_screen_next(&screen_iter);

            if (screen_iter.rem != 0)
                printf(",");
        }

        printf("]\n");
        return 0;
    }
    """;

    static string GetCCompiler()
    {
        string[] compilerCommands = ["gcc", "clang"];
        foreach (var command in compilerCommands)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = command == "cl" ? "" : "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0
                && !output.Contains("version", StringComparison.OrdinalIgnoreCase))
                continue;
            return command;
        }

        throw new Exception("Could not find any compiler to build c project");
    }
    static T? GetCResponse<T>(string content)
    {
        var compailer = GetCCompiler();
        var execFile = Path.Join(Environment.CurrentDirectory, "main");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = compailer,
                Arguments = $"-x c -o \"{execFile}\" - -lxcb",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.StandardInput.Write(content);
        process.StandardInput.Close();
        process.WaitForExit();

        Debug.Assert(process.ExitCode == 0);
        Debug.Assert(string.IsNullOrWhiteSpace(process.StandardError.ReadToEnd()));
        Debug.Assert(string.IsNullOrWhiteSpace(process.StandardOutput.ReadToEnd()));
        Debug.Assert(File.Exists(execFile));
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = execFile,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        var stream = process.StandardOutput;
        var sb = new StringBuilder();
        Span<char> current = stackalloc char[1];
        Span<char> pre = stackalloc char[1];
        while (!stream.EndOfStream)
        {
            stream.Read(current);
            if (pre[0] == ',' && current[0] == ',')
                continue;

            sb.Append(current);
            pre[0] = current[0];
        }
        File.Delete(execFile);
        return JsonSerializer.Deserialize<T>(sb.ToString());
    }

    [Fact]
    public void HandshakeResponseSetupCheck()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return;
        /// Arrange
        var xcb = GetCResponse<XCBSetupT>(SetUpResponseCBody); //Goes First
        using var xcsb = XcsbClient.Initialized();
        /// Act
        var csContent = xcsb.HandshakeSuccessResponseBody;
        /// Assert
        Assert.NotNull(xcb);
        Assert.Equal(xcb.ReleaseNumber, (int)csContent.ReleaseNumber);
        Assert.Equal(xcb.ResourceIdBase, (int)csContent.ResourceIDBase);
        Assert.Equal(xcb.ResourceIdMask, (int)csContent.ResourceIDMask);
        Assert.Equal(xcb.MotionBufferSize, (int)csContent.MotionBufferSize);
        Assert.Equal(xcb.VendorLen, csContent.VendorName.Length);
        Assert.Equal(xcb.MaximumRequestLength, csContent.MaxRequestLength);
        Assert.Equal(xcb.RootsLen, csContent.Screens.Length);
        Assert.Equal(xcb.PixmapFormatsLen, csContent.Formats.Length);
        Assert.Equal(xcb.ImageByteOrder, (int)csContent.ImageByteOrder);
        Assert.Equal(xcb.BitmapFormatBitOrder, (int)csContent.BitmapBitOrder);
        Assert.Equal(xcb.BitmapFormatScanlineUnit, csContent.BitmapScanLineUnit);
        Assert.Equal(xcb.BitmapFormatScanlinePad, csContent.BitmapScanLinePad);
        Assert.Equal(xcb.MinKeycode, csContent.MinKeyCode);
        Assert.Equal(xcb.MaxKeycode, csContent.MaxKeyCode);
    }

    [Fact]
    public void HandshakeResponseScreenCheck()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return;
        /// Arrange
        var xcb = GetCResponse<List<XCBScreenT>>(ScreenResponseCBody);
        using var xcsb = XcsbClient.Initialized();
        /// Act
        var screens = xcsb.HandshakeSuccessResponseBody.Screens;
        /// Assert
        if (xcb == null)
            Assert.Empty(screens);
        else
        {
            Assert.Equal(xcb.Count, screens.Length);
            for (int i = 0; i < xcb.Count; i++)
            {
                var csItem = screens[i];
                var cItem = xcb[i];
                Assert.Equal((int)csItem.Root, cItem.Root);
                Assert.Equal((int)csItem.DefaultColormap, cItem.DefaultColormap);
                Assert.Equal((int)csItem.WhitePixel, cItem.WhitePixel);
                Assert.Equal((int)csItem.BlackPixel, cItem.BlackPixel);
                Assert.Equal(csItem.InputMask, cItem.CurrentInputMasks);
                Assert.Equal(csItem.Width, cItem.WidthInPixels);
                Assert.Equal(csItem.Height, cItem.HeightInPixels);
                Assert.Equal(csItem.MWidth, cItem.WidthInMillimeters);
                Assert.Equal(csItem.MHeight, cItem.HeightInMillimeters);
                Assert.Equal(csItem.MinMaps, cItem.MinInstalledMaps);
                Assert.Equal(csItem.MaxMaps, cItem.MaxInstalledMaps);
                Assert.Equal((int)csItem.RootVisualId, cItem.RootVisual);
                Assert.Equal((int)csItem.BackingStore, cItem.BackingStores);
                Assert.Equal(csItem.SaveUnders, cItem.SaveUnders != 0);
                if (csItem.RootDepth == null)
                    Assert.Equal(0, cItem.RootDepth);
                else
                    Assert.Equal(csItem.RootDepth.DepthValue, cItem.RootDepth);
                Assert.Equal(csItem.Depths.Length, cItem.AllowedDepthsLen);
            }
        }
    }

    [Fact]
    public void HandshakeResponseFormat()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return;
        /// Arrange
        var xcb = GetCResponse<List<XCBFormatT>>(FormatResponseCBody);
        using var xcsb = XcsbClient.Initialized();
        /// Act
        var formats = xcsb.HandshakeSuccessResponseBody.Formats;
        /// Assert
        Assert.NotNull(xcb);
        Assert.Equal(xcb.Count, formats.Length);
        for (int i = 0; i < xcb.Count; i++)
        {
            var csItem = formats[i];
            var cItem = xcb[i];
            Assert.Equal(csItem.Depth, cItem.Depth);
            Assert.Equal(csItem.BitsPerPixel, cItem.BitsPerPixel);
            Assert.Equal(csItem.ScanLinePad, cItem.ScanlinePad);
        }
    }

    [Fact]
    public void HandshakeResponseDepth()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return;
        /// Arrange
        var xcb = GetCResponse<List<XCBDepthT>>(DepthResponseCBody);
        using var xcsb = XcsbClient.Initialized();
        /// Act
        var depthes = xcsb.HandshakeSuccessResponseBody.Screens.SelectMany(a => a.Depths).ToList();
        /// Assert
        Assert.NotNull(xcb);
        for (var i = 0; i < depthes.Count; i++)
        {
            var csItem = depthes[i];
            var cItem = xcb[i];

            Assert.Equal(csItem.DepthValue, cItem.Depth);
            Assert.Equal(csItem.Visuals.Length, cItem.VisualsLen);
        }
    }

    [Fact]
    public void HandshakeResponseVisual()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return;
        /// Arrange
        var xcb = GetCResponse<List<XCBVisualtypeT>>(VisualResponseCBody);
        using var xcsb = XcsbClient.Initialized();
        /// Act
        var depthes = xcsb.HandshakeSuccessResponseBody.Screens.SelectMany(a => a.Depths.SelectMany(a => a.Visuals)).ToList();
        /// Assert
        Assert.NotNull(xcb);
        Assert.Equal(depthes.Count, xcb.Count);
        foreach (var item in depthes)
        {
            var foundItem = xcb.FirstOrDefault(a => a.VisualId == item.VisualId
                && a.Class == (int)item.Class
                && a.BitsPerRgbValue == item.BitsPerRgb
                && a.ColormapEntries == item.MapEntries
                && a.RedMask == item.RedMask
                && a.GreenMask == item.GreenMask
                && a.BlueMask == item.BlueMask);
            Assert.NotNull(foundItem);
        }
    }
}

file class XCBSetupT
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("protocol_major_version")]
    public int ProtocolMajorVersion { get; set; }
    [JsonPropertyName("protocol_minor_version")]
    public int ProtocolMinorVersion { get; set; }
    [JsonPropertyName("length")]
    public int Length { get; set; }
    [JsonPropertyName("release_number")]
    public int ReleaseNumber { get; set; }
    [JsonPropertyName("resource_id_base")]
    public int ResourceIdBase { get; set; }
    [JsonPropertyName("resource_id_mask")]
    public int ResourceIdMask { get; set; }
    [JsonPropertyName("motion_buffer_size")]
    public int MotionBufferSize { get; set; }
    [JsonPropertyName("vendor_len")]
    public int VendorLen { get; set; }
    [JsonPropertyName("maximum_request_length")]
    public int MaximumRequestLength { get; set; }
    [JsonPropertyName("roots_len")]
    public int RootsLen { get; set; }
    [JsonPropertyName("pixmap_formats_len")]
    public int PixmapFormatsLen { get; set; }
    [JsonPropertyName("image_byte_order")]
    public int ImageByteOrder { get; set; }
    [JsonPropertyName("bitmap_format_bit_order")]
    public int BitmapFormatBitOrder { get; set; }
    [JsonPropertyName("bitmap_format_scanline_unit")]
    public int BitmapFormatScanlineUnit { get; set; }
    [JsonPropertyName("bitmap_format_scanline_pad")]
    public int BitmapFormatScanlinePad { get; set; }
    [JsonPropertyName("min_keycode")]
    public int MinKeycode { get; set; }
    [JsonPropertyName("max_keycode")]
    public int MaxKeycode { get; set; }
}

file class XCBScreenT
{
    [JsonPropertyName("root")]
    public int Root { get; set; }
    [JsonPropertyName("default_colormap")]
    public int DefaultColormap { get; set; }
    [JsonPropertyName("white_pixel")]
    public int WhitePixel { get; set; }
    [JsonPropertyName("black_pixel")]
    public int BlackPixel { get; set; }
    [JsonPropertyName("current_input_masks")]
    public int CurrentInputMasks { get; set; }
    [JsonPropertyName("width_in_pixels")]
    public int WidthInPixels { get; set; }
    [JsonPropertyName("height_in_pixels")]
    public int HeightInPixels { get; set; }
    [JsonPropertyName("width_in_millimeters")]
    public int WidthInMillimeters { get; set; }
    [JsonPropertyName("height_in_millimeters")]
    public int HeightInMillimeters { get; set; }
    [JsonPropertyName("min_installed_maps")]
    public int MinInstalledMaps { get; set; }
    [JsonPropertyName("max_installed_maps")]
    public int MaxInstalledMaps { get; set; }
    [JsonPropertyName("root_visual")]
    public int RootVisual { get; set; }
    [JsonPropertyName("backing_stores")]
    public int BackingStores { get; set; }
    [JsonPropertyName("save_unders")]
    public int SaveUnders { get; set; }
    [JsonPropertyName("root_depth")]
    public int RootDepth { get; set; }
    [JsonPropertyName("allowed_depths_len")]
    public int AllowedDepthsLen { get; set; }
}

file class XCBFormatT
{
    [JsonPropertyName("depth")]
    public int Depth { get; set; }
    [JsonPropertyName("bits_per_pixel")]
    public int BitsPerPixel { get; set; }
    [JsonPropertyName("scanline_pad")]
    public int ScanlinePad { get; set; }
}

file class XCBDepthT
{
    [JsonPropertyName("depth")]
    public int Depth { get; set; }
    [JsonPropertyName("visuals_len")]
    public int VisualsLen { get; set; }
}

file class XCBVisualtypeT
{
    [JsonPropertyName("visual_id")]
    public int VisualId { get; set; }
    [JsonPropertyName("_class")]
    public int Class { get; set; }
    [JsonPropertyName("bits_per_rgb_value")]
    public int BitsPerRgbValue { get; set; }
    [JsonPropertyName("colormap_entries")]
    public int ColormapEntries { get; set; }
    [JsonPropertyName("red_mask")]
    public int RedMask { get; set; }
    [JsonPropertyName("green_mask")]
    public int GreenMask { get; set; }
    [JsonPropertyName("blue_mask")]
    public int BlueMask { get; set; }
}