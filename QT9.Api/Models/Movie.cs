namespace Movies.Api.Models
{
    public sealed class Movie
    {
        public int MovieID { get; set; }
        public string? MovieTitle { get; set; }
        public string? MovieRating { get; set; }
        public int? ReleaseYear { get; set; }
    }
}
