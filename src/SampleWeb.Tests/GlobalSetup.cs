using GraphQL.Attachments;
using Verify;
using Xunit;

[GlobalSetUp]
public static class GlobalSetup
{
    public static void Setup()
    {
        SharedVerifySettings.AddScrubber(x => x.RemoveLineSuffix("boundary="));
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