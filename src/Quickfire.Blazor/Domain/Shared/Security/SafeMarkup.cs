using System.Net;
using Ganss.Xss;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace Quickfire.Blazor.Domain.Shared.Security;

public static class SafeMarkup
{
    private static readonly Lazy<HtmlSanitizer> Sanitizer = new(CreateSanitizer);
    private static readonly Lazy<MarkdownPipeline> MarkdownPipeline = new(CreateMarkdownPipeline);

    public static string HtmlEncode(string? text) => WebUtility.HtmlEncode(text ?? string.Empty);

    public static string SanitizeHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        return Sanitizer.Value.Sanitize(html);
    }

    public static MarkupString SanitizedHtml(string? html) => new(SanitizeHtml(html));

    public static MarkupString Markdown(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return new MarkupString(string.Empty);
        }

        var renderedHtml = Markdig.Markdown.ToHtml(markdown, MarkdownPipeline.Value);
        return new MarkupString(SanitizeHtml(renderedHtml));
    }

    private static MarkdownPipeline CreateMarkdownPipeline()
    {
        return new MarkdownPipelineBuilder()
            .DisableHtml()
            .UseAdvancedExtensions()
            .Build();
    }

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();

        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedTags.UnionWith(new[]
        {
            "a",
            "b",
            "blockquote",
            "br",
            "code",
            "div",
            "em",
            "h1",
            "h2",
            "h3",
            "h4",
            "h5",
            "h6",
            "hr",
            "i",
            "img",
            "li",
            "ol",
            "p",
            "pre",
            "s",
            "span",
            "strong",
            "sub",
            "sup",
            "table",
            "tbody",
            "td",
            "th",
            "thead",
            "tr",
            "u",
            "ul",
            "video",
            "source"
        });

        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.UnionWith(new[]
        {
            "alt",
            "aria-controls",
            "aria-describedby",
            "aria-expanded",
            "aria-hidden",
            "aria-label",
            "class",
            "controls",
            "height",
            "href",
            "id",
            "poster",
            "preload",
            "rel",
            "role",
            "src",
            "target",
            "title",
            "type",
            "width"
        });

        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.UnionWith(new[] { "http", "https", "mailto", "tel", "file" });

        sanitizer.AllowDataAttributes = false;
        sanitizer.KeepChildNodes = true;

        // Ensure inline styles are stripped even if present in input.
        sanitizer.AllowedCssProperties.Clear();
        sanitizer.AllowedAtRules.Clear();

        return sanitizer;
    }
}
