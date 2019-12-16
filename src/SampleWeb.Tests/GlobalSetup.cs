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
                serializerSettings.Converters.Add(new AttachmentConverter());
                serializerSettings.Converters.Add(new QueryResultConverter());
            });
        });
    }
}