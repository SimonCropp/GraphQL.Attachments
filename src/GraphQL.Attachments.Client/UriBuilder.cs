namespace GraphQL.Attachments
{
    public static class UriBuilder
    {
        public static string GetUri(string root, string? variablesString, string compressed, string? operationName)
        {
            var getUri = $"{root}?query={compressed}";

            if (variablesString != null)
            {
                getUri += $"&variables={variablesString}";
            }

            if (operationName != null)
            {
                getUri += $"&operationName={operationName}";
            }

            return getUri;
        }
    }
}