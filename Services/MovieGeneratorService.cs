using System.Text.Json;
using Bogus;
using MovieForge.Api.Models;

namespace MovieForge.Api.Services;

public class MovieGeneratorService : IMovieGeneratorService
{
    private readonly Dictionary<string, LocaleData> _locales;

    private static readonly string[] AnimationStyles = { "slide-fade", "zoom-pulse", "typewriter", "flip-reveal" };
    private static readonly string[] ColorPalette =
    {
    "#e63946", "#457b9d", "#2a9d8f", "#f4a261", "#8338ec", "#ffb703"
    };
    private static readonly string[] MusicTracks = { "15_1.mp3", "15_2.mp3", "15_3.mp3" };
    private static readonly string[] CameraStyles = { "zoomIn", "zoomOut", "panLeft", "panRight" };
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
            var trailerRnd = new Randomizer(CombineSeed(request.Seed, globalIndex, 3));

            var title = $"{coreRnd.ListItem(locale.TitleAdjectives)} {coreRnd.ListItem(locale.TitleNouns)}";
            var actors = new List<string>
            {
                $"{coreRnd.ListItem(locale.FirstNames)} {coreRnd.ListItem(locale.LastNames)}",
                $"{coreRnd.ListItem(locale.FirstNames)} {coreRnd.ListItem(locale.LastNames)}"
            };
            var year = coreRnd.Int(1980, 2026);
            var genre = coreRnd.ListItem(locale.Genres);
            var runtime = coreRnd.Int(80, 150);

            var likes = GenerateProbabilisticCount(request.AvgLikes, likesRnd);

            var reviewCount = GenerateProbabilisticCount(request.AvgReviews, reviewsRnd);
            var reviews = new List<ReviewItem>();
            for (var r = 0; r < reviewCount; r++)
                reviews.Add(GenerateReview(reviewsRnd, locale, year));

            var trailer = GenerateTrailerSpec(trailerRnd);

            movies.Add(new Movie
            {
                Index = globalIndex,
                Title = title,
                Actors = actors,
                Year = year,
                Genre = genre,
                RuntimeMinutes = runtime,
                Likes = likes,
                Reviews = reviews,
                Trailer = trailer
            });
        }

        return movies;
    }

    private static ReviewItem GenerateReview(Randomizer rnd, LocaleData locale, int movieYear)
    {
        var reviewer = $"{rnd.ListItem(locale.FirstNames)} {rnd.ListItem(locale.LastNames)}";
        var rating = rnd.Int(1, 5);
        var text = rnd.ListItem(locale.ReviewPhrases);
        var day = rnd.Int(1, 28);
        var month = rnd.Int(1, 12);
        var reviewYear = movieYear + rnd.Int(0, 3);
        var date = new DateTime(reviewYear, month, day).ToString("MMM d, yyyy");

        return new ReviewItem
        {
            ReviewerName = reviewer,
            Rating = rating,
            Text = text,
            Date = date
        };
    }

    private static TrailerSpec GenerateTrailerSpec(Randomizer rnd)
    {
        var raySpin = rnd.Double(0.3, 0.9) * (rnd.Bool() ? 1 : -1);

        return new TrailerSpec
        {
            AnimationStyle = rnd.ListItem(AnimationStyles),
            PrimaryColor = rnd.ListItem(ColorPalette),
            SecondaryColor = rnd.ListItem(ColorPalette),
            DurationMs = rnd.Int(5000, 9000),
            MusicTrack = rnd.ListItem(MusicTracks),
            CameraStyle = rnd.ListItem(CameraStyles),
            RayCount = rnd.Int(8, 14),
            RaySpin = raySpin,
            SweepAngleDeg = rnd.Double(20, 70)
        };
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