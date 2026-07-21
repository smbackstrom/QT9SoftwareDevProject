using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Movies.Webforms
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(LoadMoviesAsync));
            }
        }

        private async Task LoadMoviesAsync()
        {
            ErrorLabel.Text = string.Empty;

            try
            {
                //throw new HttpRequestException("test");
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
    }
}