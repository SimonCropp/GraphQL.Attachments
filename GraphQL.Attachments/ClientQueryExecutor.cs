using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GraphQL.Attachments
{
    public class ClientQueryExecutor
    {
        HttpClient client;
        string uri;

        public ClientQueryExecutor(HttpClient client, string uri= "graphql")
        {
            Guard.AgainstNull(nameof(client), client);
            Guard.AgainstNullWhiteSpace(nameof(uri), uri);

            this.client = client;
            this.uri = uri;
        }

        public async Task<HttpResponseMessage> ExecutePost(string query, object variables = null, string operationName = null, Action<PostContext> action=null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);

            using (var content = new MultipartFormDataContent())
            {
                AddQueryAndVariables(content, query, variables, operationName);

                if (action != null)
                {
                    var postContext = new PostContext(content);
                    action.Invoke(postContext);
                    postContext.HeadersAction?.Invoke(content.Headers);
                }

                var httpResponseMessage = await client.PostAsync(uri, content, cancellation).ConfigureAwait(false);
                if (httpResponseMessage.Content.Headers.ContentType.MediaType == "multipart/form-data")
                {
                    httpResponseMessage.mul
                }
                var httpResponseHeaders = httpResponseMessage.Headers;
                return httpResponseMessage;
            }
        }

        public Task<HttpResponseMessage> ExecuteGet(string query, object variables = null, Action<HttpHeaders> headerAction = null)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            var compressed = Compress.Query(query);
            var variablesString = ToJson(variables);
            string getUri;
            if (variablesString == null)
            {
                getUri = $"{uri}?query={compressed}";
            }
            else
            {
                getUri = $"{uri}?query={compressed}&variables={variablesString}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, getUri);
            headerAction?.Invoke(request.Headers);
            return client.SendAsync(request);
        }

        static void AddQueryAndVariables(MultipartFormDataContent content, string query, object variables, string operationName)
        {
            content.Add(new StringContent(query), "query");

            if (operationName != null)
            {
                content.Add(new StringContent(operationName), "operationName");
            }

            if (variables != null)
            {
                content.Add(new StringContent(ToJson(variables)), "variables");
            }
        }

        static string ToJson(object target)
        {
            if (target == null)
            {
                return "";
            }

            return JsonConvert.SerializeObject(target);
        }
        public class PostContext
        {
            MultipartFormDataContent content;

            public PostContext(MultipartFormDataContent content)
            {
                Guard.AgainstNull(nameof(content), content);
                this.content = content;
            }

            public void SetHeadersAction(Action<HttpHeaders> headerAction)
            {
                Guard.AgainstNull(nameof(headerAction), headerAction);
                HeadersAction = headerAction;
            }

            public Action<HttpHeaders> HeadersAction { get; set; }

            public void AddAttachment(string name, Stream value)
            {
                Guard.AgainstNullWhiteSpace(nameof(name), name);
                Guard.AgainstNull(nameof(value), value);
                var file = new StreamContent(value);
                content.Add(file, name, name);
            }

            public void AddAttachment(string name, byte[] value)
            {
                Guard.AgainstNullWhiteSpace(nameof(name), name);
                Guard.AgainstNull(nameof(value), value);
                var file = new ByteArrayContent(value);
                content.Add(file, name, name);
            }

            public void AddAttachment(string name, string value)
            {
                Guard.AgainstNullWhiteSpace(nameof(name), name);
                Guard.AgainstNull(nameof(value), value);
                var file = new StringContent(value);
                content.Add(file, name, name);
            }
        }
    }

}