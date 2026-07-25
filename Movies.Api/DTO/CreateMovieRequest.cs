namespace Movies.Api.DTO
{
    public class CreateMovieRequest
    {
        public string MovieTitle { get; set; } = string.Empty;

        public string MovieRating { get; set; } = string.Empty;

        public int ReleaseYear { get; set; }
    }
}
