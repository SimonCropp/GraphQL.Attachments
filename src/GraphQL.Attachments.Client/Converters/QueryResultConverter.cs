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
        writer.WriteRawValue(JToken.Parse(json).ToString());
        writer.WritePropertyName("ContentHeaders");
        serializer.Serialize(writer, result.ContentHeaders);
        writer.WritePropertyName("Attachments");
        serializer.Serialize(writer, result.Attachments);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type type, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type type)
    {
        return typeof(QueryResult).IsAssignableFrom(type);
    }
}