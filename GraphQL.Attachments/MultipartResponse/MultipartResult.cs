using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public class MultipartResult : Collection<MultipartItem>, IActionResult
{
    MultipartContent content;

    public MultipartResult(string subtype = "byteranges", string boundary = null)
    {
        if (boundary == null)
        {
            content = new MultipartContent(subtype);
        }
        else
        {
            content = new MultipartContent(subtype, boundary);
        }
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        foreach (var item in this)
        {
            if (item.Stream != null)
            {
                var streamContent = new StreamContent(item.Stream);

                if (item.ContentType != null)
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(item.ContentType);
                }

                if (item.FileName != null)
                {
                    var contentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = item.FileName
                    };
                    streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = contentDisposition.FileName,
                        FileNameStar = contentDisposition.FileNameStar
                    };
                }

                content.Add(streamContent);
            }
        }

        var response = context.HttpContext.Response;
        response.ContentLength = content.Headers.ContentLength;
        response.ContentType = content.Headers.ContentType.ToString();

        await content.CopyToAsync(response.Body);
    }
}