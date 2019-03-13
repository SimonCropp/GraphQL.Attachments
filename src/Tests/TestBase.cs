using GraphQL.Attachments;
using ObjectApproval;
using Xunit.Abstractions;

public class TestBase
{
    static TestBase()
    {
        SerializerBuilder.ExtraSettings= settings =>
        {
            settings.Converters.Add(new OutgoingConverter());
            settings.Converters.Add(new AttachmentStreamConverter());
            settings.Converters.Add(new OutgoingAttachmentsConverter());
        };
    }
    public TestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected readonly ITestOutputHelper Output;
}