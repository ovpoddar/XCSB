# Xcsb.Connection

> **⚠️ Warning**: Xcsb is currently in early development. APIs are subject to change, and not all functionality is implemented. Use in production environments is not recommended.

## Overview

**Xcsb.Connection** is the core connection library for XCSB, providing low-level protocol implementation for communicating with XServers. This library handles all aspects of establishing and maintaining connections to X servers, including authentication, handshake negotiation, and socket management.

Unlike traditional X11 bindings that wrap `libX11` or `xcb`, Xcsb.Connection implements the X11 protocol directly in pure C#, enabling .NET developers to build cross-platform desktop applications without native dependencies.

## Features

- **Pure .NET Implementation**: Zero dependencies on native X11 libraries (`libX11`, `xcb`)
- **Direct Protocol Access**: Implements the X11 protocol specification directly in C#
- **Flexible Authentication**: Supports MIT-MAGIC-COOKIE-1 authentication via `.Xauthority` files
- **Multiple Connection Types**: TCP/IP and Unix domain socket support
- **Cross-Platform**: Works on Windows (via Xming/WSLg), Linux, and macOS (via XQuartz)
- **Strongly Typed**: Idiomatic .NET interfaces for compile-time safety
- **Modern .NET**: Targets .NET Standard 2.1, .NET 8.0, and .NET 9.0

## Installation

