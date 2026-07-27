using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies.Webforms.Models
{
    public class CreateMovieRequest
    {
        public string MovieTitle { get; set; }
        public string MovieRating { get; set; }
        public int ReleaseYear { get; set; }
    }
}