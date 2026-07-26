using Movies.WebForms.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
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

                // If the user wants inactive movies too, append the query string.
                if (chkShowInactive.Checked)
                {
                    apiUrl += "?includeInactive=true";
                }

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

        protected async void MovieGrid_RowCommandAsync(object sender, GridViewCommandEventArgs e)
        {
            int movieId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "EditMovie":
                    //await LoadMovieAsync(movieId);
                    break;

                case "DeleteMovie":
                    bool deleted = await DeleteMovieAsync(movieId);

                    if (deleted)
                    {
                        await LoadMoviesAsync();
                    }

                    break;
            }
        }

        protected async void chkShowInactive_ServerChange(object sender, EventArgs e)
        {
            await LoadMoviesAsync();
        }

        private async Task<bool> DeleteMovieAsync(int movieId)
        {
            ErrorLabel.Text = string.Empty;

            try
            {
                string apiUrl =
                    ConfigurationManager.AppSettings["MoviesApiUrl"];

                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    throw new ConfigurationErrorsException(
                        "MoviesApiUrl is not configured.");
                }

                string deleteUrl =
                    $"{apiUrl.TrimEnd('/')}/{movieId}";

                using (HttpResponseMessage response =
                       await HttpClient.DeleteAsync(deleteUrl))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        ErrorLabel.Text =
                            "The selected movie was not found or is already inactive.";

                        return false;
                    }

                    response.EnsureSuccessStatusCode();

                    return true;
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorLabel.Text =
                    "The movies API could not be reached. " +
                    Server.HtmlEncode(ex.Message);

                return false;
            }
            catch (Exception ex)
            {
                ErrorLabel.Text =
                    "The movie could not be deleted. " +
                    Server.HtmlEncode(ex.Message);

                return false;
            }
        }
    }
}