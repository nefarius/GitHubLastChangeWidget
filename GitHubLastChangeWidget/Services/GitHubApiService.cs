using Microsoft.Extensions.Caching.Memory;

using Octokit;

namespace GitHubLastChangeWidget.Services;

/// <summary>
///     Abstracts calls to GitHub REST API and caches them to avoid hitting rate limits.
/// </summary>
internal sealed class GitHubApiService
{
    private readonly IHostEnvironment _environment;
    private readonly GitHubClient _gitHubClient;
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    ///     Abstracts calls to GitHub REST API and caches them to avoid hitting rate limits.
    /// </summary>
    public GitHubApiService(IMemoryCache memoryCache, IHostEnvironment environment, IConfiguration configuration)
    {
        _memoryCache = memoryCache;
        _environment = environment;

        _gitHubClient = new GitHubClient(new ProductHeaderValue("GitHubLastChangeWidget"));

        string? token = configuration.GetSection("GitHub:Token").Get<string>();

        if (!string.IsNullOrEmpty(token))
        {
            _gitHubClient.Credentials = new Credentials(token, AuthenticationType.Bearer);
        }
    }

    public async Task<IReadOnlyList<GitHubCommit>?> GetRecentPublicCommits(string owner, string name, int maxCount = 5)
    {
        string key = $"{nameof(GitHubApiService)}-commits-{owner}/{name}+{maxCount}";

        if (!_environment.IsDevelopment())
        {
            if (_memoryCache.TryGetValue(key, out List<GitHubCommit>? cached))
            {
                return cached;
            }
        }

        List<GitHubCommit> empty = Enumerable.Empty<GitHubCommit>().ToList();

        try
        {
            Repository? repository = await _gitHubClient.Repository.Get(owner, name);

            if (repository.Private)
            {
                if (!_environment.IsDevelopment())
                {
                    _memoryCache.Set(
                        key,
                        empty,
                        new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }
                    );
                }

                return empty;
            }

            IReadOnlyList<GitHubCommit>? recentCommits = await _gitHubClient.Repository.Commit.GetAll(owner, name,
                new ApiOptions { StartPage = 1, PageCount = 1, PageSize = maxCount });

            if (!_environment.IsDevelopment())
            {
                _memoryCache.Set(
                    key,
                    recentCommits.ToList(),
                    new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }
                );
            }

            return recentCommits;
        }
        catch (NotFoundException)
        {
            if (!_environment.IsDevelopment())
            {
                _memoryCache.Set(
                    key,
                    empty,
                    new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }
                );
            }

            return empty;
        }
    }

    public async Task<IReadOnlyList<Activity>?> GetRecentActivityEvents(string owner, string name, int maxCount = 5)
    {
        string key = $"{nameof(GitHubApiService)}-events-{owner}/{name}";

        if (!_environment.IsDevelopment())
        {
            if (_memoryCache.TryGetValue(key, out List<Activity>? cached))
            {
                return cached;
            }
        }

        IReadOnlyList<Activity>? recentEvents = await _gitHubClient.Activity.Events.GetAllForRepository(owner, name,
            new ApiOptions { StartPage = 1, PageCount = 1, PageSize = maxCount });

        if (!_environment.IsDevelopment())
        {
            _memoryCache.Set(
                key,
                recentEvents.ToList(),
                new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }
            );
        }

        return recentEvents;
    }
}