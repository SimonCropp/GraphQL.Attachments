using GraphQL.Attachments;
using VerifyXunit;
using Xunit;

[GlobalSetUp]
public static class GlobalSetup
{
    public static void Setup()
    {
        Global.ModifySerialization(settings =>
        {
            settings.AddExtraSettings(serializerSettings =>
            {
                serializerSettings.Converters.Add(new OutgoingConverter());
                serializerSettings.Converters.Add(new AttachmentStreamConverter());
                serializerSettings.Converters.Add(new OutgoingAttachmentsConverter());
            });
        });
    }
}