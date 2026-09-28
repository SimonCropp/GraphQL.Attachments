public class IncomingAttachmentsTests
{
    [Test]
    public async Task GetValue_Single_Returns()
    {
        var attachments = new IncomingAttachments();
        var stream = new AttachmentStream("key", new MemoryStream(), 0, new HeaderDictionary());
        attachments.Add("key", stream);

        await Assert.That(attachments.GetValue()).IsSameReferenceAs(stream);
    }

    [Test]
    public async Task GetValue_Empty_Throws()
    {
        var attachments = new IncomingAttachments();

        var exception = await Assert.That(() => attachments.GetValue()).ThrowsExactly<Exception>();
        await Assert.That(exception!.Message).Contains("none were found");
    }

    [Test]
    public async Task GetValue_Multiple_Throws()
    {
        var attachments = new IncomingAttachments
        {
            {"first", new("first", new MemoryStream(), 0, new HeaderDictionary())},
            {"second", new("second", new MemoryStream(), 0, new HeaderDictionary())}
        };

        var exception = await Assert.That(() => attachments.GetValue()).ThrowsExactly<Exception>();
        await Assert.That(exception!.Message).Contains("Found 2 attachments");
        await Assert.That(exception!.Message).Contains("first");
        await Assert.That(exception!.Message).Contains("second");
    }

    [Test]
    public async Task TryGetValue_Empty_ReturnsFalse()
    {
        var attachments = new IncomingAttachments();

        await Assert.That(attachments.TryGetValue(out var stream)).IsFalse();
        await Assert.That(stream).IsNull();
    }

    [Test]
    public async Task TryGetValue_Single_ReturnsTrue()
    {
        var attachments = new IncomingAttachments();
        var stream = new AttachmentStream("key", new MemoryStream(), 0, new HeaderDictionary());
        attachments.Add("key", stream);

        await Assert.That(attachments.TryGetValue(out var found)).IsTrue();
        await Assert.That(found).IsSameReferenceAs(stream);
    }

    [Test]
    public async Task TryGetValue_Multiple_Throws()
    {
        var attachments = new IncomingAttachments
        {
            {"first", new("first", new MemoryStream(), 0, new HeaderDictionary())},
            {"second", new("second", new MemoryStream(), 0, new HeaderDictionary())}
        };

        var exception = await Assert.That(() => attachments.TryGetValue(out _)).ThrowsExactly<Exception>();
        await Assert.That(exception!.Message).Contains("Found 2 attachments");
    }

    [Test]
    public async Task GetValueByName_Found_Returns()
    {
        var attachments = new IncomingAttachments();
        var stream = new AttachmentStream("key", new MemoryStream(), 0, new HeaderDictionary());
        attachments.Add("key", stream);

        await Assert.That(attachments.GetValue("key")).IsSameReferenceAs(stream);
    }

    [Test]
    public async Task GetValueByName_Empty_Throws()
    {
        var attachments = new IncomingAttachments();

        var exception = await Assert.That(() => attachments.GetValue("missing")).ThrowsExactly<Exception>();
        await Assert.That(exception!.Message).Contains("'missing'");
        await Assert.That(exception!.Message).Contains("No attachments are available");
    }

    [Test]
    public async Task GetValueByName_NotFound_ListsAvailable()
    {
        var attachments = new IncomingAttachments
        {
            {"first", new("first", new MemoryStream(), 0, new HeaderDictionary())},
            {"second", new("second", new MemoryStream(), 0, new HeaderDictionary())}
        };

        var exception = await Assert.That(() => attachments.GetValue("missing")).ThrowsExactly<Exception>();
        await Assert.That(exception!.Message).Contains("'missing'");
        await Assert.That(exception!.Message).Contains("first");
        await Assert.That(exception!.Message).Contains("second");
    }
}
