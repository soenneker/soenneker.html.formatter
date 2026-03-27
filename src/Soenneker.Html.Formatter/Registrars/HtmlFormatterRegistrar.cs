using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.AngleSharp.Parser.Registrars;
using Soenneker.Html.Formatter.Abstract;
using Soenneker.Utils.Directory.Registrars;
using Soenneker.Utils.File.Registrars;

namespace Soenneker.Html.Formatter.Registrars;

/// <summary>
/// A utility library that formats and normalizes HTML strings and files.
/// </summary>
public static class HtmlFormatterRegistrar
{
    /// <summary>
    /// Adds <see cref="IHtmlFormatter"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddHtmlFormatterAsSingleton(this IServiceCollection services)
    {
        services.AddFileUtilAsSingleton()
                .AddDirectoryUtilAsSingleton()
                .AddAngleSharpParserAsSingleton()
                .TryAddSingleton<IHtmlFormatter, HtmlFormatter>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IHtmlFormatter"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddHtmlFormatterAsScoped(this IServiceCollection services)
    {
        services.AddFileUtilAsScoped()
                .AddDirectoryUtilAsScoped()
                .AddAngleSharpParserAsScoped()
                .TryAddScoped<IHtmlFormatter, HtmlFormatter>();

        return services;
    }
}