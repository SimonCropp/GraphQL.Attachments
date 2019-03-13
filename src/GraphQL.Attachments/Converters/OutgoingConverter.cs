using System;
using Newtonsoft.Json;

namespace GraphQL.Attachments
{
    public class OutgoingConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var outgoing = (Outgoing) value;
            writer.WriteStartObject();
            writer.WritePropertyName("Headers");
            serializer.Serialize(writer, outgoing.Headers);
            writer.WritePropertyName("Value");
            var content = outgoing.ContentBuilder().GetAwaiter().GetResult();
            var result = content.ReadAsStringAsync().GetAwaiter().GetResult();
            serializer.Serialize(writer, result);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type type)
        {
            return typeof(Outgoing).IsAssignableFrom(type);
        }
    }
}