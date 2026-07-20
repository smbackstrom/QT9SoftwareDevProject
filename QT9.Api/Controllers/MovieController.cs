using Microsoft.AspNetCore.Mvc;
using Movies.Api.Models;
using Movies.Api.Repository;
using System.Threading;

namespace Movies.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private readonly MovieRepository _repository;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(
            MovieRepository repository,
            ILogger<MoviesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(
        typeof(IReadOnlyList<Movie>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<Movie>>> GetMovies()
        {
            try
            {
                IReadOnlyList<Movie> movies =
                    await _repository.GetMoviesAsync();

                return Ok(movies);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An error occurred while retrieving movies.");

                return Problem(
                    title: "Unable to retrieve movie data.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
