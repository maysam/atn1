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
            sortCol = (Request.QueryString[Common.QueryStrings.SortCol] != null) ? 
                       Request.QueryString[Common.QueryStrings.SortCol] : null;
            sortOrder = (Request.QueryString[Common.QueryStrings.SortOrder] != null) ?
                         Request.QueryString[Common.QueryStrings.SortOrder] : null;

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

            List<Theory> allTheories;
            allTheories = theoryCaller.GetTheoriesAsList();
            //sort the partStepList if necessary
            if (sortCol != null)
            {
                Common.Sort<Theory>(allTheories, Request.QueryString[Common.QueryStrings.SortCol] + " " +
                                                    Request.QueryString[Common.QueryStrings.SortOrder]);
            }
            //default sort by Theory Name
            else
            {
                Common.Sort<Theory>(allTheories, Common.QueryStrings.TheoryName + " " + Common.Symbols.Des);
            }

            //set the grid
            grdNetworks.DataSource = allTheories;
            grdNetworks.DataBind();
            

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
                #region DataRows
                Theory theory = e.Row.DataItem as Theory;
                Theories dataRetriever = new Theories();
                CrawlerProgress lastCrawl = new CrawlerProgress();

                ImageButton ImgVisualizationLink = e.Row.Cells[0].Controls[1] as ImageButton;

                string BaseAttributes = "channelmode=no,directories=no,resizable=yes,scrollbars=yes,location=yes,menubar=yes,status=no,toolbar=no";
                ImgVisualizationLink.Attributes.Add("onclick", "window.open('ExportVisualization.ashx?TheoryId=" + theory.TheoryId.ToString() + "','_blank','" + BaseAttributes + "',false);");
                ImgVisualizationLink.ImageUrl = "/Images/HBPLogo.png";

                LinkButton lnkTheoryNameHeader = e.Row.Cells[1].Controls[1] as LinkButton;
                lnkTheoryNameHeader.Text = theory.TheoryName;
                lnkTheoryNameHeader.PostBackUrl = Common.Pages.Theory + Common.Symbols.Question + Common.QueryStrings.TheoryId + Common.Symbols.Eq + theory.TheoryId.ToString();

                Label lblDate = e.Row.Cells[2].Controls[1] as Label;
                lblDate.Text = theory.DateAdded.ToString();

                Label lblLastRun = e.Row.Cells[3].Controls[1] as Label;
                lblLastRun.Text = lastCrawl.GetLastCrawlDate(theory.TheoryId).ToString();

                Label lblLastEigenfactor = e.Row.Cells[4].Controls[1] as Label;
                lblLastEigenfactor.Text = theory.LastAnalysisDate.ToString();

                Label lblLastMachineLearning = e.Row.Cells[5].Controls[1] as Label;

                Label lblStatus = e.Row.Cells[6].Controls[1] as Label;
                
                Label lblSecondLevel = e.Row.Cells[7].Controls[1] as Label;
                lblSecondLevel.Text = dataRetriever.GetFirstLevelSourcesForTheory(theory.TheoryId).Length.ToString();
                
                Label lblThirdLevel = e.Row.Cells[8].Controls[1] as Label;

                Label lblTheoryContributing  = e.Row.Cells[9].Controls[1] as Label;
                //lblTheoryContributing.Text = 

                LinkButton lnkEdit = e.Row.Cells[10].Controls[1] as LinkButton;
                lnkEdit.PostBackUrl = Common.Pages.Launcher + Common.Symbols.Question + Common.QueryStrings.TheoryId + Common.Symbols.Eq + theory.TheoryId.ToString();
                #endregion
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                
                Image imgSort = new Image();
                if (sortCol != null)
                {
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        if (cell.FindControl("img" + sortCol + "Header") != null)
                        {
                            imgSort = cell.FindControl("img" + sortCol + "Header") as Image;
                            imgSort.Visible = true;
                            imgSort.ImageUrl = (sortOrder == Common.Symbols.Asc) ? "Images/AtoZ.gif" : "Images/ZtoA.gif";
                        }
                    }
                }

                string URL = Common.Pages.Networks + Common.Symbols.Question;
                            
                switch (sortOrder)
                {
                    case null:
                        URL += Common.QueryStrings.SortOrder + Common.Symbols.Eq + Common.Symbols.Des + Common.Symbols.Amp;
                        break;
                    case Common.Symbols.Asc:
                        URL += Common.QueryStrings.SortOrder + Common.Symbols.Eq + Common.Symbols.Des + Common.Symbols.Amp;
                        break;
                    case Common.Symbols.Des:
                        URL += Common.QueryStrings.SortOrder + Common.Symbols.Eq + Common.Symbols.Asc + Common.Symbols.Amp;
                        break;
                }

                //make headers sortable
                LinkButton lnkNetworkHeader = e.Row.Cells[1].Controls[1] as LinkButton;
                lnkNetworkHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.TheoryName;

                LinkButton lnkDateHeader = e.Row.Cells[2].Controls[1] as LinkButton;
                lnkDateHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.DateAdded;

                LinkButton lnkLastRunHeader = e.Row.Cells[3].Controls[1] as LinkButton;
                //lnkLastRunHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.LastRun;

                LinkButton lnkLastEigenfactorHeader = e.Row.Cells[4].Controls[1] as LinkButton;
                //lnkLastEigenfactorHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.LastEigenfactor;

                LinkButton lnkLastMachineLearningHeader = e.Row.Cells[5].Controls[1] as LinkButton;
                //lnkLastMachineLearningHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.LastMachineLearning;

                LinkButton lnkStatusHeader = e.Row.Cells[6].Controls[1] as LinkButton;
                //lnkStatusHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.Status;

                LinkButton lnkSecondLevelHeader = e.Row.Cells[7].Controls[1] as LinkButton;
                //lnkSecondLevelHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.SecondLevel;

                LinkButton lnkThirdLevelHeader = e.Row.Cells[8].Controls[1] as LinkButton;
                //lnkThirdLevelHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.ThirdLevel;

                LinkButton lnkTheoryContributingHeader = e.Row.Cells[9].Controls[1] as LinkButton;
                //lnkTheoryContributingHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.TheoryContributing;

            }
        }
    }
}