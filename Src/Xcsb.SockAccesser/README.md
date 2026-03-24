# Xcsb.SockAccesser

Low-level socket access extensions for `IXConnection`. Exposes internal socket operations for advanced protocol manipulation.

## Installation

Reference `Xcsb.SockAccesser` in your project. Requires `Xcsb.Connection`.

## Usage

```csharp
using Xcsb.SockAccesser;

// Send raw bytes (no sequence increment)
connection.SendData(rawBytes);

// Send request (increments sequence)
connection.SendRequest(requestBytes);

// Get current sequences
int sendSeq = connection.GetSendRequestSequence();
int recvSeq = connection.GetReceivedRequestSequence();
```

## API

| Method | Description |
|--------|-------------|
| `SendData(byte[])` | Send raw data without sequence tracking |
| `SendRequest(byte[])` | Send request with sequence increment |
| `GetSendRequestSequence()` | Returns current send sequence number |
| `GetReceivedRequestSequence()` | Returns current received sequence number |