Install via [NuGet](https://www.nuget.org/packages/Xcsb):

```shell
dotnet add package Xcsb
```

Or add directly to your `.csproj` file:

```xml
<PackageReference Include="Xcsb" Version="1.0.0-beta5.1" />
```

## Quick Start

### Basic Connection

The simplest way to connect to an X server:

```csharp
using Xcsb.Connection;

// Connect to the default X server (reads from DISPLAY environment variable)
using var connection = XcsbClient.Connect();

// Check connection status
if (connection.HandshakeSuccessResponseBody != null)
{
    Console.WriteLine("Connected successfully!");
    var screen = connection.HandshakeSuccessResponseBody.Screens[0];
    Console.WriteLine($"Screen size: {screen.WidthInPixels}x{screen.HeightInPixels}");
}
else
{
    Console.WriteLine($"Connection failed: {connection.FailReason}");
}
```

### Specifying a Display

Connect to a specific X display:

```csharp
// Connect to display :1
using var connection = XcsbClient.Connect(":1");

// Connect to a remote X server
using var connection = XcsbClient.Connect("tcp/192.168.1.100:0");

// Connect via Unix socket
using var connection = XcsbClient.Connect("/tmp/.X11-unix/X0:0");
```

### Custom Authentication

Provide custom authentication credentials:

```csharp
byte[] authName = "MIT-MAGIC-COOKIE-1"u8.ToArray();
byte[] authData = GetAuthCookie(); // Your authentication cookie

using var connection = XcsbClient.Connect(":0", authName, authData);
```

### Creating a Window

Once connected, you can start creating windows and interacting with the X server:

```csharp
using Xcsb;
using Xcsb.Connection;
using Xcsb.Masks;
using Xcsb.Models;

// Connect to the X Server
using var connection = XcsbClient.Connect();
var xcsb = connection.Initialized();
var screen = connection.HandshakeSuccessResponseBody.Screens[0];

// Create a Window ID
var windowId = connection.NewId();

// Create the Window
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

// Map (Show) the Window
xcsb.MapWindowUnchecked(windowId);

// Basic Event Loop
while (true)
{
    var evnt = xcsb.GetEvent();
    if (evnt.ReplyType == XEventType.KeyPress)
    {
        break; // Exit on key press
    }
}
```

## Display String Format

The library supports various display string formats:

| Format | Description | Example |
|--------|-------------|---------|
| `:N` | Local display N (Unix socket) | `:0`, `:1` |
| `:N.S` | Local display N, screen S | `:0.0`, `:1.1` |
| `host:N` | TCP connection to host, display N | `localhost:0` |
| `tcp/host:N` | Explicit TCP connection | `tcp/192.168.1.100:0` |
| `/path:N` | Unix socket at specific path | `/tmp/.X11-unix/X0:0` |

## Authentication

### Automatic Authentication

By default, `XcsbClient.Connect()` automatically:
1. Looks for the `XAUTHORITY` environment variable
2. Falls back to `$HOME/.Xauthority`
3. Reads MIT-MAGIC-COOKIE-1 authentication data
4. Attempts connection with discovered credentials

### Manual Authentication

For custom authentication scenarios:

```csharp
byte[] authName = System.Text.Encoding.UTF8.GetBytes("MIT-MAGIC-COOKIE-1");
byte[] authData = /* your 16-byte cookie */;

using var connection = XcsbClient.Connect(":0", authName, authData);
```

## Configuration

Customize connection behavior with `XcsbClientConfiguration`:

```csharp
var config = new XcsbClientConfiguration
{
    ShouldCrashOnFailConnection = true // Throw exception on connection failure
};

using var connection = XcsbClient.Connect(":0", config);
```

## Requirements

- **.NET Standard 2.1**, **.NET 8.0**, or **.NET 9.0**
- **X Server**:
  - **Linux**: Standard Xorg or Wayland (with XWayland)
  - **Windows**: [Xming](https://sourceforge.net/projects/xming/), VcXsrv, or WSLg
  - **macOS**: [XQuartz](https://www.xquartz.org/)

## Error Handling

The library provides detailed error information:

```csharp
using var connection = XcsbClient.Connect(":0");

if (connection.HandshakeSuccessResponseBody == null)
{
    Console.WriteLine($"Connection failed: {connection.FailReason}");
    // Handle connection failure
}
```

Common failure reasons:
- **Authentication failed**: Invalid or missing `.Xauthority` credentials
- **Connection refused**: X server not running or not accepting connections
- **Display not found**: Invalid display number or socket path

### Accessing Screen Information

```csharp
var handshake = connection.HandshakeSuccessResponseBody;
if (handshake != null)
{
    foreach (var screen in handshake.Screens)
    {
        Console.WriteLine($"Screen: {screen.WidthInPixels}x{screen.HeightInPixels}");
        Console.WriteLine($"Root Window: {screen.Root}");
        Console.WriteLine($"Default Colormap: {screen.DefaultColormap}");
        Console.WriteLine($"Root Visual: {screen.RootVisualId}");
    }
}
```

## Examples

For complete examples demonstrating various features, check the [Examples](../../Examples) directory in the repository:

- Basic window creation
- Event handling
- Graphics operations
- Multi-screen applications

## Project Structure

```
Xcsb.Connection/
├── Configuration/          # Connection configuration options
├── Handlers/              # Socket access and communication handlers
├── Helpers/               # Utility classes for protocol operations
├── Infrastructure/        # Core connection and handshake logic
├── Models/                # Protocol data structures and models
│   └── Handshake/        # Handshake-specific models
├── IXConnection.cs        # Public connection interface
├── IXConnectionInternal.cs # Internal connection interface
└── XcsbClient.cs          # Main entry point
```

## Contributing

Contributions are welcome! When contributing to Xcsb.Connection:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/ConnectionFeature`)
3. **Commit** your changes (`git commit -m 'Add connection feature'`)
4. **Push** to the branch (`git push origin feature/ConnectionFeature`)
5. **Open** a Pull Request

Please reference the [X11 Protocol Specification](https://www.x.org/releases/X11R7.7/doc/xproto/x11protocol.html) when implementing protocol features.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Related Projects

- **Xcsb** - The main library containing X11 protocol operations
- **Xcsb.Models** - Protocol data structures and type definitions
- **Xcsb.Masks** - Event masks and attribute flags

## Resources

- [X11 Protocol Reference](https://www.x.org/releases/X11R7.7/doc/xproto/x11protocol.html)
- [X Window System Documentation](https://www.x.org/wiki/)
- [XCB Protocol Documentation](https://xcb.freedesktop.org/tutorial/)

## Support

For issues, questions, or contributions:
- **GitHub Issues**: [https://github.com/ovpoddar/XCSB/issues](https://github.com/ovpoddar/XCSB/issues)
- **Repository**: [https://github.com/ovpoddar/XCSB](https://github.com/ovpoddar/XCSB)
