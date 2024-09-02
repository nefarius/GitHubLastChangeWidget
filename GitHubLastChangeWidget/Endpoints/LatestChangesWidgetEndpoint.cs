using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

using FastEndpoints;

using FluentValidation;

using GitHubLastChangeWidget.Services;

using Humanizer;

using Microsoft.Extensions.Primitives;

using Octokit;

using VectSharp;
using VectSharp.Markdown;
using VectSharp.SVG;

using Page = VectSharp.Page;
using Point = VectSharp.Point;
using StringBuilder = System.Text.StringBuilder;

namespace GitHubLastChangeWidget.Endpoints;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
internal sealed class LatestChangesWidgetEndpointRequest
{
    /// <summary>
    ///     The GitHub username.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    ///     The GitHub repository.
    /// </summary>
    public required string Repository { get; set; }

    /// <summary>
    ///     The base font size to use to render.
    /// </summary>
    public double? BaseFontSize { get; set; } = 14;

    /// <summary>
    ///     The foreground (text) color.
    /// </summary>
    public string? ForegroundColour { get; set; } = Colours.Black.ToCSSString(false) /*"#C4D1DE"*/;

    /// <summary>
    ///     The background color.
    /// </summary>
    public string? BackgroundColour { get; set; }

    /// <summary>
    ///     The rendered page width.
    /// </summary>
    public double? Width { get; set; } = 830;

    /// <summary>
    ///     The maximum amount of commits to include in the table. Valid values include 1 to 50.
    /// </summary>
    public int? MaxCommits { get; set; } = 5;
}

internal sealed class LatestChangesWidgetEndpointRequestValidator : Validator<LatestChangesWidgetEndpointRequest>
{
    public LatestChangesWidgetEndpointRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(39)
            .WithMessage("Please specify a GitHub username or organisation name!");

        RuleFor(x => x.Repository)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(100)
            .WithMessage("Please specify a public GitHub repository name!");

        RuleFor(x => x.MaxCommits)
            .InclusiveBetween(1, 50)
            .WithMessage("Allowed values include 1 to 50.");
    }
}

