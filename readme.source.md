# <img src="https://raw.githubusercontent.com/SimonCropp/GraphQL.Attachments/master/src/icon.png" height="40px"> GraphQL.Attachments

[![Build status](https://ci.appveyor.com/api/projects/status/wq5ox06crbl9c2py/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/GraphQL.Attachments)
[![NuGet Status](https://img.shields.io/nuget/v/GraphQL.Attachments.svg?cacheSeconds=86400)](https://www.nuget.org/packages/GraphQL.Attachments/)

GraphQL.Attachments provides a bridge between an HTTP stream (via JavaScript on a web page) and exposes that stream to a [GraphQL](https://graphql-dotnet.github.io/) [Mutation](https://graphql-dotnet.github.io/docs/getting-started/mutations/) or [Query](https://graphql-dotnet.github.io/docs/getting-started/queries). Attachments are transfered via a [multipart form](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Disposition).


toc


## NuGet

https://nuget.org/packages/GraphQL.Attachments/

    PM> Install-Package GraphQL.Attachments


## Usage in Graphs

Incoming and Outgoing attachments can be accessed via the `ResolveFieldContext`:

snippet: UsageInGraphs


## Server-side Controller


### Non Attachments scenario

In the usual usage scenario of graphQL in a Controller the query is passed in via a model object:

```cs
public class PostBody
{
    public string? OperationName;
    public string Query = null!;
    public JObject? Variables;
}
```

Which is then extracted using model binding:

```cs
[HttpPost]
public Task<ExecutionResult> Post(
    [BindRequired, FromBody] PostBody body)
{
    // run graphQL query
```


### With Attachments scenario


#### RequestReader instead of binding

When using Attachments the incoming request also requires the incoming form data to be parse. To facilitate this [RequestReader](/src/GraphQL.Attachments/RequestReader.cs) is used. This removes the requirement for model binding. The resulting Post and Get become:

snippet: ControllerPost

snippet: ControllerGet


#### Query Execution

To expose the attachments to the queries, the attachment context needs to be added to the `IDocumentExecuter`. This is done using `AttachmentsExtensions.ExecuteWithAttachments`:

snippet: ExecuteWithAttachments


#### Result Writing

As with RequestReader for the incoming data, the outgoing data needs to be written with any resulting attachments. To facilitate this [ResponseWriter](/src/GraphQL.Attachments/ResponseWriter.cs) is used.

snippet: ResponseWriter


## Client - JavaScript


### Form submission

The JavaScript that submits the query does so through by building up a [FormData](https://developer.mozilla.org/en-US/docs/Web/API/FormData) object and [POSTing](https://developer.mozilla.org/en-US/docs/Learn/HTML/Forms/Sending_and_retrieving_form_data#The_POST_method) that via the [Fetch API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API).

snippet: PostMutation


## Client - .NET

Creating and posting a multipart form can be done using a combination of [MultipartFormDataContent](https://msdn.microsoft.com/en-us/library/system.net.http.multipartformdatacontent.aspx) and [HttpClient.PostAsync](https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.postasync.aspx). To simplify this action the `ClientQueryExecutor` class can be used:

snippet: ClientQueryExecutor.cs

This can be useful when performing [Integration testing in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/testing/integration-testing).


## Icon

<a href="https://thenounproject.com/term/database/1631008/" target="_blank">memory</a> designed by H Alberto Gongora from [The Noun Project](https://thenounproject.com)