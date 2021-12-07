using GraphQL;
using GraphQL.Attachments;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Enable();
        VerifierSettings.AddScrubber(x => x.RemoveLineSuffix("boundary="));
        VerifierSettings.ModifySerialization(settings =>
        {
            settings.IgnoreMember<ExecutionResult>(result => result.Perf);
            settings.IgnoreMember<ExecutionResult>(result => result.Document);
            settings.IgnoreMember<ExecutionResult>(result => result.Operation);
            settings.AddExtraSettings(serializerSettings =>
            {
                serializerSettings.Converters.Add(new AttachmentConverter());
                serializerSettings.Converters.Add(new QueryResultConverter());
            });
        });
    }
}