using System.Globalization;
using Argon;

namespace GraphQL.Attachments;

public class QueryResultConverter :
    JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var result = (QueryResult) value!;
        writer.WriteStartObject();
        writer.WritePropertyName("Status");
        serializer.Serialize(writer, result.Status);
        writer.WritePropertyName("ResultStream");
        var json = new StreamReader(result.Stream).ReadToEnd();
        writer.WriteValue(PrettyJson(json));
        if (result.ContentHeaders.Any())
        {
            writer.WritePropertyName("ContentHeaders");
            serializer.Serialize(writer, result.ContentHeaders);
        }

        if (result.Headers.Any())
        {
            writer.WritePropertyName("Headers");
            serializer.Serialize(writer, result.Headers);
        }

        writer.WritePropertyName("Attachments");
        serializer.Serialize(writer, result.Attachments);
        writer.WriteEndObject();
    }

    static string PrettyJson(string json)
    {
        if (json == string.Empty)
        {
            return string.Empty;
        }

        var token = JToken.Parse(json);
        using var stringWriter = new StringWriter(CultureInfo.InvariantCulture)
        {
            NewLine = "\n"
        };
        var textWriter = new JsonTextWriter(stringWriter);
        textWriter.Formatting = Formatting.Indented;

        token.WriteTo(textWriter);

        return stringWriter.ToString();
    }

    public override object ReadJson(JsonReader reader, Type type, object? value, JsonSerializer serializer) =>
        throw new NotImplementedException();

    public override bool CanConvert(Type type) =>
        type.IsAssignableTo(typeof(QueryResult));
}