# Xcsb

> **⚠️ Warning**: Xcsb is currently in early development. APIs are subject to change, and not all functionality is implemented. Use in production environments is not recommended.

## Overview

**Xcsb** is a comprehensive .NET library that provides a complete implementation of the X11 protocol, enabling developers to create cross-platform desktop applications that communicate directly with X servers. Built on top of [Xcsb.Connection](../Xcsb.Connection), this library offers high-level, strongly-typed methods for performing all standard X11 operations including window management, graphics rendering, event handling, and resource management.

Unlike traditional X11 bindings that rely on native C libraries like `libX11` or `xcb`, Xcsb implements the entire X11 protocol in pure C#, providing a truly native .NET experience without any external dependencies.

## Features

- **Complete X11 Protocol Support**: Implements the full X11 core protocol specification
- **Strongly-Typed API**: Idiomatic .NET interfaces with compile-time type safety
- **Comprehensive Operations**:
  - Window creation, configuration, and management
  - Graphics rendering (shapes, lines, arcs, images, text)
  - Event handling (keyboard, mouse, window events)
  - Resource management (colormaps, fonts, pixmaps, cursors)
  - Property and atom management
  - Input grabbing and focus control
  - Screen saver and access control
- **Flexible Request Modes**: Synchronous, checked, and unchecked request variants
- **Event System**: Complete event loop support with all X11 event types
- **Cross-Platform**: Works on Windows (via Xming/WSLg), Linux, and macOS (via XQuartz)
- **Zero Native Dependencies**: Pure .NET implementation
- **Modern .NET**: Targets .NET Standard 2.1, .NET 8.0, and .NET 9.0

## Installation

