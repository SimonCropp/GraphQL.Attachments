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
                serializerSettings.Converters.Add(new AttachmentConverter());
                serializerSettings.Converters.Add(new QueryResultConverter());
            });
        });
    }
}