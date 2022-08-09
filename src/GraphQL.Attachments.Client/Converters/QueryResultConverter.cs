using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        writer.WritePropertyName("ContentHeaders");
        serializer.Serialize(writer, result.ContentHeaders);
        writer.WritePropertyName("Attachments");
        serializer.Serialize(writer, result.Attachments);
        writer.WriteEndObject();
    }

    static string PrettyJson(string json)
    {
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
        typeof(QueryResult).IsAssignableFrom(type);
}