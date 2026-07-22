using Movies.WebForms.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Movies.WebForms
{
    public partial class _Default : System.Web.UI.Page
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        private const string MoviesSessionKey = "Movies";
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
                /*
                 * The API URL is stored in Web.config so it can be changed by environment without modifying application code. 
                 * 
                 *In production, the value is commonly supplied through a Web.config transform or deployment pipeline, allowing each environment (Development, QA, Production) to use a different API endpoint.
                 */
                string apiUrl = ConfigurationManager.AppSettings["MoviesApiUrl"];

                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    throw new ConfigurationErrorsException("MoviesApiUrl is not configured.");
                }

                using (HttpResponseMessage response = await HttpClient.GetAsync(apiUrl))
                {
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();

                    List<Movie> movies = JsonConvert.DeserializeObject<List<Movie>>(json) ?? new List<Movie>();

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
            ErrorLabel.Text = string.Empty;
            if (e.CommandName != "SelectedMovie")
            {
                return;
            }

            int rowIndex;

            if (!int.TryParse(e.CommandArgument?.ToString(),out rowIndex))
            {
                ErrorLabel.Text = "The selected row was invalid.";
                return;
            }

            List<Movie> movies =
                Session[MoviesSessionKey] as List<Movie>;

            if (movies == null || rowIndex < 0 || rowIndex >= movies.Count)
            {
                ErrorLabel.Text = "The selected movie could not be found.";
                return;
            }

            Movie selectedMovie = movies[rowIndex];

            SelectedMovieTextBox.Text =
            $"Movie ID: {selectedMovie.MovieID}{Environment.NewLine}" +
            $"Title: {selectedMovie.MovieTitle}{Environment.NewLine}" +
            $"Rating: {selectedMovie.MovieRating}{Environment.NewLine}" +
            $"Release Year: {selectedMovie.ReleaseYear}";
        }
    }
}