/// <summary>
///     Generates an SVG image of the latest commit activity for a given repository.
/// </summary>
internal sealed partial class LatestChangesWidgetEndpoint(GitHubApiService gitHubApiService)
    : Endpoint<LatestChangesWidgetEndpointRequest>
{
    /// <summary>
    ///     We need to convert newlines in commit messages to single line strings with HTML line breaks to not screw up the
    ///     Markdown table syntax.
    /// </summary>
    [GeneratedRegex(@"\r\n?|\n", RegexOptions.Multiline)]
    private static partial Regex NewlineRegex();

    public override void Configure()
    {
        Get("/widgets/github/{Username}/{Repository}/changes/latest");
        AllowAnonymous();
        Description(b => b
            .Accepts<LatestChangesWidgetEndpointRequest>()
            .Produces<string>(200, "image/svg+xml"));
    }

    private static string? GetPrettyDate(DateTimeOffset d)
    {
        TimeSpan s = DateTimeOffset.UtcNow.Subtract(d);
        int dayDiff = (int)s.TotalDays;
        int secDiff = (int)s.TotalSeconds;

        return dayDiff switch
        {
            < 0 or >= 31 => $"{Math.Ceiling((double)dayDiff / 7)} weeks ago",
            // Less than one minute ago.
            0 when secDiff < 60 => "just now",
            // Less than 2 minutes ago.
            0 when secDiff < 120 => "1 minute ago",
            // Less than one hour ago.
            0 when secDiff < 3600 => $"{Math.Floor((double)secDiff / 60)} minutes ago",
            // Less than 2 hours ago.
            0 when secDiff < 7200 => "1 hour ago",
            // Less than one day ago.
            0 when secDiff < 86400 => $"{Math.Floor((double)secDiff / 3600)} hours ago",
            // Handle previous days.
            1 => "yesterday",
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            _ => dayDiff switch
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            {
                < 7 => $"{dayDiff} days ago",
                < 31 => $"{Math.Ceiling((double)dayDiff / 7)} weeks ago"
            }
        };
    }

    public override async Task HandleAsync(LatestChangesWidgetEndpointRequest req, CancellationToken ct)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        using MemoryStream responseStream = new();
        int commitsLimit = req.MaxCommits!.Value;
        const int expiresSeconds = 60;

        Colour foregroundColor = Colour.FromCSSString(req.ForegroundColour) ?? Colours.Black;

        IReadOnlyList<GitHubCommit>? recentCommits =
            await gitHubApiService.GetRecentPublicCommits(req.Username, req.Repository, commitsLimit);

        // Repository is private or failed to load commits
        if (recentCommits is null || !recentCommits.Any())
        {
            Page pageNotFound = new(400, 25);
            Graphics graphics = pageNotFound.Graphics;

            FontFamily family = FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica);
            Font font = new(family, req.BaseFontSize!.Value);
            Point position = new(5, 5);

            const string text = "Oops, no data available!";

            graphics.FillText(position, text, font, foregroundColor);

            pageNotFound.SaveAsSVG(responseStream);
            responseStream.Position = 0;

            SetCacheControl(expiresSeconds, now);

            await SendStreamAsync(responseStream, contentType: "image/svg+xml", cancellation: ct);
            return;
        }

        DateTimeOffset lastChange = recentCommits[0].Commit.Committer.Date;

        StringBuilder commitsTable = new();
        commitsTable.AppendLine(
            """
            +--------------------------+------------------------------------------------------------------------------------------------------+
            | Date                     | Message                                                                                              |
            +==========================+======================================================================================================+
            """
        );

        foreach (GitHubCommit commit in recentCommits)
        {
            string? message = NewlineRegex().Replace(commit.Commit.Message, "<br>");
            string date = commit.Commit.Author.Date.ToString("s", CultureInfo.InvariantCulture).Replace("T", " ");
            commitsTable.AppendLine($"| {date}      | {message.Truncate(140)}");
            commitsTable.AppendLine(
                "+--------------------------+------------------------------------------------------------------------------------------------------+"
            );
        }

        string commitsSnippet = commitsLimit == 1
            ? "one commit"
            : $"{commitsLimit} commits";

        string markdownSource = $"""
                                 ## Last change

                                 {lastChange.ToString("s", CultureInfo.InvariantCulture)} ({GetPrettyDate(lastChange)})

                                 ## Recent commits

                                 Limiting to last {commitsSnippet}:

                                 {commitsTable}

                                 This data is refreshed once per hour.
                                 """;

        Colour bgFallback = Colour.WithAlpha(Colours.White, 0 /* full transparency */);

        MarkdownRenderer renderer = new()
        {
            Margins = new Margins(0, 0, 0, 0),
            ForegroundColour = foregroundColor,
            BackgroundColour =
                req.BackgroundColour is null
                    ? bgFallback
                    : Colour.FromCSSString(req.BackgroundColour) ?? bgFallback,
            TableVAlign = MarkdownRenderer.VerticalAlignment.Top,
            BaseFontSize = req.BaseFontSize!.Value
        };

        Page page = renderer.RenderSinglePage(markdownSource, req.Width!.Value, out _, out _);

        page.SaveAsSVG(responseStream);
        responseStream.Position = 0;

        SetCacheControl(expiresSeconds, now);

        await SendStreamAsync(responseStream, contentType: "image/svg+xml", cancellation: ct);
    }

    private void SetCacheControl(int expiresSeconds, DateTimeOffset now)
    {
        // cache control, otherwise GitHub etc. will not request an update for like a day or so
        HttpContext.Response.Headers.CacheControl =
            new StringValues($"max-age={expiresSeconds}, s-maxage={expiresSeconds}");
        HttpContext.Response.Headers.Age = new StringValues("0");
        HttpContext.Response.Headers.Date = new StringValues(now.ToString("R"));
        HttpContext.Response.Headers.LastModified = new StringValues(now.ToString("R"));
        HttpContext.Response.Headers.Expires = new StringValues(now.AddSeconds(expiresSeconds).ToString("R"));
    }
}