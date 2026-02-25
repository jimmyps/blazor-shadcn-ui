using ColorCode;

namespace NeoUI.Demo.Shared;

/// <summary>
/// Lightweight server/WASM-safe syntax highlighter backed by ColorCode.HTML.
/// Produces class-based HTML spans styled via the project's highlight CSS variables.
/// </summary>
internal static class CodeHighlighter
{
    private static readonly HtmlClassFormatter _formatter = new();

    /// <summary>
    /// Returns an HTML string with syntax-highlighted spans for the given code.
    /// Language is auto-detected: Razor/HTML when the first non-whitespace character
    /// is <c>&lt;</c>, otherwise C#.
    /// The ColorCode outer <c>&lt;div&gt;</c> wrapper is stripped so the result
    /// can be embedded directly inside a <c>&lt;pre&gt;&lt;code&gt;</c> block.
    /// </summary>
    public static string Highlight(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return string.Empty;

        var language = code.TrimStart().StartsWith('<') ? Languages.Html : Languages.CSharp;

        try
        {
            var html = _formatter.GetHtmlString(code, language);
            return StripOuterDiv(html);
        }
        catch
        {
            return System.Net.WebUtility.HtmlEncode(code);
        }
    }

    // ColorCode wraps output in <div class="{languageId}">...</div>.
    // Strip it so the content sits cleanly inside <pre><code>.
    private static string StripOuterDiv(string html)
    {
        var start = html.IndexOf('>') + 1;
        var end = html.LastIndexOf('<');
        return start > 0 && end > start ? html[start..end] : html;
    }
}
