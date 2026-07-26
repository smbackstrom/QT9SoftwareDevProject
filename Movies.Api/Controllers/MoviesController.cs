using Microsoft.AspNetCore.Mvc;
using Movies.Api.DTO;
using Movies.Api.Models;
using Movies.Api.Repository;

namespace Movies.Api.ControllerBase
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private readonly MovieRepository _repository;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(MovieRepository repository, ILogger<MoviesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<Movie>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<Movie>>> GetMovies(bool includeInactive = false)
        {
            try
            {
                var movies = await _repository.GetMoviesAsync(includeInactive);
                if(includeInactive)
                {
                    var response = movies.Select(m => new MovieAdminResponse
                    {
                        MovieID = m.MovieID,
                        MovieTitle = m.MovieTitle,
                        MovieRating = m.MovieRating,
                        ReleaseYear = m.ReleaseYear,
                        IsActive = m.IsActive
                    });
                    return Ok(response);
                }

                var activeResponse = movies.Select(m => new MovieResponse
                {
                    MovieID = m.MovieID,
                    MovieTitle = m.MovieTitle,
                    MovieRating = m.MovieRating,
                    ReleaseYear = m.ReleaseYear
                });

                return Ok(activeResponse);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while retrieving movies.");

                return Problem(title: "Unable to retrieve movie data.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Movie>> GetMovie(int id) 
        {
            try
            {
                Movie movie = await _repository.GetMovieAsync(id);
                return Ok(movie);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while retrieving movies.");

                return Problem(title: "Unable to retrieve movie data.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MovieResponse>> CreateMovie(CreateMovieRequest request)
        {
            if(request == null)
            {
                return BadRequest();
            }

            MovieResponse movie = await _repository.CreateMovieAsync(request);

            return CreatedAtAction( nameof(GetMovie), new { id = movie.MovieID }, movie);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutMovie(int id, UpdateMovieRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            bool updated = await _repository.UpdateMovieAsync(id, request);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            bool deleted = await _repository.DeleteMovieAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
