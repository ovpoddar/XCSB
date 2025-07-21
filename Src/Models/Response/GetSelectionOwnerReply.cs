﻿namespace Xcsb.Models.Response;

public readonly struct GetSelectionOwnerReply : IXBaseResponse
{
    public readonly byte ResponseType; // 1
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint Owner;

    public bool Verify()
    {
        return this.ResponseType == 1 && this._pad0 == 0 && this.Length == 0;
    }
}