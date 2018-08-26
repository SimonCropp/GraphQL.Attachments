﻿using System;
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

        public async Task<QueryResult> ExecutePost(string query, object variables = null, string operationName = null, HttpContentHeaders headers = null, Action<PostContext> attachmentAppender = null, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            var content = new MultipartFormDataContent();
            AddQueryAndVariables(content, query, variables, operationName);

            content.Headers.MergeHeaders(headers);

            if (attachmentAppender != null)
            {
                var postContext = new PostContext(content);
                attachmentAppender.Invoke(postContext);
            }

            var response = await client.PostAsync(uri, content, cancellation).ConfigureAwait(false);

            return await ResponseParser.EvaluateResponse(response, cancellation);
        }

        public async Task<QueryResult> ExecuteGet(string query, object variables = null, HttpRequestHeaders headers = null, CancellationToken cancellation = default)
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
            request.Headers.MergeHeaders(headers);
            var response = await client.SendAsync(request, cancellation).ConfigureAwait(false);
            return await ResponseParser.EvaluateResponse(response, cancellation).ConfigureAwait(false);
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
    }
}