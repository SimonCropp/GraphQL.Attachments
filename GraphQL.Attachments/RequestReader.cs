using System;
using System.IO;
using System.Linq;
using GraphQL;
using GraphQL.Attachments;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

public class RequestReader
{
    public static void ReadRequestInformation(HttpRequest request, out string query, out Inputs inputs, out IncomingAttachments attachments, out string operation)
    {
        if (!request.Form.TryGetValue("query", out var queryValues))
        {
            throw new Exception("Expected to find a form value named 'query'.");
        }

        if (queryValues.Count != 1)
        {
            throw new Exception("Expected 'query' to have a single value.");
        }

        query = queryValues.ToString();

        if (request.Form.TryGetValue("variables", out var variablesValues))
        {
            if (variablesValues.Count != 1)
            {
                throw new Exception("Expected 'variables' to have a single value.");
            }

            var variables = JObject.Parse(variablesValues.ToString());
            inputs = variables.ToInputs();
        }
        else
        {
            inputs = new Inputs();
        }

        attachments = new IncomingAttachments(request.Form.Files.ToDictionary(x => x.FileName, x => (Func<Stream>) x.OpenReadStream));

        operation = null;
        if (request.Form.TryGetValue("operation", out var operationValues))
        {
            if (variablesValues.Count != 1)
            {
                throw new Exception("Expected 'operation' to have a single value.");
            }

            operation = operationValues.ToString();
        }
    }
}