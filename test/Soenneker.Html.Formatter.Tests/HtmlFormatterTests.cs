using Soenneker.Html.Formatter.Abstract;
using Soenneker.Tests.HostedUnit;
using System;
using System.IO;
using System.Threading.Tasks;
using Soenneker.Tests.Attributes.Local;

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

    [LocalOnly]
    public async ValueTask PrettyPrintDirectory()
    {
       // _util.PrettyPrintDirectory("c:\")
    }

}
