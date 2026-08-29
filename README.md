[![](https://img.shields.io/nuget/v/soenneker.html.formatter.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.html.formatter/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.html.formatter/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.html.formatter/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.html.formatter.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.html.formatter/)

# Soenneker.Html.Formatter

Provides utilities for formatting, pretty-printing, normalizing, reading, and saving HTML content.

## Install

```bash
dotnet add package Soenneker.Html.Formatter
```

## Quick start

```csharp
using Soenneker.Html.Formatter.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddHtmlFormatterAsSingleton();
```

Adds `IHtmlFormatter` as a singleton service.

## What you get

- `IHtmlFormatter` — Provides utilities for formatting, pretty-printing, normalizing, reading, and saving HTML content.
- `HtmlFormatterRegistrar` — A utility library that formats and normalizes HTML strings and files.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IHtmlFormatter.PrettyPrint(html, cancellationToken)` | Pretty-prints the specified HTML with indentation and readable formatting. | The pretty-printed HTML. |
| `IHtmlFormatter.Normalize(html, cancellationToken)` | Normalizes the specified HTML into a consistent serialized form without pretty-print indentation. | The normalized HTML. |
| `IHtmlFormatter.PrettyPrintFile(filePath, log, cancellationToken)` | Reads HTML from the specified file and pretty-prints it. | The pretty-printed HTML. |
| `IHtmlFormatter.NormalizeFile(filePath, log, cancellationToken)` | Reads HTML from the specified file and normalizes it. | The normalized HTML. |
| `IHtmlFormatter.SavePrettyPrintedFile(sourcePath, destinationPath, log, cancellationToken)` | Reads HTML from the source file, pretty-prints it, and saves the result. | A task representing the asynchronous save operation. |
| `IHtmlFormatter.SaveNormalizedFile(sourcePath, destinationPath, log, cancellationToken)` | Reads HTML from the source file, normalizes it, and saves the result. | A task representing the asynchronous save operation. |
| `IHtmlFormatter.PrettyPrintDirectory(directoryPath, recursive, log, cancellationToken)` | Formats all HTML files in the specified directory and saves the results in place. | A task representing the asynchronous formatting operation. |
| `HtmlFormatterRegistrar.AddHtmlFormatterAsSingleton(services)` | Adds `IHtmlFormatter` as a singleton service. | The same service collection, so additional registrations can be chained. |
| `HtmlFormatterRegistrar.AddHtmlFormatterAsScoped(services)` | Adds `IHtmlFormatter` as a scoped service. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
