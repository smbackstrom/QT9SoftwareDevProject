namespace Movies.Api.DTO
{
    public class MovieAdminResponse
    {
        public int MovieID { get; set; }
        public string? MovieTitle { get; set; }
        public string? MovieRating { get; set; }
        public int ReleaseYear { get; set; }
        public bool IsActive { get; set; }
    }
}
