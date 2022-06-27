using GraphQL;
using GraphQL.Attachments;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Enable();
        VerifierSettings.AddScrubber(x => x.RemoveLineSuffix("boundary="));
        VerifierSettings.IgnoreMember<ExecutionResult>(result => result.Perf);
        VerifierSettings.IgnoreMember<ExecutionResult>(result => result.Document);
        VerifierSettings.IgnoreMember<ExecutionResult>(result => result.Operation);
        VerifierSettings.AddExtraSettings(serializerSettings =>
        {
            serializerSettings.Converters.Add(new AttachmentConverter());
            serializerSettings.Converters.Add(new QueryResultConverter());
        });
    }
}