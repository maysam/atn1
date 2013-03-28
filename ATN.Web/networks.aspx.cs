using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATN.Data;

namespace ATN.Web
{
    public partial class networks : System.Web.UI.Page
    {
        string sortCol = string.Empty;
        string sortOrder = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            sortCol = (Request.QueryString[Common.QueryStrings.SortCol] != null) ? Request.QueryString[Common.QueryStrings.SortCol] : "TheoryName";
            sortOrder = (Request.QueryString[Common.QueryStrings.SortOrder] != null) ? Request.QueryString[Common.QueryStrings.SortOrder] : Common.Symbols.Asc;

            if (!Page.IsPostBack)
            {
                Theories theoryCaller = new Theories();

                //test data
                //Theory theory1 = new Theory();
                //theory1.TheoryId = 1;
                //theory1.TheoryName = "Theory 1";
                //theory1.DateAdded = DateTime.Now;
                //Theory theory2 = new Theory();
                //theory2.TheoryId = 1;
                //theory2.TheoryName = "Theory 2";
                //theory2.DateAdded = DateTime.Now;
                //Theory[] allTheories = new Theory[2] { theory1, theory2 };

                Theory[] allTheories;
                allTheories = theoryCaller.GetTheories();

                grdNetworks.DataSource = allTheories;//.Cast<Theory>().ToList<Theory>();

                grdNetworks.DataBind();
            }

        }

        /// <summary>
        /// Sets up each row in the gridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdNetworks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Theory theory = e.Row.DataItem as Theory;
                Theories dataRetriever = new Theories();
                CrawlerProgress lastCrawl = new CrawlerProgress();

                ImageButton ImgVisualizationLink = e.Row.Cells[0].Controls[1] as ImageButton;
                //ImgVisualizationLink.OnClientClick
                //ImgVisualizationLink.ImageUrl = "/Images/visualizationButton.gif";

                LinkButton lnkNetwork = e.Row.Cells[1].Controls[1] as LinkButton;
                lnkNetwork.Text = theory.TheoryName;
                lnkNetwork.PostBackUrl = Common.Pages.Theory + Common.Symbols.Question + Common.QueryStrings.TheoryId + Common.Symbols.Eq + theory.TheoryId.ToString();

                Label lblDate = e.Row.Cells[2].Controls[1] as Label;
                lblDate.Text = theory.DateAdded.ToString();

                Label lblLastRun = e.Row.Cells[3].Controls[1] as Label;
                lblLastRun.Text = lastCrawl.GetLastCrawlDate(theory.TheoryId).ToString();

                Label lblLastEigenfactor = e.Row.Cells[4].Controls[1] as Label;

                Label lblLastMachineLearning = e.Row.Cells[5].Controls[1] as Label;

                Label lblStatus = e.Row.Cells[6].Controls[1] as Label;
                
                Label lblSecondLevel = e.Row.Cells[7].Controls[1] as Label;
                lblSecondLevel.Text = dataRetriever.GetFirstLevelSourcesForTheory(theory.TheoryId).Length.ToString();

                Label lblThirdLevel = e.Row.Cells[8].Controls[1] as Label;

                Label lblTheoryContributing  = e.Row.Cells[9].Controls[1] as Label;
                //lblTheoryContributing.Text = 

                LinkButton lnkEdit = e.Row.Cells[10].Controls[1] as LinkButton;
                lnkEdit.PostBackUrl = Common.Pages.Launcher + Common.Symbols.Question + Common.QueryStrings.TheoryId + Common.Symbols.Eq + theory.TheoryId.ToString();
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                //make headers sortable

                //set asc/dec arrow

            }
        }
    }
}