using Soenneker.Html.Formatter.Abstract;
using Soenneker.Tests.HostedUnit;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Html.Formatter.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class HtmlFormatterTests : HostedUnitTest
{
    private readonly IHtmlFormatter _util;

    public HtmlFormatterTests(Host host) : base(host)
    {
        _util = Resolve<IHtmlFormatter>(true);
    }

    [Test]
    public void Default()
    {

    }

    [Test]
    public async ValueTask PrettyPrintDirectory_saves_formatted_html_in_place(CancellationToken cancellationToken)
    {
        string directory = Path.Combine(Path.GetTempPath(), $"html-formatter-{Guid.NewGuid():N}");
        string file = Path.Combine(directory, "index.html");
        const string input = "<div><span>Hello</span></div>";

        Directory.CreateDirectory(directory);

        try
        {
            await File.WriteAllTextAsync(file, input);
            string expected = await _util.PrettyPrint(input, cancellationToken: cancellationToken);

            await _util.PrettyPrintDirectory(directory, log: false, cancellationToken: cancellationToken);

            string actual = await File.ReadAllTextAsync(file);
            await Assert.That(actual).IsEqualTo(expected);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