Install via [NuGet](https://www.nuget.org/packages/Xcsb):

```shell
dotnet add package Xcsb
```

Or via the NuGet Package Manager:

```shell
Install-Package Xcsb
```

## Quick Start

### Basic Window Creation

Create a simple window and handle events:

```csharp
using Xcsb;
using Xcsb.Connection;
using Xcsb.Masks;
using Xcsb.Models;

// Connect to X server and initialize protocol
using var connection = XcsbClient.Connect();
var xcsb = connection.Initialized();
var screen = connection.HandshakeSuccessResponseBody.Screens[0];

// Create a window
var windowId = connection.NewId();
xcsb.CreateWindowUnchecked(
    screen.RootDepth.DepthValue,
    windowId,
    screen.Root,
    x: 0, y: 0, width: 800, height: 600,
    borderWidth: 10,
    ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

// Set window title
xcsb.ChangePropertyUnchecked<byte>(
    PropertyMode.Replace,
    windowId,
    ATOM.WmName,
    ATOM.String,
    System.Text.Encoding.UTF8.GetBytes("My X11 Window")
);

// Show the window
xcsb.MapWindowUnchecked(windowId);

// Event loop
while (true)
{
    var evnt = xcsb.GetEvent();
    
    if (evnt.ReplyType == XEventType.LastEvent)
        break; // Connection closed
    
    if (evnt.ReplyType == XEventType.KeyPress)
        break; // Exit on key press
    
    if (evnt.ReplyType == XEventType.Expose)
    {
        // Handle window exposure (redraw)
        Console.WriteLine("Window exposed - redraw needed");
    }
}
```

### Drawing Graphics

Draw shapes, lines, and images:

```csharp
using Xcsb;
using Xcsb.Connection;
using Xcsb.Masks;
using Xcsb.Models;

using var connection = XcsbClient.Connect();
var xcsb = connection.Initialized();
var screen = connection.HandshakeSuccessResponseBody.Screens[0];

// Create window
var windowId = connection.NewId();
xcsb.CreateWindowUnchecked(
    screen.RootDepth.DepthValue,
    windowId,
    screen.Root,
    0, 0, 500, 400,
    10,
    ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackgroundPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

// Create graphics context
var gc = connection.NewId();
xcsb.CreateGCUnchecked(
    gc,
    windowId,
    GCMask.Foreground | GCMask.GraphicsExposures,
    [screen.BlackPixel, 0]
);

xcsb.MapWindowUnchecked(windowId);

while (true)
{
    var evnt = xcsb.GetEvent();
    if (evnt.ReplyType == XEventType.LastEvent) break;
    if (evnt.ReplyType == XEventType.KeyPress) break;
    
    if (evnt.ReplyType == XEventType.Expose)
    {
        // Draw rectangles
        xcsb.PolyRectangleUnchecked(windowId, gc, [
            new Rectangle { X = 10, Y = 10, Width = 100, Height = 80 },
            new Rectangle { X = 150, Y = 10, Width = 100, Height = 80 }
        ]);
        
        // Draw filled rectangles
        xcsb.PolyFillRectangleUnchecked(windowId, gc, [
            new Rectangle { X = 10, Y = 120, Width = 100, Height = 80 },
            new Rectangle { X = 150, Y = 120, Width = 100, Height = 80 }
        ]);
        
        // Draw arcs/circles
        xcsb.PolyArcUnchecked(windowId, gc, [
            new Arc { X = 20, Y = 220, Width = 60, Height = 60, Angle1 = 0, Angle2 = 360 * 64 }
        ]);
        
        // Draw lines
        xcsb.PolyLineUnchecked(CoordinateMode.Origin, windowId, gc, [
            new Point { X = 10, Y = 300 },
            new Point { X = 250, Y = 300 }
        ]);
    }
}
```

### Working with Images

Put and get pixel data:

```csharp
const int WIDTH = 100;
const int HEIGHT = 100;

// Create image data (RGBA format)
var imageData = new byte[WIDTH * HEIGHT * 4];
for (var y = 0; y < HEIGHT; y++)
{
    for (var x = 0; x < WIDTH; x++)
    {
        var index = (y * WIDTH + x) * 4;
        imageData[index + 0] = (byte)(x * 255 / WIDTH);      // Red
        imageData[index + 1] = (byte)(y * 255 / HEIGHT);     // Green
        imageData[index + 2] = (byte)((x + y) * 255 / (WIDTH + HEIGHT)); // Blue
        imageData[index + 3] = 0;                             // Alpha
    }
}

// Put image to window
xcsb.PutImageUnchecked(
    ImageFormatBitmap.ZPixmap,
    windowId,
    gc,
    WIDTH,
    HEIGHT,
    0, 0,  // destination x, y
    0,     // left pad
    screen.RootDepth.DepthValue,
    imageData
);

// Get image from window
var retrievedImage = xcsb.GetImage(
    ImageFormat.ZPixmap,
    windowId,
    0, 0,  // source x, y
    WIDTH,
    HEIGHT,
    uint.MaxValue  // plane mask
);
Console.WriteLine($"Retrieved {retrievedImage.Data.Length} bytes");
```

### Event Handling

Handle various X11 events:

```csharp
while (true)
{
    var evnt = xcsb.GetEvent();
    
    switch (evnt.ReplyType)
    {
        case XEventType.LastEvent:
            return; // Connection closed
            
        case XEventType.KeyPress:
            Console.WriteLine("Key pressed");
            break;
            
        case XEventType.KeyRelease:
            Console.WriteLine("Key released");
            break;
            
        case XEventType.ButtonPress:
            Console.WriteLine("Mouse button pressed");
            break;
            
        case XEventType.ButtonRelease:
            Console.WriteLine("Mouse button released");
            break;
            
        case XEventType.MotionNotify:
            Console.WriteLine("Mouse moved");
            break;
            
        case XEventType.EnterNotify:
            Console.WriteLine("Mouse entered window");
            break;
            
        case XEventType.LeaveNotify:
            Console.WriteLine("Mouse left window");
            break;
            
        case XEventType.Expose:
            Console.WriteLine("Window needs redraw");
            break;
            
        case XEventType.ConfigureNotify:
            Console.WriteLine("Window configuration changed");
            break;
            
        case XEventType.MapNotify:
            Console.WriteLine("Window mapped");
            break;
            
        case XEventType.UnmapNotify:
            Console.WriteLine("Window unmapped");
            break;
    }
    
    // Check for errors
    if (evnt.Error.HasValue)
    {
        Console.WriteLine($"X11 Error: {evnt.Error.Value.ResponseHeader.Reply}");
    }
}
```

### Working with Atoms and Properties

Query and manipulate window properties:

```csharp
// List all properties on root window
var rootProperties = xcsb.ListProperties(screen.Root);
foreach (var atom in rootProperties.Atoms)
{
    var atomName = xcsb.GetAtomName(atom);
    Console.WriteLine($"Property: {atomName.Name}");
}

// Intern a custom atom
var customAtom = xcsb.InternAtom(
    onlyIfExists: false,
    System.Text.Encoding.UTF8.GetBytes("MY_CUSTOM_PROPERTY")
);

// Set a property
xcsb.ChangePropertyUnchecked<uint>(
    PropertyMode.Replace,
    windowId,
    customAtom.Atom,
    ATOM.Cardinal,
    [12345]
);

// Get a property
var property = xcsb.GetProperty(
    delete: false,
    windowId,
    customAtom.Atom,
    ATOM.Cardinal,
    offset: 0,
    length: 1
);
```

### Querying Extensions

Check available X11 extensions:

```csharp
// List all available extensions
var extensions = xcsb.ListExtensions();
Console.WriteLine("Available extensions:");
foreach (var ext in extensions.Names)
{
    Console.WriteLine($"  - {ext}");
}

// Query specific extension
var extensionInfo = xcsb.QueryExtension(
    System.Text.Encoding.UTF8.GetBytes("RENDER")
);
if (extensionInfo.Present)
{
    Console.WriteLine($"RENDER extension present");
    Console.WriteLine($"  Major opcode: {extensionInfo.MajorOpcode}");
    Console.WriteLine($"  First event: {extensionInfo.FirstEvent}");
    Console.WriteLine($"  First error: {extensionInfo.FirstError}");
}
```

## API Structure

### Core Interfaces

#### `IXProto`
The main interface providing access to all X11 protocol operations. Obtained by calling `connection.Initialized()`.

Key methods include:
- **Window Management**: `CreateWindow`, `MapWindow`, `UnmapWindow`, `DestroyWindow`, `ConfigureWindow`
- **Graphics**: `CreateGC`, `PolyLine`, `PolyRectangle`, `PolyArc`, `FillPoly`, `PutImage`, `GetImage`
- **Events**: `GetEvent`, `IsEventAvailable`, `WaitForEvent`
- **Resources**: `CreatePixmap`, `CreateColormap`, `CreateCursor`, `OpenFont`, `CloseFont`
- **Properties**: `GetProperty`, `ChangeProperty`, `DeleteProperty`, `ListProperties`
- **Atoms**: `InternAtom`, `GetAtomName`
- **Input**: `GrabPointer`, `GrabKeyboard`, `GrabButton`, `GrabKey`, `SetInputFocus`
- **Queries**: `QueryTree`, `QueryPointer`, `GetGeometry`, `GetWindowAttributes`

### Request Variants

Most operations come in three variants:

1. **Synchronous** (e.g., `CreateWindow`): Waits for server confirmation
2. **Checked** (e.g., `CreateWindowChecked`): Returns immediately but allows error checking
3. **Unchecked** (e.g., `CreateWindowUnchecked`): Fire-and-forget, no error checking

```csharp
// Synchronous - blocks until complete
xcsb.CreateWindow(...);

// Checked - returns immediately, can check for errors later
xcsb.CreateWindowChecked(...);

// Unchecked - fastest, no error checking
xcsb.CreateWindowUnchecked(...);
```

### Event Types

The library supports all X11 event types via the `XEvent` structure:

- **Keyboard**: `KeyPress`, `KeyRelease`
- **Mouse**: `ButtonPress`, `ButtonRelease`, `MotionNotify`
- **Window**: `Expose`, `MapNotify`, `UnmapNotify`, `ConfigureNotify`, `DestroyNotify`
- **Focus**: `FocusIn`, `FocusOut`, `EnterNotify`, `LeaveNotify`
- **Property**: `PropertyNotify`
- **Selection**: `SelectionClear`, `SelectionRequest`, `SelectionNotify`
- **Colormap**: `ColormapNotify`
- **Client**: `ClientMessage`
- **Mapping**: `MappingNotify`

### Models and Masks

The library provides strongly-typed models for all X11 data structures:

- **Geometry**: `Point`, `Rectangle`, `Arc`, `Segment`
- **Color**: `ColorItem`, `Pixel`
- **Text**: `CharInfo`, `FontProp`
- **Masks**: `EventMask`, `ValueMask`, `GCMask`, `ConfigureWindowMask`
- **Enums**: `ClassType`, `MapState`, `GrabMode`, `ImageFormat`, `PropertyMode`

## Requirements

- **.NET Standard 2.1**, **.NET 8.0**, or **.NET 9.0**
- **X Server**:
  - **Linux**: Standard Xorg or Wayland (with XWayland)
  - **Windows**: [Xming](https://sourceforge.net/projects/xming/), VcXsrv, or WSLg
  - **macOS**: [XQuartz](https://www.xquartz.org/)

## Project Structure

```
Xcsb/
├── GenericExtension.cs    # Extension methods for IXConnection
├── Infrastructure/        # Core protocol interfaces
│   ├── IXProto.cs        # Main protocol interface
│   ├── IXBufferProto.cs  # Buffered operations
│   ├── ResponseProto/    # Response handling
│   └── VoidProto/        # Void (no-reply) operations
├── Implementation/        # Protocol implementation
│   └── XProto.cs         # Main implementation class
├── Requests/             # Request type definitions (122 types)
├── Response/             # Response handling
│   ├── Contract/         # Response contracts
│   ├── Errors/           # X11 error types
│   ├── Event/            # Event types (47 types)
│   └── Replies/          # Reply types (61 types)
├── Models/               # Data structures and enums
├── Masks/                # Bitmask definitions
└── Handlers/             # Low-level protocol handlers
```

## Examples

The [Examples](../../Examples) directory contains complete working examples:

- **Images**: Image rendering and manipulation
- **TextVisualizationAndFonts**: Text rendering with fonts
- **Grabbing**: Keyboard and mouse input grabbing
- **Selection**: Clipboard and selection handling
- **LifeCycleOfColorMap**: Colormap management
- **FocusChange**: Window focus handling
- **Clipping**: Graphics clipping regions
- **Transparent**: Transparency and compositing
- **RotatingValues**: Property rotation

## Advanced Topics

### Buffered Operations

For performance-critical scenarios, use buffered operations:

```csharp
var bufferClient = xcsb.BufferClient;
// Batch multiple operations
// Flush when ready
```

### Error Handling

Handle X11 protocol errors:

```csharp
var evnt = xcsb.GetEvent();
if (evnt.Error.HasValue)
{
    var error = evnt.Error.Value;
    Console.WriteLine($"Error code: {error.ResponseHeader.Reply}");
    Console.WriteLine($"Error sequence: {error.ResponseHeader.Sequence}");
}
```

### Resource Management

Always clean up X11 resources:

```csharp
// Create resources
var pixmap = connection.NewId();
xcsb.CreatePixmapUnchecked(...);

var gc = connection.NewId();
xcsb.CreateGCUnchecked(...);

// Use resources...

// Clean up
xcsb.FreePixmapUnchecked(pixmap);
xcsb.FreeGCUnchecked(gc);
xcsb.DestroyWindowUnchecked(windowId);
```

## Contributing

Contributions are welcome! The Xcsb project aims to be approachable for contributors of all levels.

> **Note**: Not all X11 protocol methods have example code. If you implement or use a feature, please consider adding an example to help others.

### To Implement a Missing Feature:

1. Review the [X11 Protocol Reference](https://www.x.org/releases/X11R7.7/doc/xproto/x11protocol.html) and [xcb-proto Documentation](https://xcb.freedesktop.org/manual/) to understand the protocol details
2. Fork the repository and create a new branch for your feature
3. Add or modify code following the project's style and structure
4. Write tests if possible, or create an example demonstrating the feature
5. Submit a pull request describing your changes and referencing the relevant protocol documentation

If you have questions or need guidance, please open an issue or start a discussion on [GitHub](https://github.com/ovpoddar/XCSB).

## Resources

- [X11 Protocol Reference](https://www.x.org/releases/X11R7.7/doc/xproto/x11protocol.html)
- [xcb-proto Documentation](https://xcb.freedesktop.org/manual/)
- [X Window System Documentation](https://www.x.org/wiki/)
- [Xlib Programming Manual](https://tronche.com/gui/x/xlib/)

## Related Projects

- **[Xcsb.Connection](../Xcsb.Connection)** - Low-level X server connection library
- **Xcsb.Models** - Protocol data structures (included in this package)
- **Xcsb.Masks** - Event masks and attribute flags (included in this package)

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Support

For issues, questions, or contributions:
- **GitHub Issues**: [https://github.com/ovpoddar/XCSB/issues](https://github.com/ovpoddar/XCSB/issues)
- **Repository**: [https://github.com/ovpoddar/XCSB](https://github.com/ovpoddar/XCSB)
