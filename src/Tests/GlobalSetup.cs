using GraphQL.Attachments;
using Verify;
using Xunit;

[GlobalSetUp]
public static class GlobalSetup
{
    public static void Setup()
    {
        SharedVerifySettings.ModifySerialization(settings =>
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