using Movies.WebForms.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Movies.WebForms
{
    public partial class _Default : System.Web.UI.Page
    {
        private static readonly HttpClient HttpClient =
            new HttpClient();

        private const string MoviesSessionKey =
            "Movies";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(LoadMoviesAsync));
            }
        }

        private async Task LoadMoviesAsync()
        {
            ErrorLabel.Text = string.Empty;

            try
            {
                string apiUrl =
                    ConfigurationManager
                        .AppSettings["MoviesApiUrl"];

                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    throw new ConfigurationErrorsException(
                        "MoviesApiUrl is not configured.");
                }

                using (HttpResponseMessage response =
                    await HttpClient.GetAsync(apiUrl))
                {
                    response.EnsureSuccessStatusCode();

                    string json =
                        await response.Content.ReadAsStringAsync();

                    List<Movie> movies =
                        JsonConvert.DeserializeObject<List<Movie>>(json)
                        ?? new List<Movie>();

                    Session[MoviesSessionKey] = movies;

                    MovieGrid.DataSource = movies;
                    MovieGrid.DataBind();
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorLabel.Text = "The movies API could not be reached. " + Server.HtmlEncode(ex.Message);
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = "The movie data could not be loaded. " + Server.HtmlEncode(ex.Message);
            }
        }

            protected void MovieGrid_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if(e.CommandName != "SelectedMovie")
            {
                return;
            }

            int rowIndex;

            if (!int.TryParse(
                    e.CommandArgument.ToString(),
                    out rowIndex))
            {
                ErrorLabel.Text =
                    "The selected row was invalid.";

                return;
            }

            List<Movie> movies =
                Session[MoviesSessionKey] as List<Movie>;

            if (movies == null ||
                rowIndex < 0 ||
                rowIndex >= movies.Count)
            {
                ErrorLabel.Text =
                    "The selected movie could not be found.";
                return;
            }

            Movie selectedMovie =
                movies[rowIndex];

            SelectedMovieTextBox.Text =
                string.Format(
                    "Movie ID: {0}{4}" +
                    "Title: {1}{4}" +
                    "Rating: {2}{4}" +
                    "Release Year: {3}",
                    selectedMovie.MovieID,
                    selectedMovie.MovieTitle,
                    selectedMovie.MovieRating,
                    selectedMovie.ReleaseYear,
                    Environment.NewLine);
        }
    }
}