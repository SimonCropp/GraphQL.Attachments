[TestFixture]
public class IncomingAttachmentsTests
{
    [Test]
    public void GetValue_Single_Returns()
    {
        var attachments = new IncomingAttachments();
        var stream = new AttachmentStream("key", new MemoryStream(), 0, new HeaderDictionary());
        attachments.Add("key", stream);

        Assert.That(attachments.GetValue(), Is.SameAs(stream));
    }

    [Test]
    public void GetValue_Empty_Throws()
    {
        var attachments = new IncomingAttachments();

        var exception = Assert.Throws<Exception>(() => attachments.GetValue())!;
        Assert.That(exception.Message, Does.Contain("none were found"));
    }

    [Test]
    public void GetValue_Multiple_Throws()
    {
        var attachments = new IncomingAttachments
        {
            {"first", new("first", new MemoryStream(), 0, new HeaderDictionary())},
            {"second", new("second", new MemoryStream(), 0, new HeaderDictionary())}
        };

        var exception = Assert.Throws<Exception>(() => attachments.GetValue())!;
        Assert.That(exception.Message, Does.Contain("Found 2 attachments"));
        Assert.That(exception.Message, Does.Contain("first"));
        Assert.That(exception.Message, Does.Contain("second"));
    }

    [Test]
    public void TryGetValue_Empty_ReturnsFalse()
    {
        var attachments = new IncomingAttachments();

        Assert.That(attachments.TryGetValue(out var stream), Is.False);
        Assert.That(stream, Is.Null);
    }

    [Test]
    public void TryGetValue_Single_ReturnsTrue()
    {
        var attachments = new IncomingAttachments();
        var stream = new AttachmentStream("key", new MemoryStream(), 0, new HeaderDictionary());
        attachments.Add("key", stream);

        Assert.That(attachments.TryGetValue(out var found), Is.True);
        Assert.That(found, Is.SameAs(stream));
    }

    [Test]
    public void TryGetValue_Multiple_Throws()
    {
        var attachments = new IncomingAttachments
        {
            {"first", new("first", new MemoryStream(), 0, new HeaderDictionary())},
            {"second", new("second", new MemoryStream(), 0, new HeaderDictionary())}
        };

        var exception = Assert.Throws<Exception>(() => attachments.TryGetValue(out _))!;
        Assert.That(exception.Message, Does.Contain("Found 2 attachments"));
    }

    [Test]
    public void GetValueByName_Found_Returns()
    {
        var attachments = new IncomingAttachments();
        var stream = new AttachmentStream("key", new MemoryStream(), 0, new HeaderDictionary());
        attachments.Add("key", stream);

        Assert.That(attachments.GetValue("key"), Is.SameAs(stream));
    }

    [Test]
    public void GetValueByName_Empty_Throws()
    {
        var attachments = new IncomingAttachments();

        var exception = Assert.Throws<Exception>(() => attachments.GetValue("missing"))!;
        Assert.That(exception.Message, Does.Contain("'missing'"));
        Assert.That(exception.Message, Does.Contain("No attachments are available"));
    }

    [Test]
    public void GetValueByName_NotFound_ListsAvailable()
    {
        var attachments = new IncomingAttachments
        {
            {"first", new("first", new MemoryStream(), 0, new HeaderDictionary())},
            {"second", new("second", new MemoryStream(), 0, new HeaderDictionary())}
        };

        var exception = Assert.Throws<Exception>(() => attachments.GetValue("missing"))!;
        Assert.That(exception.Message, Does.Contain("'missing'"));
        Assert.That(exception.Message, Does.Contain("first"));
        Assert.That(exception.Message, Does.Contain("second"));
    }
}
