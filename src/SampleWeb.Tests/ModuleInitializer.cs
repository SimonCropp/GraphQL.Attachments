public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.InitializePlugins();
        VerifierSettings.AddScrubber(_ => _.RemoveLineSuffix("boundary="));
        VerifierSettings.IgnoreMembers<ExecutionResult>(
            _ => _.Perf,
            _ => _.Document,
            _ => _.Operation);
        VerifierSettings.AddExtraSettings(serializer =>
        {
            var converters = serializer.Converters;
            converters.Add(new AttachmentConverter());
            converters.Add(new QueryResultConverter());
        });
    }
}