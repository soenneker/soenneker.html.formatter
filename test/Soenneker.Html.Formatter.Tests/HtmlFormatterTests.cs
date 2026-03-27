using Soenneker.Html.Formatter.Abstract;
using Soenneker.Tests.FixturedUnit;
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
}
