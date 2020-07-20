<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /readme.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# <img src="https://raw.githubusercontent.com/SimonCropp/GraphQL.Attachments/master/src/icon.png" height="40px"> GraphQL.Attachments

[![Build status](https://ci.appveyor.com/api/projects/status/wq5ox06crbl9c2py/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/graphql-attachments)
[![NuGet Status](https://img.shields.io/nuget/v/GraphQL.Attachments.svg)](https://www.nuget.org/packages/GraphQL.Attachments/)

Provides access to a HTTP stream (via JavaScript on a web page) in [GraphQL](https://graphql-dotnet.github.io/) [Mutations](https://graphql-dotnet.github.io/docs/getting-started/mutations/) or [Queries](https://graphql-dotnet.github.io/docs/getting-started/queries). Attachments are transferred via a [multipart form](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Disposition).

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-graphql.attachments?utm_source=nuget-graphql.attachments&utm_medium=referral&utm_campaign=enterprise).

<!-- toc -->
## Contents

  * [Usage in Graphs](#usage-in-graphs)
  * [Server-side Controller](#server-side-controller)
    * [Non Attachments scenario](#non-attachments-scenario)
    * [With Attachments scenario](#with-attachments-scenario)
    * [Full Controller](#full-controller)
  * [Client - JavaScript](#client---javascript)
  * [Client - .NET](#client---net)
  * [Security contact information](#security-contact-information)<!-- endtoc -->


## NuGet package

https://nuget.org/packages/GraphQL.Attachments/

    PM> Install-Package GraphQL.Attachments


## Usage in Graphs

Incoming and Outgoing attachments can be accessed via the `ResolveFieldContext`:

<!-- snippet: UsageInGraphs -->
<a id='snippet-usageingraphs'/></a>
```cs
Field<ResultGraph>(
    "withAttachment",
    arguments: new QueryArguments(
        new QueryArgument<NonNullGraphType<StringGraphType>>
        {
            Name = "argument"
        }
    ),
    resolve: context =>
    {
        var incomingAttachments = context.IncomingAttachments();
        var outgoingAttachments = context.OutgoingAttachments();

        foreach (var incoming in incomingAttachments.Values)
        {
            // For sample purpose echo the incoming request
            // stream to the outgoing response stream
            var memoryStream = new MemoryStream();
            incoming.CopyTo(memoryStream);
            memoryStream.Position = 0;
            outgoingAttachments.AddStream(incoming.Name, memoryStream);
        }

        return new Result
        {
            Argument = context.GetArgument<string>("argument"),
        };
    });
```
<sup><a href='/src/Shared/Graphs/BaseRootGraph.cs#L24-L54' title='File snippet `usageingraphs` was extracted from'>snippet source</a> | <a href='#snippet-usageingraphs' title='Navigate to start of snippet `usageingraphs`'>anchor</a></sup>
<!-- endsnippet -->


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

<!-- snippet: ControllerPost -->
<a id='snippet-controllerpost'/></a>
```cs
[HttpPost]
public async Task Post(CancellationToken cancellation)
{
    var result = await RequestReader.ReadPost(Request, cancellation);
    await Execute(
        result.query,
        result.operation,
        result.attachments,
        result.inputs,
        cancellation);
}
```
<sup><a href='/src/SampleWeb/GraphQlController.cs#L24-L38' title='File snippet `controllerpost` was extracted from'>snippet source</a> | <a href='#snippet-controllerpost' title='Navigate to start of snippet `controllerpost`'>anchor</a></sup>
<!-- endsnippet -->

<!-- snippet: ControllerGet -->
<a id='snippet-controllerget'/></a>
```cs
[HttpGet]
public Task Get(CancellationToken cancellation)
{
    var (query, inputs, operation) = RequestReader.ReadGet(Request);
    return Execute(query, operation, null, inputs, cancellation);
}
```
<sup><a href='/src/SampleWeb/GraphQlController.cs#L40-L49' title='File snippet `controllerget` was extracted from'>snippet source</a> | <a href='#snippet-controllerget' title='Navigate to start of snippet `controllerget`'>anchor</a></sup>
<!-- endsnippet -->


#### Query Execution

To expose the attachments to the queries, the attachment context needs to be added to the `IDocumentExecuter`. This is done using `AttachmentsExtensions.ExecuteWithAttachments`:

<!-- snippet: ExecuteWithAttachments -->
<a id='snippet-executewithattachments'/></a>
```cs
var result = await executer.ExecuteWithAttachments(
    executionOptions,
    incomingAttachments);
```
<sup><a href='/src/SampleWeb/GraphQlController.cs#L71-L77' title='File snippet `executewithattachments` was extracted from'>snippet source</a> | <a href='#snippet-executewithattachments' title='Navigate to start of snippet `executewithattachments`'>anchor</a></sup>
<!-- endsnippet -->


#### Result Writing

As with RequestReader for the incoming data, the outgoing data needs to be written with any resulting attachments. To facilitate this [ResponseWriter](/src/GraphQL.Attachments/ResponseWriter.cs) is used.

<!-- snippet: ResponseWriter -->
<a id='snippet-responsewriter'/></a>
```cs
await ResponseWriter.WriteResult(Response, result, cancellation);
```
<sup><a href='/src/SampleWeb/GraphQlController.cs#L79-L83' title='File snippet `responsewriter` was extracted from'>snippet source</a> | <a href='#snippet-responsewriter' title='Navigate to start of snippet `responsewriter`'>anchor</a></sup>
<!-- endsnippet -->


### Full Controller

[Full sample GraphQlController](https://github.com/SimonCropp/GraphQL.Attachments/blob/master/src/SampleWeb/GraphQlController.cs).


## Client - JavaScript

The JavaScript that submits the query does so through by building up a [FormData](https://developer.mozilla.org/en-US/docs/Web/API/FormData) object and [POSTing](https://developer.mozilla.org/en-US/docs/Learn/HTML/Forms/Sending_and_retrieving_form_data#The_POST_method) that via the [Fetch API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API).

#### Helper method for builgin post settings

<!-- snippet: BuildPostSettings -->
<a id='snippet-buildpostsettings'/></a>
```html
function BuildPostSettings() {
    var data = new FormData();
    var files = document.getElementById("files").files;
    for (var i = 0; i < files.length; i++) {
        data.append('files[]', files[i], files[i].name);
    }
    data.append(
        "query",
        'mutation{ withAttachment (argument: "argumentValue"){argument}}'
    );

    return {
        method: 'POST',
        body: data
    };
}
```
<sup><a href='/src/SampleWeb/test.html#L44-L61' title='File snippet `buildpostsettings` was extracted from'>snippet source</a> | <a href='#snippet-buildpostsettings' title='Navigate to start of snippet `buildpostsettings`'>anchor</a></sup>
<!-- endsnippet -->


#### Post mutation and download result

<!-- snippet: PostMutationAndDownloadFile -->
<a id='snippet-postmutationanddownloadfile'/></a>
```html
function PostMutationAndDownloadFile() {

    var postSettings = BuildPostSettings();
    return fetch('graphql', postSettings)
        .then(function (data) {
            return data.formData().then(x => {
                var resultContent = '';
                x.forEach(e => {
                    // This is the attachments
                    if (e.name) {
                        var a = document.createElement('a');
                        var blob = new Blob([e]);
                        a.href = window.URL.createObjectURL(blob);
                        a.download = e.name;
                        a.click();
                    }
                    else {
                        resultContent += JSON.stringify(e);
                    }
                });
                result.innerHTML = resultContent;
            });
        });
}
```
<sup><a href='/src/SampleWeb/test.html#L17-L42' title='File snippet `postmutationanddownloadfile` was extracted from'>snippet source</a> | <a href='#snippet-postmutationanddownloadfile' title='Navigate to start of snippet `postmutationanddownloadfile`'>anchor</a></sup>
<!-- endsnippet -->


#### Post mutation and display text result

<!-- snippet: PostMutationWithTextResult -->
<a id='snippet-postmutationwithtextresult'/></a>
```html
function PostMutationWithTextResult() {
    var postSettings = BuildPostSettings();
    return fetch('graphql', postSettings)
        .then(function (data) {
            return data.text().then(x => {
                result.innerHTML = x;
            });
        });
}
```
<sup><a href='/src/SampleWeb/test.html#L5-L15' title='File snippet `postmutationwithtextresult` was extracted from'>snippet source</a> | <a href='#snippet-postmutationwithtextresult' title='Navigate to start of snippet `postmutationwithtextresult`'>anchor</a></sup>
<!-- endsnippet -->


## Client - .NET

Creating and posting a multipart form can be done using a combination of [MultipartFormDataContent](https://msdn.microsoft.com/en-us/library/system.net.http.multipartformdatacontent.aspx) and [HttpClient.PostAsync](https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.postasync.aspx). To simplify this action the `ClientQueryExecutor` class can be used:

<!-- snippet: QueryExecutor.cs -->
<a id='snippet-QueryExecutor.cs'/></a>
```cs
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Attachments
{
    public class QueryExecutor
    {
        HttpClient client;
        string uri;

        public QueryExecutor(HttpClient client, string uri = "graphql")
        {
            Guard.AgainstNull(nameof(client), client);
            Guard.AgainstNullWhiteSpace(nameof(uri), uri);

            this.client = client;
            this.uri = uri;
        }

        public Task<QueryResult> ExecutePost(string query, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            return ExecutePost(new PostRequest(query), cancellation);
        }

        public async Task<QueryResult> ExecutePost(PostRequest request, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(request), request);
            Guard.AgainstNull(nameof(request), request);
            using var content = new MultipartFormDataContent();
            content.AddQueryAndVariables(request.Query, request.Variables, request.OperationName);

            if (request.Action != null)
            {
                var postContext = new PostContext(content);
                request.Action?.Invoke(postContext);
                postContext.HeadersAction?.Invoke(content.Headers);
            }

            var response = await client.PostAsync(uri, content, cancellation);
            var result = await response.ProcessResponse(cancellation);
            return new QueryResult(result.Stream, result.Attachments, response.Content.Headers, response.StatusCode);
        }

        public Task<QueryResult> ExecuteGet(string query, CancellationToken cancellation = default)
        {
            Guard.AgainstNullWhiteSpace(nameof(query), query);
            return ExecuteGet(new GetRequest(query), cancellation);
        }

        public async Task<QueryResult> ExecuteGet(GetRequest request, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(nameof(request), request);
            var compressed = Compress.Query(request.Query);
            var variablesString = RequestAppender.ToJson(request.Variables);
            var getUri = UriBuilder.GetUri(uri, variablesString, compressed, request.OperationName);

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, getUri);
            request.HeadersAction?.Invoke(getRequest.Headers);
            var response = await client.SendAsync(getRequest, cancellation);
            return await response.ProcessResponse(cancellation);
        }
    }
}
```
<sup><a href='/src/GraphQL.Attachments.Client/QueryExecutor.cs#L1-L65' title='File snippet `QueryExecutor.cs` was extracted from'>snippet source</a> | <a href='#snippet-QueryExecutor.cs' title='Navigate to start of snippet `QueryExecutor.cs`'>anchor</a></sup>
<!-- endsnippet -->

This can be useful when performing [Integration testing in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/testing/integration-testing).


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

<a href="https://thenounproject.com/term/database/1631008/" target="_blank">memory</a> designed by H Alberto Gongora from [The Noun Project](https://thenounproject.com)
