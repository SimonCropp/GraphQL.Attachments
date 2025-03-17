﻿public static class ModuleInitializer
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
        VerifierSettings.AddExtraSettings(serializer =>
        {
            var converters = serializer.Converters;
            converters.Add(new OutgoingConverter());
            converters.Add(new AttachmentStreamConverter());
            converters.Add(new OutgoingAttachmentsConverter());
        });
    }
}