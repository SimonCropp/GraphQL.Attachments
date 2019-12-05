namespace GraphQL.Attachments
{
    public static class UriBuilder
    {
        public static string GetUri(string root, string? variables, string compressed, string? operationName)
        {
            Guard.AgainstNullWhiteSpace(nameof(root), root);
            Guard.AgainstNullWhiteSpace(nameof(compressed), compressed);
            var getUri = $"{root}?query={compressed}";

            if (variables != null)
            {
                getUri += $"&variables={variables}";
            }

            if (operationName != null)
            {
                getUri += $"&operationName={operationName}";
            }

            return getUri;
        }
    }
}