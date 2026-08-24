using MovieForge.Api.Models;

namespace MovieForge.Api.Services;

public interface IMovieGeneratorService
{
    List<Movie> GenerateBatch(MovieRequestParams request);
}