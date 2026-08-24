using Microsoft.AspNetCore.Mvc;
using MovieForge.Api.Models;
using MovieForge.Api.Services;

namespace MovieForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieGeneratorService _generator;

    public MoviesController(IMovieGeneratorService generator)
    {
        _generator = generator;
    }

    [HttpGet]
    public ActionResult<List<Movie>> Get(
        [FromQuery] long seed,
        [FromQuery] string locale = "en-US",
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10,
        [FromQuery] double avgLikes = 2,
        [FromQuery] double avgReviews = 2)
    {
        if (pageSize <= 0 || pageSize > 100)
            return BadRequest("pageSize must be between 1 and 100.");

        if (avgLikes < 0 || avgLikes > 10 || avgReviews < 0 || avgReviews > 10)
            return BadRequest("avgLikes and avgReviews must be between 0 and 10.");

        var request = new MovieRequestParams
        {
            Seed = seed,
            Locale = locale,
            Page = page,
            PageSize = pageSize,
            AvgLikes = avgLikes,
            AvgReviews = avgReviews
        };

        return Ok(_generator.GenerateBatch(request));
    }
}