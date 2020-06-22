using GraphQL.Attachments;
using VerifyTests;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        VerifierSettings.ModifySerialization(settings =>
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