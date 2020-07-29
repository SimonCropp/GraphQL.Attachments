using GraphQL;
using GraphQL.Attachments;
using VerifyTests;

public static class ModuleInitializer
{
    public static void Initialize()
    {
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