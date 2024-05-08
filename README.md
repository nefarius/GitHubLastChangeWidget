# GitHubLastChangeWidget

[![.NET](https://github.com/nefarius/GitHubLastChangeWidget/actions/workflows/build.yml/badge.svg)](https://github.com/nefarius/GitHubLastChangeWidget/actions/workflows/build.yml)

Webservice that generates an SVG of latest GitHub repository activity.

## Features

- Generates an SVG summary of the recent commit activity for a GitHub repository
- In-memory caching of GitHub API responses for an hour to not trigger rate limits
    - Optional [personal access token (classic)](https://github.com/settings/tokens) can be configured (`GitHub:Token`)
      to overcome GitHub API rate limits
- Customization of basic attributes via [query parameters](https://ghstats.api.nefarius.systems/swagger)
- Light and dark mode color schemes can be configured
  client-side ([see examples](#embed-with-support-for-color-scheme-preference))

## Limitations

- Private repositories are not supported for security reasons
- Date rendering is done server-side so the Browser local time can not be taken into consideration (all dates are in
  UTC)

## Demo

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest?foregroundColour=%23C4D1DE">
  <source media="(prefers-color-scheme: light)" srcset="https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest">
  <img alt="Repository activity" src="https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest">
</picture>

## Embed with support for color scheme preference

### GitHub-flavoured Markdown

```html

<picture>
    <source media="(prefers-color-scheme: dark)"
            srcset="https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest?foregroundColour=%23C4D1DE">
    <source media="(prefers-color-scheme: light)"
            srcset="https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest">
    <img alt="Repository activity"
         src="https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest">
</picture>
```

### Material for MkDocs

```markdown
![Repository activity](https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest#only-light)
![Repository activity](https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest?foregroundColour=%23b5b3b0#only-dark)
```

## Public instance

I'm hosting an instance of this project
at [`https://ghstats.api.nefarius.systems/`](https://ghstats.api.nefarius.systems/) which you can
consume:

```text
https://ghstats.api.nefarius.systems/widgets/github/{username}/{repository}/changes/latest
```

Like any other of my public web services it only logs an absolute minimal amount required for debugging purposes and
I'll never sell your data out to the big bois 😉

If you like this idea and want to keep my public instance happy, up and
running [consider making a donation](https://docs.nefarius.at/Community-Support/) 💸

## Resources

- [Specifying the theme an image is shown to](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#specifying-the-theme-an-image-is-shown-to)
- [Light and dark mode](https://squidfunk.github.io/mkdocs-material/reference/images/#light-and-dark-mode)
- [C# Pretty Date Format (Hours or Minutes Ago)](https://thedeveloperblog.com/c-sharp/pretty-date)
- [VectSharp: a light library for C# vector graphics](https://giorgiobianchini.com/VectSharp/)
- [Octokit - GitHub API Client Library for .NET](https://github.com/octokit/octokit.net)
