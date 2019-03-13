using System;
using Newtonsoft.Json;

namespace GraphQL.Attachments
{
    public class OutgoingAttachmentsConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var attachments = (OutgoingAttachments) value;
            writer.WriteStartObject();
            writer.WritePropertyName("HasPendingAttachments");
            serializer.Serialize(writer, attachments.HasPendingAttachments);
            writer.WritePropertyName("Inner");
            serializer.Serialize(writer, attachments.Inner);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type type)
        {
            return typeof(IOutgoingAttachments).IsAssignableFrom(type);
        }
    }
}