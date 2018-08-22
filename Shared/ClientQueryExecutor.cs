using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static class ClientQueryExecutor
{
    static string uri = "graphql";

    public static Task<HttpResponseMessage> ExecutePost(HttpClient client, string query = null, object variables = null, Action<HttpHeaders> headerAction = null)
    {
        query = CompressQuery(query);
        var body = new
        {
            query,
            variables
        };
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(ToJson(body), Encoding.UTF8, "application/json")
        };
        headerAction?.Invoke(request.Headers);
        return client.SendAsync(request);
    }

    public static Task<HttpResponseMessage> ExecuteGet(HttpClient client, string query = null, object variables = null, Action<HttpHeaders> headerAction = null)
    {
        var compressed = CompressQuery(query);
        var variablesString = ToJson(variables);
        var getUri = $"{uri}?query={compressed}&variables={variablesString}";
        var request = new HttpRequestMessage(HttpMethod.Get, getUri);
        headerAction?.Invoke(request.Headers);
        return client.SendAsync(request);
    }

    static string ToJson(object target)
    {
        if (target == null)
        {
            return "";
        }

        return JsonConvert.SerializeObject(target);
    }

    static string CompressQuery(string query)
    {
        if (query == null)
        {
            return "";
        }

        return Compress.Query(query);
    }
}