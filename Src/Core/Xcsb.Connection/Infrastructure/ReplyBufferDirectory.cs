using System.Collections.Concurrent;

namespace Xcsb.Connection.Infrastructure;

public class ReplyBufferDirectory<TValue>
{
    private ConcurrentDictionary<ushort, ConcurrentDictionary<uint, TValue>> dict =
        new ConcurrentDictionary<ushort, ConcurrentDictionary<uint, TValue>>();
    private ConcurrentDictionary<ushort, uint> lenCollect = new ConcurrentDictionary<ushort, uint>();

    public ReplyBufferDirectory()
    {
        lenCollect.TryAdd(0, 0);
    }

    public void Add(ushort key, TValue value)
    {
        if (!lenCollect.TryGetValue(key, out var index))
        {
            index = 0;
            lenCollect[key] = 0;
        }
        else
        {
            lenCollect[key] = ++index;
        }

        var dictTemp = dict.GetOrAdd(key, new ConcurrentDictionary<uint, TValue>());
        dictTemp.GetOrAdd(index, value);
    }

    public TValue? Get(uint key)
    {
        var index = (ushort)key;
        var lenIndex = (key) >> 16;

        if (!dict.TryGetValue(index, out var dicitem))
            return default;

        if (!dicitem.TryRemove(lenIndex, out var response))
            return default;
        if (dicitem.IsEmpty)
        {
            if (!dict.Remove(index, out _))
                throw new Exception("fail in len");
        }

        return response;
    }

    public void Clear()
    {
        dict.Clear();
        lenCollect.Clear();
    }

    public bool ContainKey(uint key)
    {
        var index = (ushort)key;
        var lenIndex = key >> 16;
        return dict.TryGetValue(index, out var dicItem) && dicItem.ContainsKey(lenIndex);
    }
}