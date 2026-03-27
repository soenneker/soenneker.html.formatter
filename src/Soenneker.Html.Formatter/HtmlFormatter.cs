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

    public ValueTask<string> Format(string? html, CancellationToken cancellationToken = default) => PrettyPrint(html, cancellationToken);

    public ValueTask<string> PrettyPrint(string? html, CancellationToken cancellationToken = default) => Process(html, _prettyFormatter, cancellationToken);

    public ValueTask<string> Normalize(string? html, CancellationToken cancellationToken = default) =>
        Process(html, HtmlMarkupFormatter.Instance, cancellationToken);

    public ValueTask<string> FormatFile(string filePath, bool log = true, CancellationToken cancellationToken = default)
    {
        return PrettyPrintFile(filePath, log, cancellationToken);
    }

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

    public ValueTask SaveFormattedFile(string sourcePath, string? destinationPath = null, bool log = true, CancellationToken cancellationToken = default)
    {
        return SavePrettyPrintedFile(sourcePath, destinationPath, log, cancellationToken);
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

    private Task<string> ReadFile(string filePath, bool log, CancellationToken cancellationToken)
    {
        if (filePath.IsNullOrWhiteSpace())
            throw new ArgumentException("File path must be provided.", nameof(filePath));

        return _fileUtil.Read(filePath, log, cancellationToken);
    }

    private async ValueTask Save(string sourcePath, string? destinationPath, string content, bool log, CancellationToken cancellationToken)
    {
        string targetPath = destinationPath.IsNullOrWhiteSpace() ? sourcePath : destinationPath;

        if (targetPath.IsNullOrWhiteSpace())
            throw new ArgumentException("Destination path must be provided.", nameof(destinationPath));

        string? directory = Path.GetDirectoryName(targetPath);

        if (!string.IsNullOrWhiteSpace(directory))
            await _directoryUtil.Create(directory, log, cancellationToken)
                                .NoSync();

        await _fileUtil.Write(targetPath, content, log, cancellationToken)
                       .NoSync();
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