using Argon;

namespace GraphQL.Attachments;

public class AttachmentStreamConverter :
    JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var attachment = (AttachmentStream) value;
        writer.WriteStartObject();
        writer.WritePropertyName("Name");
        serializer.Serialize(writer, attachment.Name);
        writer.WritePropertyName("Metadata");
        serializer.Serialize(writer, attachment.Metadata);
        writer.WritePropertyName("Value");
        serializer.Serialize(writer, new StreamReader(attachment).ReadToEnd());
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type type, object? value, JsonSerializer serializer) =>
        throw new NotImplementedException();

    public override bool CanConvert(Type type) =>
        typeof(AttachmentStream).IsAssignableFrom(type);
}