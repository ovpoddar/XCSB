# Xcsb

> **⚠️Warning**: Xcsb is currently in early development. APIs are subject to change, and not all functionality is implemented. Use in production environments is not recommended.

**Xcsb** is a generic, low-level .NET library designed to interface directly with the X11 Window System. It allows .NET developers to create cross-platform desktop applications that communicate natively with X11 servers, bypassing heavy C wrappers.

## Features

- **Direct Protocol Access**: Implements the X11 protocol directly in C#.
- **Strongly Typed**: Idiomatic .NET interfaces for identifying protocol errors at compile time.
- **Cross-Platform**: Compatible with Windows (via Xming/WSLg), Linux, and macOS (via XQuartz).
- **Zero-Dependency**: No native dependencies on `libX11` or `xcb`.

## Installation

Install via [NuGet](https://www.nuget.org/packages/Xcsb):

```shell
dotnet add package Xcsb
```

## 💡 Quick Start

Here is a minimal example of connecting to an X server and creating a basic window.

```csharp
using Xcsb;
using Xcsb.Connection;
using Xcsb.Masks;
using Xcsb.Models;

// 1. Connect to the X Server
using var connection = XcsbClient.Connect();
var xcsb = connection.Initialized();
var screen = connection.HandshakeSuccessResponseBody.Screens[0];

// 2. Create a Window ID
var windowId = connection.NewId();

// 3. Create the Window
xcsb.CreateWindowUnchecked(
    0,
    windowId,
    screen.Root,
    x: 0, y: 0, width: 800, height: 600,
    borderWidth: 0,
    ClassType.InputOutput,
    screen.RootVisualId,
    ValueMask.BackPixel | ValueMask.EventMask,
    [screen.WhitePixel, (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)]
);

// 4. Map (Show) the Window
xcsb.MapWindowUnchecked(windowId);

// 5. Basic Event Loop
while (true)
{
    var evnt = xcsb.GetEvent();
    if (evnt.ReplyType == XEventType.KeyPress)
    {
        break; // Exit on key press
    }
}
```

For more complex examples, such as drawing graphics or handling input, check the [Examples](./Examples) directory.

## 📋 Requirements

- **.NET 8.0** or later (supports .NET Standard 2.1)
- **X Server**:
  - **Linux**: Standard Xorg or Wayland (with XWayland).
  - **Windows**: [Xming](https://sourceforge.net/projects/xming/), VcXsrv, or WSLg.
  - **macOS**: [XQuartz](https://www.xquartz.org/).

## 🤝 Contributing

Contributions are welcome!

1.  **Fork** the repository.
2.  **Create** a feature branch (`git checkout -b feature/NewFeature`).
3.  **Commit** your changes (`git commit -m 'Add some NewFeature'`).
4.  **Push** to the branch (`git push origin feature/NewFeature`).
5.  **Open** a Pull Request.

Please review the [X11 Protocol Reference](https://www.x.org/releases/X11R7.7/doc/xproto/x11protocol.html) when implementing new features.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
