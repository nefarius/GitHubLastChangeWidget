# GitHubLastChangeWidget

[![.NET](https://github.com/nefarius/GitHubLastChangeWidget/actions/workflows/build.yml/badge.svg)](https://github.com/nefarius/GitHubLastChangeWidget/actions/workflows/build.yml)

WIP 🔥

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
  <source media="(prefers-color-scheme: dark)" srcset="https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest?foregroundColour=%23C4D1DE">
  <source media="(prefers-color-scheme: light)" srcset="https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest">
  <img alt="Repository activity" src="https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest">
</picture>
```

### Material for MkDocs

```markdown
![Repository activity](https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest#only-light)
![Repository activity](https://ghstats.api.nefarius.systems/widgets/github/nefarius/GitHubLastChangeWidget/changes/latest?foregroundColour=%23b5b3b0#only-dark)
```

## Resources

- [Specifying the theme an image is shown to](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#specifying-the-theme-an-image-is-shown-to)
- [Light and dark mode](https://squidfunk.github.io/mkdocs-material/reference/images/#light-and-dark-mode)
- [C# Pretty Date Format (Hours or Minutes Ago)](https://thedeveloperblog.com/c-sharp/pretty-date)
- [VectSharp: a light library for C# vector graphics](https://giorgiobianchini.com/VectSharp/)
- [Octokit - GitHub API Client Library for .NET](https://github.com/octokit/octokit.net)
