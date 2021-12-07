using GraphQL;
using GraphQL.Attachments;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Enable();
        VerifierSettings.ModifySerialization(settings =>
        {
            settings.IgnoreMember<ExecutionResult>(result => result.Perf);
            settings.IgnoreMember<ExecutionResult>(result => result.Document);
            settings.IgnoreMember<ExecutionResult>(result => result.Operation);
            settings.IgnoreMember("SourceLocation");
            settings.AddExtraSettings(serializerSettings =>
            {
                serializerSettings.Converters.Add(new OutgoingConverter());
                serializerSettings.Converters.Add(new AttachmentStreamConverter());
                serializerSettings.Converters.Add(new OutgoingAttachmentsConverter());
            });
        });
    }
}