using Microsoft.Data.SqlClient;
using Movies.Api.Models;

namespace Movies.Api.Repository
{
    public class MovieRepository
    {
        private readonly string _connectionString;
        public MovieRepository(IConfiguration configuration)
        {
            /* 
             * For this take-home assignment, the connection string is stored in appsettings.json
             * to simplify setup and execution.
             *
             * In a production Azure environment, sensitive configuration such as connection
             * strings should be stored securely in Azure Key Vault (or another approved
             * secrets management solution) rather than in source-controlled configuration files.
             */
            _connectionString =
                configuration.GetConnectionString("MoviesDatabase")
                ?? throw new InvalidOperationException(
                    "The QT9 Movies Database connection string is not configured.");
        }

        public async Task<IReadOnlyList<Movie>> GetMoviesAsync()
        {
            var movies = new List<Movie>();
            
            const string sql = """
                SELECT MovieID
                    ,MovieTitle
                    ,MovieRating
                    ,ReleaseYear
                FROM dbo.tblMovie
                ORDER BY MovieID
                """;
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            /*
             * Column ordinals are resolved once before reading the result set to avoid
             * repeatedly looking up column positions by name.
             */
            int movieIdOrdinal = reader.GetOrdinal("MovieID");
            int movieTitleOrdinal = reader.GetOrdinal("MovieTitle");
            int movieRatingOrdinal = reader.GetOrdinal("MovieRating");
            int releaseYearOrdinal = reader.GetOrdinal("ReleaseYear");

            while (await reader.ReadAsync())
            {
                movies.Add(new Movie
                {
                    MovieID = reader.GetInt32(movieIdOrdinal)
                    , MovieTitle = reader.IsDBNull(movieTitleOrdinal) ? null : reader.GetString(movieTitleOrdinal)
                    , MovieRating = reader.IsDBNull(movieRatingOrdinal) ? null : reader.GetString(movieRatingOrdinal)
                    , ReleaseYear = reader.IsDBNull(releaseYearOrdinal) ? null : reader.GetInt32(releaseYearOrdinal)
                });                
            }
            return movies;
        }
    }
}
