using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Html.Formatter.Abstract;
using Soenneker.Utils.Directory.Abstract;
using Soenneker.Utils.File.Abstract;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Soenneker.AngleSharp.Parser.Abstract;
using Soenneker.AngleSharp.Parser.Enums;
using Soenneker.Extensions.String;
using System.Collections.Generic;

namespace Soenneker.Html.Formatter;

/// <inheritdoc cref="IHtmlFormatter"/>
public sealed class HtmlFormatter : IHtmlFormatter
{
    private static readonly PrettyMarkupFormatter _prettyFormatter = new();

    private readonly IFileUtil _fileUtil;
    private readonly IDirectoryUtil _directoryUtil;
    private readonly IAngleSharpParser _angleSharpParser;

    public HtmlFormatter(IFileUtil fileUtil, IDirectoryUtil directoryUtil, IAngleSharpParser angleSharpParser)
    {
        _fileUtil = fileUtil;
        _directoryUtil = directoryUtil;
        _angleSharpParser = angleSharpParser;
    }

    public ValueTask<string> PrettyPrint(string? html, CancellationToken cancellationToken = default) => Process(html, _prettyFormatter, cancellationToken);

    public ValueTask<string> Normalize(string? html, CancellationToken cancellationToken = default) =>
        Process(html, HtmlMarkupFormatter.Instance, cancellationToken);

    public async ValueTask<string> PrettyPrintFile(string filePath, bool log = true, CancellationToken cancellationToken = default)
    {
        string html = await ReadFile(filePath, log, cancellationToken)
            .NoSync();

        return await PrettyPrint(html, cancellationToken)
            .NoSync();
    }

    public async ValueTask<string> NormalizeFile(string filePath, bool log = true, CancellationToken cancellationToken = default)
    {
        string html = await ReadFile(filePath, log, cancellationToken)
            .NoSync();

        return await Normalize(html, cancellationToken)
            .NoSync();
    }

    public async ValueTask SavePrettyPrintedFile(string sourcePath, string? destinationPath = null, bool log = true,
        CancellationToken cancellationToken = default)
    {
        string formatted = await PrettyPrintFile(sourcePath, log, cancellationToken)
            .NoSync();

        await Save(sourcePath, destinationPath, formatted, log, cancellationToken)
            .NoSync();
    }

    public async ValueTask SaveNormalizedFile(string sourcePath, string? destinationPath = null, bool log = true, CancellationToken cancellationToken = default)
    {
        string normalized = await NormalizeFile(sourcePath, log, cancellationToken)
            .NoSync();

        await Save(sourcePath, destinationPath, normalized, log, cancellationToken)
            .NoSync();
    }

    public async ValueTask PrettyPrintDirectory(string directoryPath, bool recursive = false, bool log = true,
        CancellationToken cancellationToken = default)
    {
        if (directoryPath.IsNullOrWhiteSpace())
            throw new ArgumentException("Directory path must be provided.", nameof(directoryPath));

        List<string> htmlFiles = await GetHtmlFiles(directoryPath, recursive, cancellationToken)
            .NoSync();

        foreach (string htmlFile in htmlFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await PrettyPrintFile(htmlFile, log: log, cancellationToken: cancellationToken)
                .NoSync();
        }
    }

    private Task<string> ReadFile(string filePath, bool log, CancellationToken cancellationToken)
    {
        filePath.ThrowIfNullOrWhiteSpace();

        return _fileUtil.Read(filePath, log, cancellationToken);
    }

    private async ValueTask Save(string sourcePath, string? destinationPath, string content, bool log, CancellationToken cancellationToken)
    {
        string targetPath = destinationPath.IsNullOrWhiteSpace() ? sourcePath : destinationPath;

        targetPath.ThrowIfNullOrWhiteSpace();

        string? directory = Path.GetDirectoryName(targetPath);

        if (directory.HasContent())
            await _directoryUtil.Create(directory, log, cancellationToken)
                                .NoSync();

        await _fileUtil.Write(targetPath, content, log, cancellationToken)
                       .NoSync();
    }

    private async ValueTask<List<string>> GetHtmlFiles(string directoryPath, bool recursive, CancellationToken cancellationToken)
    {
        List<string> htmlFiles = await _directoryUtil.GetFilesByExtension(directoryPath, ".html", recursive, cancellationToken)
                                                     .NoSync();
        List<string> htmFiles = await _directoryUtil.GetFilesByExtension(directoryPath, ".htm", recursive, cancellationToken)
                                                    .NoSync();

        if (htmFiles.Count == 0)
            return htmlFiles;

        htmlFiles.AddRange(htmFiles);

        return htmlFiles;
    }

    private async ValueTask<string> Process(string? html, IMarkupFormatter formatter, CancellationToken cancellationToken)
    {
        if (html.IsNullOrWhiteSpace())
            return string.Empty;

        string input = StripBom(html);

        return LooksLikeDocument(input)
            ? await SerializeDocument(input, formatter, cancellationToken)
                .NoSync()
            : await SerializeFragment(input, formatter, cancellationToken)
                .NoSync();
    }

    private async ValueTask<string> SerializeDocument(string html, IMarkupFormatter formatter, CancellationToken cancellationToken)
    {
        HtmlParser parser = await _angleSharpParser.Get(cancellationToken)
                                                   .NoSync();

        IHtmlDocument document = await parser.ParseDocumentAsync(html, cancellationToken)
                                             .NoSync();

        var builder = new StringBuilder(Math.Max(html.Length + 64, 256));

        await using var writer = new StringWriter(builder, CultureInfo.InvariantCulture);
        document.ToHtml(writer, formatter);

        return TrimTrailingLineEndings(builder.ToString());
    }

    private async ValueTask<string> SerializeFragment(string html, IMarkupFormatter formatter, CancellationToken cancellationToken)
    {
        HtmlParser parser = await _angleSharpParser.Get(AngleSharpContextType.Fast, cancellationToken)
                                                   .NoSync();

        IHtmlDocument document = await parser.ParseDocumentAsync("<body></body>", cancellationToken)
                                             .NoSync();
        IElement context = document.Body ?? document.CreateElement("body");
        INodeList nodes = parser.ParseFragment(html, context);

        var builder = new StringBuilder(Math.Max(html.Length + 64, 256));

        await using var writer = new StringWriter(builder, CultureInfo.InvariantCulture);

        foreach (INode node in nodes)
        {
            node.ToHtml(writer, formatter);
        }

        return TrimTrailingLineEndings(builder.ToString());
    }

    private static bool LooksLikeDocument(string html)
    {
        return html.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) || html.Contains("<html", StringComparison.OrdinalIgnoreCase) ||
               html.Contains("<head", StringComparison.OrdinalIgnoreCase) || html.Contains("<body", StringComparison.OrdinalIgnoreCase);
    }

    private static string StripBom(string value)
    {
        return value.Length > 0 && value[0] == '\uFEFF' ? value[1..] : value;
    }

    private static string TrimTrailingLineEndings(string value)
    {
        return value.TrimEnd('\r', '\n');
    }
}