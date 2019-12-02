using System;

namespace GraphQL.Attachments
{
    public class PostRequest
    {
        public string Query { get; }

        public PostRequest(string query)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            Query = query;
        }

        public Action<PostContext>? Action { get; set; }
        public object? Variables { get; set; }
        public string? OperationName { get; set; }
    }
}