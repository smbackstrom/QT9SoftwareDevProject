using Microsoft.Data.SqlClient;
using Movies.Api.DTO;
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

        public async Task<IReadOnlyList<Movie>> GetMoviesAsync(bool includeInactive)
        {
            var movies = new List<Movie>();

            const string getAllSql = @"
            SELECT MovieID,
                   MovieTitle,
                   MovieRating,
                   ReleaseYear
            FROM dbo.tblMovie
            ORDER BY MovieID";

            const string getActiveSql = @"
            SELECT MovieID,
                   MovieTitle,
                   MovieRating,
                   ReleaseYear
            FROM dbo.tblMovie
            WHERE IsActive = 1
            ORDER BY MovieID";

            string sql = includeInactive ? getAllSql : getActiveSql;

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

        public async Task<Movie> GetMovieAsync(int id)
        {
            var movie = new Movie();
            const string sql = @"
                SELECT MovieID
                    ,MovieTitle
                    ,MovieRating
                    ,ReleaseYear
                FROM dbo.tblMovie
                WHERE IsActive = 1
                AND MovieID = @MovieID;";

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@MovieID", id);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }
            return new Movie
            {
                MovieID = reader.GetInt32(reader.GetOrdinal("MovieID")),
                MovieTitle = reader.IsDBNull(reader.GetOrdinal("MovieTitle")) ? null : reader.GetString(reader.GetOrdinal("MovieTitle")),
                MovieRating = reader.IsDBNull(reader.GetOrdinal("MovieRating")) ? null : reader.GetString(reader.GetOrdinal("MovieRating")),
                ReleaseYear = reader.IsDBNull(reader.GetOrdinal("ReleaseYear")) ? null : reader.GetInt32(reader.GetOrdinal("ReleaseYear"))
            };

        }
        public async Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            const string sql = @"
            INSERT INTO dbo.tblMovie
                (MovieTitle, MovieRating, ReleaseYear)
            OUTPUT INSERTED.MovieID
            VALUES
                (@MovieTitle, @MovieRating, @ReleaseYear);";

            using SqlCommand command = new(sql, connection);

            command.Parameters.AddWithValue("@MovieTitle", request.MovieTitle);
            command.Parameters.AddWithValue("@MovieRating", request.MovieRating);
            command.Parameters.AddWithValue("@ReleaseYear", request.ReleaseYear);

            await connection.OpenAsync();

            int newId = (int)await command.ExecuteScalarAsync();

            return new MovieResponse
            {
                MovieID = newId,
                MovieTitle = request.MovieTitle,
                MovieRating = request.MovieRating,
                ReleaseYear = request.ReleaseYear
            };
        }

        public async Task<bool> UpdateMovieAsync(int id, UpdateMovieRequest request)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            const string sql = @"
            UPDATE dbo.tblMovie
            SET
                MovieTitle = @MovieTitle,
                MovieRating = @MovieRating,
                ReleaseYear = @ReleaseYear
            WHERE MovieID = @MovieID";

            using SqlCommand command = new(sql, connection);

            command.Parameters.AddWithValue("@MovieID", id);
            command.Parameters.AddWithValue("@MovieTitle", request.MovieTitle);
            command.Parameters.AddWithValue("@MovieRating", request.MovieRating);
            command.Parameters.AddWithValue("@ReleaseYear", request.ReleaseYear);

            await connection.OpenAsync();

            int rows = await command.ExecuteNonQueryAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            const string sql = @"
            UPDATE dbo.tblMovie
            SET IsActive = 0
            WHERE MovieID = @MovieID
            AND IsActive = 1";

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@MovieID", id);

            await connection.OpenAsync();

            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
    }
}
