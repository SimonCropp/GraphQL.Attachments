using GraphQL;
using GraphQL.Attachments;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.InitializePlugins();
        VerifierSettings.IgnoreMembers<ExecutionResult>(
            _ => _.Perf,
            _ => _.Document,
            _ => _.Operation);
        VerifierSettings.IgnoreMembers("Location", "Length");
        VerifierSettings.AddExtraSettings(serializerSettings =>
        {
            var converters = serializerSettings.Converters;
            converters.Add(new OutgoingConverter());
            converters.Add(new AttachmentStreamConverter());
            converters.Add(new OutgoingAttachmentsConverter());
        });
    }
}