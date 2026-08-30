[![](https://img.shields.io/nuget/v/soenneker.html.formatter.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.html.formatter/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.html.formatter/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.html.formatter/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.html.formatter/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.html.formatter/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.html.formatter.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.html.formatter/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.html.formatter/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.html.formatter/actions/workflows/codeql.yml)

# Soenneker.Html.Formatter

Formats HTML strings, files, and directories with AngleSharp.

## Install

```bash
dotnet add package Soenneker.Html.Formatter
```

## Register

```csharp
using Soenneker.Html.Formatter.Registrars;

services.AddHtmlFormatterAsSingleton();
```

A scoped registration is also available through `AddHtmlFormatterAsScoped()`.

## Format strings

```csharp
using Soenneker.Html.Formatter.Abstract;

string readable = await formatter.PrettyPrint(
    "<main><h1>Account</h1><p>Ready</p></main>",
    cancellationToken);

string compact = await formatter.Normalize(readable, cancellationToken);
```

Both methods parse and serialize the input rather than only changing whitespace. AngleSharp may repair malformed markup, normalize element or attribute syntax, and supply document structure when the input represents a full document. A null, empty, or whitespace-only string produces an empty string.

## Format files

```csharp
string preview = await formatter.PrettyPrintFile("page.html", cancellationToken: cancellationToken);

await formatter.SavePrettyPrintedFile(
    sourcePath: "page.html",
    destinationPath: "output/page.html",
    cancellationToken: cancellationToken);

await formatter.SaveNormalizedFile(
    sourcePath: "page.html",
    cancellationToken: cancellationToken); // overwrites page.html
```

The methods ending in `File` return formatted content without changing the source. The methods beginning with `Save` write to `destinationPath`, or overwrite the source when no destination is supplied.

## Format a directory

```csharp
await formatter.PrettyPrintDirectory(
    "templates",
    recursive: true,
    cancellationToken: cancellationToken);
```

This overwrites `.html` and `.htm` files in place. With `recursive: false`, only files directly inside the specified directory are processed. Cancellation does not roll back files already written.
