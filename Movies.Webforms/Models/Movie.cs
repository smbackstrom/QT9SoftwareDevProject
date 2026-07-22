using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movies.WebForms.Models
{
    public class Movie
    {       
        public int MovieID { get; set; }
        public string MovieTitle { get; set; }
        public string MovieRating { get; set; }
        public int? ReleaseYear { get; set; }
    }
}