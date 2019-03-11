using System.Net.Http;
using Newtonsoft.Json;

namespace GraphQL.Attachments
{
    public static class GraphQlRequestAppender
    {
        public static void AddQueryAndVariables(this MultipartFormDataContent content, string query, object variables, string operationName)
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

        internal static string ToJson(object target)
        {
            if (target == null)
            {
                return "";
            }

            return JsonConvert.SerializeObject(target);
        }
    }
}