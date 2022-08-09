using Newtonsoft.Json;

namespace GraphQL.Attachments;

public class AttachmentConverter :
    JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var attachment = (Attachment) value!;
        writer.WriteStartObject();
        writer.WritePropertyName("Name");
        serializer.Serialize(writer, attachment.Name);
        writer.WritePropertyName("Metadata");
        serializer.Serialize(writer, attachment.Headers);
        writer.WritePropertyName("Value");
        using (var reader = new StreamReader(attachment.Stream))
        {
            serializer.Serialize(writer, reader.ReadToEnd());
        }

        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type type, object? value, JsonSerializer serializer) =>
        throw new NotImplementedException();

    public override bool CanConvert(Type type) =>
        typeof(Attachment).IsAssignableFrom(type);
}