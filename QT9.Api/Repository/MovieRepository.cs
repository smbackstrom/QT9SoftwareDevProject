using Microsoft.Data.SqlClient;
using Movies.Api.Models;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace Movies.Api.Repository
{
    public class MovieRepository
    {
        private readonly string _connectionString;
        public MovieRepository(IConfiguration configuration)
        {
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
                FROM MoviesDB.dbo.tblMovie
                ORDER BY MovieID
                """;
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();
            
            /*added for speed/eficiency with no overhead,
             * not really usefull with this data set but would use it in a production environment.*/
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
