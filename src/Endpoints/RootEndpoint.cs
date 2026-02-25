using FastEndpoints;

namespace GitHubLastChangeWidget.Endpoints;

/// <summary>
///     Serves the root landing page that explains the service and links to the repository.
/// </summary>
internal sealed class RootEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
        Description(b => b
            .Produces<string>(200, "text/html"));
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        string html = """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>GitHubLastChangeWidget</title>
                <style>
                    :root {
                        --bg: #0d1117;
                        --bg-card: #161b22;
                        --border: #30363d;
                        --text: #c9d1d9;
                        --text-muted: #8b949e;
                        --accent: #58a6ff;
                        --accent-hover: #79b8ff;
                    }
                    @media (prefers-color-scheme: light) {
                        :root {
                            --bg: #ffffff;
                            --bg-card: #f6f8fa;
                            --border: #d0d7de;
                            --text: #24292f;
                            --text-muted: #57606a;
                            --accent: #0969da;
                            --accent-hover: #0550ae;
                        }
                    }
                    * { box-sizing: border-box; }
                    body {
                        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Noto Sans', Helvetica, Arial, sans-serif;
                        background: var(--bg);
                        color: var(--text);
                        margin: 0;
                        padding: 2rem 1rem;
                        line-height: 1.6;
                        min-height: 100vh;
                        display: flex;
                        flex-direction: column;
                        align-items: center;
                    }
                    main {
                        max-width: 640px;
                        width: 100%;
                    }
                    h1 {
                        font-size: 2rem;
                        font-weight: 600;
                        margin: 0 0 0.5rem;
                        letter-spacing: -0.02em;
                    }
                    .subtitle {
                        color: var(--text-muted);
                        font-size: 1.125rem;
                        margin-bottom: 2rem;
                    }
                    .card {
                        background: var(--bg-card);
                        border: 1px solid var(--border);
                        border-radius: 12px;
                        padding: 1.5rem;
                        margin-bottom: 1.5rem;
                    }
                    .card h2 {
                        font-size: 1rem;
                        font-weight: 600;
                        margin: 0 0 0.75rem;
                        color: var(--text);
                    }
                    .card p {
                        margin: 0;
                        color: var(--text-muted);
                        font-size: 0.9375rem;
                    }
                    .links {
                        display: flex;
                        flex-wrap: wrap;
                        gap: 1rem;
                        margin-top: 2rem;
                    }
                    a {
                        color: var(--accent);
                        text-decoration: none;
                        font-weight: 500;
                        display: inline-flex;
                        align-items: center;
                        gap: 0.35rem;
                    }
                    a:hover {
                        color: var(--accent-hover);
                        text-decoration: underline;
                    }
                    .btn {
                        background: var(--accent);
                        color: #fff !important;
                        padding: 0.5rem 1rem;
                        border-radius: 6px;
                        text-decoration: none !important;
                    }
                    .btn:hover {
                        background: var(--accent-hover);
                        text-decoration: none !important;
                    }
                    code {
                        background: var(--bg-card);
                        border: 1px solid var(--border);
                        border-radius: 4px;
                        padding: 0.2em 0.4em;
                        font-size: 0.875em;
                    }
                    .demo-img {
                        max-width: 100%;
                        border-radius: 8px;
                        border: 1px solid var(--border);
                        margin-top: 0.5rem;
                    }
                </style>
            </head>
            <body>
                <main>
                    <h1>GitHubLastChangeWidget</h1>
                    <p class="subtitle">Web service that generates an SVG of latest GitHub repository activity.</p>

                    <div class="card">
                        <h2>What is this?</h2>
                        <p>This service generates an SVG badge summarizing the recent commit activity for any public GitHub repository. Perfect for README files, documentation sites, or dashboards.</p>
                    </div>

                    <div class="card">
                        <h2>Usage</h2>
                        <p>Use this URL pattern to get a widget for any public repository:</p>
                        <p><code>/widgets/github/{username}/{repository}/changes/latest</code></p>
                        <p>Example: <a href="/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest">/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest</a></p>
                        <p>Customize colors and other options via <a href="/swagger">query parameters</a>.</p>
                    </div>

                    <div class="card">
                        <h2>Demo</h2>
                        <img src="/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest" alt="Repository activity widget" class="demo-img" width="830">
                    </div>

                    <div class="links">
                        <a href="https://github.com/nefarius/GitHubLastChangeWidget" class="btn">View on GitHub</a>
                        <a href="/swagger">API Documentation</a>
                    </div>
                </main>
            </body>
            </html>
            """;

        return Send.StringAsync(html, contentType: "text/html", cancellation: ct);
    }
}
