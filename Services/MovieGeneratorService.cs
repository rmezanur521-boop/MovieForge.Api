using System.Text.Json;
using Bogus;
using MovieForge.Api.Models;

namespace MovieForge.Api.Services;

public class MovieGeneratorService : IMovieGeneratorService
{
    private readonly Dictionary<string, LocaleData> _locales;

    public MovieGeneratorService(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "Locales", "locales.json");
        var json = File.ReadAllText(path);
        _locales = JsonSerializer.Deserialize<Dictionary<string, LocaleData>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new();
    }

    public List<Movie> GenerateBatch(MovieRequestParams request)
    {
        if (!_locales.TryGetValue(request.Locale, out var locale))
            locale = _locales["en-US"];

        var movies = new List<Movie>();

        for (var i = 0; i < request.PageSize; i++)
        {
            var globalIndex = request.Page * request.PageSize + i + 1;

            var coreRnd = new Randomizer(CombineSeed(request.Seed, globalIndex, 0));
            var likesRnd = new Randomizer(CombineSeed(request.Seed, globalIndex, 1));
            var reviewsRnd = new Randomizer(CombineSeed(request.Seed, globalIndex, 2));

            var title = $"{coreRnd.ListItem(locale.TitleAdjectives)} {coreRnd.ListItem(locale.TitleNouns)}";
            var actors = new List<string>
            {
                $"{coreRnd.ListItem(locale.FirstNames)} {coreRnd.ListItem(locale.LastNames)}",
                $"{coreRnd.ListItem(locale.FirstNames)} {coreRnd.ListItem(locale.LastNames)}"
            };
            var year = coreRnd.Int(1980, 2026);
            var genre = coreRnd.ListItem(locale.Genres);

            var likes = GenerateProbabilisticCount(request.AvgLikes, likesRnd);

            var reviewCount = GenerateProbabilisticCount(request.AvgReviews, reviewsRnd);
            var reviews = new List<string>();
            for (var r = 0; r < reviewCount; r++)
                reviews.Add(reviewsRnd.ListItem(locale.ReviewPhrases));

            movies.Add(new Movie
            {
                Index = globalIndex,
                Title = title,
                Actors = actors,
                Year = year,
                Genre = genre,
                Likes = likes,
                Reviews = reviews
            });
        }

        return movies;
    }

    private static int GenerateProbabilisticCount(double average, Randomizer rnd)
    {
        var baseCount = (int)Math.Floor(average);
        var fraction = average - baseCount;
        var extra = rnd.Double() < fraction ? 1 : 0;
        return baseCount + extra;
    }

    private static int CombineSeed(long seed, int index, int salt)
    {
        unchecked
        {
            return (int)(seed * 397 + index * 31 + salt);
        }
    }
}