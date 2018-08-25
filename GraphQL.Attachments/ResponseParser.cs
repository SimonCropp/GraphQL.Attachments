using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Attachments;

class ResponseParser
{
    public static async Task<QueryResult> EvaluateResponse(HttpResponseMessage response, CancellationToken cancellation)
    {
        var queryResult = new QueryResult(response);
        if (response.IsMultipart())
        {
            var multipart = await response.Content.ReadAsMultipartAsync(cancellation);
            foreach (var multipartContent in multipart.Contents)
            {
                var name = multipartContent.Headers.ContentDisposition.Name;
                if (name == null)
                {
                    queryResult.ResultStream = await multipartContent.ReadAsStreamAsync().ConfigureAwait(false);
                }
                else
                {
                    queryResult.Attachments[name] = await multipartContent.ReadAsStreamAsync().ConfigureAwait(false);
                }
            }

            if (queryResult.ResultStream == null)
            {
                throw new Exception("Expected the multipart response top contain a single un-named part which contains the graphql response data.");
            }
        }
        else
        {
            queryResult.ResultStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        return queryResult;
    }
}