﻿using System;
using System.Net.Http.Headers;

namespace GraphQL.Attachments
{
    public class GetRequest
    {
        public string Query { get; }

        public GetRequest(string query)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            Query = query;
        }

        public Action<HttpRequestHeaders> HeadersAction { get; set; }
        public object Variables { get; set; }
        public string OperationName { get; set; }
    }
}