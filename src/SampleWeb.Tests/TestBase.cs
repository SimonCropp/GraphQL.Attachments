using ObjectApproval;
using Xunit.Abstractions;

public class TestBase
{
    static TestBase()
    {
        SerializerBuilder.ExtraSettings = settings =>
        {
            settings.Converters.Add(new AttachmentConverter());
            settings.Converters.Add(new QueryResultConverter());
        };
    }

    public TestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected readonly ITestOutputHelper Output;
}