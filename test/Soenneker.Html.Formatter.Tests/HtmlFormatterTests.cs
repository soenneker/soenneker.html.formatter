using Soenneker.Html.Formatter.Abstract;
using Soenneker.Tests.FixturedUnit;
using System;
using System.IO;
using System.Threading.Tasks;
using Soenneker.Facts.Local;
using Xunit;

namespace Soenneker.Html.Formatter.Tests;

[Collection("Collection")]
public sealed class HtmlFormatterTests : FixturedUnitTest
{
    private readonly IHtmlFormatter _util;

    public HtmlFormatterTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IHtmlFormatter>(true);
    }

    [Fact]
    public void Default()
    {

    }

    [LocalFact]
    public async ValueTask PrettyPrintDirectory()
    {
       // _util.PrettyPrintDirectory("c:\")
    }

}
