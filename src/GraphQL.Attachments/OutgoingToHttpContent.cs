using System;
using System.Net.Http;
using System.Threading.Tasks;

static class OutgoingToHttpContent
{
   public static async Task<HttpContent> BuildContent(this Outgoing outgoing)
    {
        if (outgoing.AsyncStreamFactory != null)
        {
            var value = await outgoing.AsyncStreamFactory();
            return new StreamContent(value);
        }

        if (outgoing.StreamFactory != null)
        {
            return new StreamContent(outgoing.StreamFactory());
        }

        if (outgoing.StreamInstance != null)
        {
            return new StreamContent(outgoing.StreamInstance);
        }

        if (outgoing.AsyncBytesFactory != null)
        {
            var value = await outgoing.AsyncBytesFactory();
            return new ByteArrayContent(value);
        }

        if (outgoing.BytesFactory != null)
        {
            return new ByteArrayContent(outgoing.BytesFactory());
        }

        if (outgoing.BytesInstance != null)
        {
            return new ByteArrayContent(outgoing.BytesInstance);
        }

        if (outgoing.AsyncStringFactory != null)
        {
            var value = await outgoing.AsyncStringFactory();
            return new StringContent(value);
        }

        if (outgoing.StringFactory != null)
        {
            return new StringContent(outgoing.StringFactory());
        }

        if (outgoing.StringInstance != null)
        {
            return new StringContent(outgoing.StringInstance);
        }

        throw new Exception("No matching way to handle outgoing.");
    }
}