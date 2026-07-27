using Movies.Webforms.Models;
using Movies.WebForms.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
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
                    EditorTitleLabel.Text = "Edit Movie";
                    SaveMovieButton.Text = "Update";
                    MovieEditorPanel.Visible = true;
                    await LoadMovieAsync(movieId);
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
                string apiUrl = ConfigurationManager.AppSettings["MoviesApiUrl"];

                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    throw new ConfigurationErrorsException("MoviesApiUrl is not configured.");
                }

                string deleteUrl = $"{apiUrl.TrimEnd('/')}/{movieId}";

                using (HttpResponseMessage response = await HttpClient.DeleteAsync(deleteUrl))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        ErrorLabel.Text = "The selected movie was not found or is already inactive.";

                        return false;
                    }

                    response.EnsureSuccessStatusCode();

                    return true;
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorLabel.Text ="The movies API could not be reached. " + Server.HtmlEncode(ex.Message);

                return false;
            }
            catch (Exception ex)
            {
                ErrorLabel.Text ="The movie could not be deleted. " + Server.HtmlEncode(ex.Message);

                return false;
            }
        }

        private async Task LoadMovieAsync(int movieId)
        {
            ErrorLabel.Text = string.Empty;

            try
            {
                string apiUrl = ConfigurationManager.AppSettings["MoviesApiUrl"];

                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    throw new ConfigurationErrorsException("MoviesApiUrl is not configured.");
                }

                string movieUrl = $"{apiUrl.TrimEnd('/')}/{movieId}";

                using (HttpResponseMessage response = await HttpClient.GetAsync(movieUrl))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        ErrorLabel.Text = "The selected movie was not found.";
                        return;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        ErrorLabel.Text =
                            $"Unable to retrieve movie: " +
                            $"{(int)response.StatusCode} " +
                            $"{response.ReasonPhrase}. " +
                            Server.HtmlEncode(responseBody);

                        return;
                    }

                    string json = await response.Content.ReadAsStringAsync();

                    Movie movie = JsonConvert.DeserializeObject<Movie>(json);

                    if (movie == null)
                    {
                        ErrorLabel.Text = "The API returned an invalid movie response.";
                        return;
                    }

                    SelectedMovieId.Value = movie.MovieID.ToString();

                    MovieTitleTextBox.Text = movie.MovieTitle;

                    MovieRatingDropDown.Text = movie.MovieRating.Trim();

                    ReleaseYearTextBox.Text = movie.ReleaseYear.ToString();
                    
                    IsActiveCheckBox.Checked = movie.IsActive;

                    SaveMovieButton.Text = "Update Movie";
                    CancelEditButton.Visible = true;
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorLabel.Text =
                    "Unable to connect to the API. " + Server.HtmlEncode(ex.Message);
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = "The movie could not be loaded. " + Server.HtmlEncode(ex.Message);
            }
        }

        protected void AddMovieButton_Click(object sender, EventArgs e)
        {
            ClearMovieForm();

            EditorTitleLabel.Text = "Add Movie";
            SaveMovieButton.Text = "Save";

            MovieEditorPanel.Visible = true;
        }

        protected void CancelMovieForm_Click(object sender, EventArgs e)
        {
            ClearMovieForm();
            MovieEditorPanel.Visible = false;
        }

        private void ClearMovieForm()
        {
            // Clear the selected movie
            SelectedMovieId.Value = string.Empty;

            // Clear the input controls
            MovieTitleTextBox.Text = string.Empty;
            MovieRatingDropDown.SelectedIndex = 0;   // or SelectedValue = "G"
            ReleaseYearTextBox.Text = string.Empty;

            // Reset labels/buttons
            EditorTitleLabel.Text = "Add Movie";
            SaveMovieButton.Text = "Save";

            // Hide the editor
            MovieEditorPanel.Visible = false;

            // Clear any messages
            ErrorLabel.Text = string.Empty;
        }

        protected async void SaveButton_Click(object sender, EventArgs e)
        {
            bool success;

            if (string.IsNullOrEmpty(SelectedMovieId.Value))
            {
                success = await CreateMovieAsync();
            }
            else
            {
                success = await UpdateMovieAsync(int.Parse(SelectedMovieId.Value));
            }

            if (success)
            {
                ClearMovieForm();
                MovieEditorPanel.Visible = false;
                await LoadMoviesAsync();
            }
        }

        private async Task<bool> CreateMovieAsync()
        {
            ErrorLabel.Text = string.Empty;

            try
            {
                string apiUrl = ConfigurationManager.AppSettings["MoviesApiUrl"];

                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    ErrorLabel.Text = "MoviesApiUrl is not configured.";
                    return false;
                }

                if (!int.TryParse(ReleaseYearTextBox.Text.Trim(), out int releaseYear))
                {
                    ErrorLabel.Text = "Release Year must be a valid number.";
                    return false;
                }

                var request = new CreateMovieRequest
                {
                    MovieTitle = MovieTitleTextBox.Text.Trim(),
                    MovieRating = MovieRatingDropDown.SelectedValue,
                    ReleaseYear = releaseYear
                };

                string json = JsonConvert.SerializeObject(request);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    using (HttpResponseMessage response = await HttpClient.PostAsync(apiUrl, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return true;
                        }

                        string responseBody = await response.Content.ReadAsStringAsync();

                        ErrorLabel.Text =
                            $"Unable to create movie. " +
                            $"{(int)response.StatusCode} " +
                            $"{response.ReasonPhrase}";

                        if (!string.IsNullOrWhiteSpace(responseBody))
                        {
                            ErrorLabel.Text += "<br/>" + Server.HtmlEncode(responseBody);
                        }

                        return false;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorLabel.Text = "Unable to connect to the Movies API.<br/>" + Server.HtmlEncode(ex.Message);

                return false;
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = "An unexpected error occurred.<br/>" + Server.HtmlEncode(ex.Message);

                return false;
            }
        }

        private async Task<bool> UpdateMovieAsync(int movieId)
        {
            ErrorLabel.Text = string.Empty;

            try
            {
                string apiUrl = ConfigurationManager.AppSettings["MoviesApiUrl"];

                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    ErrorLabel.Text = "MoviesApiUrl is not configured.";
                    return false;
                }

                if (!int.TryParse(
                    ReleaseYearTextBox.Text.Trim(),
                    out int releaseYear))
                {
                    ErrorLabel.Text =
                        "Release Year must be a valid number.";

                    return false;
                }

                var request = new UpdateMovieRequest
                {
                    MovieTitle = MovieTitleTextBox.Text.Trim(),
                    MovieRating = MovieRatingDropDown.SelectedValue,
                    ReleaseYear = releaseYear,
                    IsActive = IsActiveCheckBox.Checked
                };

                string json = JsonConvert.SerializeObject(request);

                string updateUrl = $"{apiUrl.TrimEnd('/')}/{movieId}";

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    using (HttpResponseMessage response = await HttpClient.PutAsync(updateUrl, content))
                    {
                        if (response.StatusCode == HttpStatusCode.NoContent)
                        {
                            return true;
                        }

                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            ErrorLabel.Text = "The selected movie was not found.";

                            return false;
                        }

                        string responseBody = await response.Content.ReadAsStringAsync();

                        ErrorLabel.Text =
                            $"Unable to update movie. " +
                            $"{(int)response.StatusCode} " +
                            $"{response.ReasonPhrase}";

                        if (!string.IsNullOrWhiteSpace(responseBody))
                        {
                            ErrorLabel.Text += "<br/>" + Server.HtmlEncode(responseBody);
                        }

                        return false;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorLabel.Text = "Unable to connect to the Movies API.<br/>" + Server.HtmlEncode(ex.Message);

                return false;
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = "An unexpected error occurred.<br/>" + Server.HtmlEncode(ex.Message);

                return false;
            }
        }
    }
}