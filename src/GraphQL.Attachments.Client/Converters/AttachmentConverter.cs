using System;
using System.IO;
using Newtonsoft.Json;

namespace GraphQL.Attachments
{
    public class AttachmentConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var attachment = (Attachment) value;
            writer.WriteStartObject();
            writer.WritePropertyName("Name");
            serializer.Serialize(writer, attachment.Name);
            writer.WritePropertyName("Metadata");
            serializer.Serialize(writer, attachment.Headers);
            writer.WritePropertyName("Value");
            serializer.Serialize(writer, new StreamReader(attachment.Stream).ReadToEnd());
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type type)
        {
            return typeof(Attachment).IsAssignableFrom(type);
        }
    }
}