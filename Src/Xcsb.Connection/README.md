# Xcsb

> ⚠️ **Warning:** Xcsb is currently in an unstable state. APIs and functionality may change without notice. Not all
> methods are implemented. Use in production environments is not recommended.

**Xcsb** is a .NET library designed to interface with the X11 Window System. It allows .NET developers to create
cross-platform desktop applications that communicate directly with X11 servers.

## Features

- Comprehensive X11 protocol support
- Asynchronous and synchronous API options
- Strongly-typed, idiomatic .NET interfaces
- Cross-platform compatibility (Windows, Linux, macOS via .NET)
- Minimal dependencies
- Actively maintained and open source

## Installation

Install via [NuGet](https://www.nuget.org/):

```shell
dotnet add package Xcsb
```

Or via the NuGet Package Manager:

```shell
Install-Package Xcsb
```

For more detailed documentation and advanced usage, see the [Wiki](https://github.com/ovpoddar/XCSB/wiki).

## Requirements

- dotnet
- **Windows users:** [Xming](https://sourceforge.net/projects/xming/) or any other alternatives.

## References

- [X11 Protocol Reference](https://www.x.org/releases/X11R7.7/doc/xproto/x11protocol.html)
- [xcb-proto Documentation](https://xcb.freedesktop.org/manual/)

## Contributing

Contributions are welcome! If you need a feature that is not yet implemented, add it yourself and add a PR. The Xcsb
project aims to be approachable for contributors of all levels.

> **Note:** Not all methods have example code associated with them. If you implement a new feature or method, please
> consider adding an example to help others.

To implement a missing feature:

1. Review the [X11 Protocol Reference](https://www.x.org/releases/X11R7.7/doc/xproto/x11protocol.html)
   and [xcb-proto Documentation](https://xcb.freedesktop.org/manual/) to understand the protocol details. or if you have
   experience you can skip this.
2. Fork the repository and create a new branch for your feature.
3. Add or modify code following the project's style and structure.
4. Write tests if possible, or write an example to demonstrate it. put at least one.
5. Submit a pull request describing your changes and referencing the relevant protocol documentation.

If you have questions or need guidance, please open an issue or start a discussion
on [GitHub](https://github.com/ovpoddar/XCSB).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
