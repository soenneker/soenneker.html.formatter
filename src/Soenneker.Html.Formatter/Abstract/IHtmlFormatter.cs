using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Html.Formatter.Abstract;

/// <summary>
/// Provides utilities for formatting, pretty-printing, normalizing, reading, and saving HTML content.
/// </summary>
public interface IHtmlFormatter
{
    /// <summary>
    /// Pretty-prints the specified HTML with indentation and readable formatting.
    /// </summary>
    /// <param name="html">The HTML content to pretty-print.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The pretty-printed HTML.</returns>
    ValueTask<string> PrettyPrint(string? html, CancellationToken cancellationToken = default);

    /// <summary>
    /// Normalizes the specified HTML into a consistent serialized form without pretty-print indentation.
    /// </summary>
    /// <param name="html">The HTML content to normalize.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The normalized HTML.</returns>
    ValueTask<string> Normalize(string? html, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads HTML from the specified file and pretty-prints it.
    /// </summary>
    /// <param name="filePath">The path to the HTML file.</param>
    /// <param name="log">Whether file operations should be logged.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The pretty-printed HTML.</returns>
    ValueTask<string> PrettyPrintFile(string filePath, bool log = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads HTML from the specified file and normalizes it.
    /// </summary>
    /// <param name="filePath">The path to the HTML file.</param>
    /// <param name="log">Whether file operations should be logged.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The normalized HTML.</returns>
    ValueTask<string> NormalizeFile(string filePath, bool log = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads HTML from the source file, pretty-prints it, and saves the result.
    /// </summary>
    /// <param name="sourcePath">The source HTML file path.</param>
    /// <param name="destinationPath">
    /// The destination file path. When <see langword="null"/>, the source file is overwritten.
    /// </param>
    /// <param name="log">Whether file operations should be logged.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    ValueTask SavePrettyPrintedFile(string sourcePath, string? destinationPath = null, bool log = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads HTML from the source file, normalizes it, and saves the result.
    /// </summary>
    /// <param name="sourcePath">The source HTML file path.</param>
    /// <param name="destinationPath">
    /// The destination file path. When <see langword="null"/>, the source file is overwritten.
    /// </param>
    /// <param name="log">Whether file operations should be logged.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    ValueTask SaveNormalizedFile(string sourcePath, string? destinationPath = null, bool log = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Formats all HTML files in the specified directory and saves the results in place.
    /// </summary>
    /// <param name="directoryPath">The directory that contains HTML files.</param>
    /// <param name="recursive">Whether subdirectories should also be processed.</param>
    /// <param name="log">Whether file operations should be logged.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous formatting operation.</returns>
    ValueTask PrettyPrintDirectory(string directoryPath, bool recursive = false, bool log = true, CancellationToken cancellationToken = default);
}