using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Outgoing
{
    public Func<Task<Stream>> AsyncStreamFactory;
    public Func<Stream> StreamFactory;
    public Stream StreamInstance;
    public Func<Task<byte[]>> AsyncBytesFactory;
    public Func<byte[]> BytesFactory;
    public byte[] BytesInstance;
    public Func<Task<string>> AsyncStringFactory;
    public Func<string> StringFactory;
    public string StringInstance;
    public Action Cleanup;
    public IReadOnlyDictionary<string, IEnumerable<string>> Headers;
}