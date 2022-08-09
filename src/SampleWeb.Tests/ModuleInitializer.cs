using GraphQL;
using GraphQL.Attachments;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyDiffPlex.Initialize();
        VerifyHttp.Enable();
        VerifierSettings.AddScrubber(x => x.RemoveLineSuffix("boundary="));
        VerifierSettings.IgnoreMembers<ExecutionResult>(
            _ => _.Perf,
            _ => _.Document,
            _ => _.Operation);
        VerifierSettings.AddExtraSettings(serializerSettings =>
        {
            var converters = serializerSettings.Converters;
            converters.Add(new AttachmentConverter());
            converters.Add(new QueryResultConverter());
        });
    }
}