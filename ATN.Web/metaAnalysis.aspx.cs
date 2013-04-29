using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATN.Data;

namespace ATN.Web
{
    public partial class metaAnalysis : System.Web.UI.Page
    {
        //the theory to evaluate
        int theoryId;
        //the source id of the metaAnalysis or 0 if it is not a meta analysis
        long metaAnalysisId;
        //The page number
        int pageNumber;
        int lastPageIndex;
        string sortCol = string.Empty;
        string sortOrder = string.Empty;

        /// <summary>
        /// retrieves url parameters, retrieves data, populates grid values, sets button values, and handles postback controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //retrieve the arguments from url
            theoryId = (Request.QueryString[Common.QueryStrings.TheoryId] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.TheoryId]) : 2;
            metaAnalysisId = (Request.QueryString[Common.QueryStrings.MetaAnalysis] != null) ?
                long.Parse(Request.QueryString[Common.QueryStrings.MetaAnalysis]) : 654;
            lastPageIndex = (Request.QueryString[Common.QueryStrings.PageNumber] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.PageNumber]) : 0;
            sortCol = (Request.QueryString[Common.QueryStrings.SortCol] != null) ?
                Request.QueryString[Common.QueryStrings.SortCol] : null;
            sortOrder = (Request.QueryString[Common.QueryStrings.SortOrder] != null) ?
                 Request.QueryString[Common.QueryStrings.SortOrder] : null;
            
            //Declare variables
            Theories sourceRetriever = new Theories();
            Theory theoryRetriever = new Theory();
            List<ExtendedSource> sources = new List<ExtendedSource>();

            //retrieve information from database
            sources = sourceRetriever.GetExtendedSourceReferencesForSource(theoryId, metaAnalysisId);
            
            theoryRetriever = sourceRetriever.GetTheory(theoryId);
            //set the network label
            lnkNetworkName.Text = theoryRetriever.TheoryName;
            //link back to theory page
            lnkNetworkName.PostBackUrl = Common.Pages.MetaAnalysis + Common.Symbols.Question +
                Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                Common.QueryStrings.MetaAnalysis + Common.Symbols.Eq + metaAnalysisId.ToString() + Common.Symbols.Amp +
                Common.QueryStrings.PageNumber + Common.Symbols.Eq + lastPageIndex.ToString();

            //Mark as metaAnalysis
            btnMarkAsMetaAnalysis.PostBackUrl = Common.Pages.MetaAnalysis + Common.Symbols.Question +
                Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                Common.QueryStrings.MetaAnalysis + Common.Symbols.Eq + metaAnalysisId.ToString() + Common.Symbols.Amp +
                Common.QueryStrings.PageNumber + Common.Symbols.Eq + lastPageIndex.ToString();

            //display the meta analysis label
            if (sources.Count != 0)
            {
                lblMetaAnalysisName.Text = "Meta-Analysis ID: " + metaAnalysisId.ToString();
            }
            else
            {
                lblMetaAnalysisName.Text = "There are no citations available for source ID " + metaAnalysisId.ToString();
                btnMarkAsMetaAnalysis.Visible = false;
            }

            #region end of page navigation and save buttons
            //if first page
            if (lastPageIndex == 0)
            {
                btnPrevious.Visible = false;
            }
            else
            {
                btnPrevious.Visible = true;
                pageNumber = lastPageIndex - 1;
                btnPrevious.PostBackUrl = Common.Pages.MetaAnalysis + Common.Symbols.Question +
                    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.MetaAnalysis + Common.Symbols.Eq + metaAnalysisId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.PageNumber + Common.Symbols.Eq + pageNumber.ToString();
            }
            //if last page
            if (sources.Count < Common.Data.PageSize)
            {
                btnNext.Visible = false;
            }
            else
            {
                btnNext.Visible = true;
                pageNumber = lastPageIndex + 1;
                btnNext.PostBackUrl = Common.Pages.MetaAnalysis + Common.Symbols.Question +
                    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.MetaAnalysis + Common.Symbols.Eq + metaAnalysisId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.PageNumber + Common.Symbols.Eq + pageNumber.ToString();
            }
            #endregion

            #region sort the data
            if (sortCol != null)
            {
                Common.Sort<ExtendedSource>(sources, Request.QueryString[Common.QueryStrings.SortCol] + " " +
                                                    Request.QueryString[Common.QueryStrings.SortOrder]);
            }
            //default sort by Theory Name
            else
            {
                Common.Sort<ExtendedSource>(sources, Common.QueryStrings.SourceId + " " + Common.Symbols.Asc);
            }
            #endregion

            #region post back controls
            string postBackControl = Common.GetPostBackControlId(Page);
            //not a post back
            if (postBackControl == string.Empty)
            {
                grdFirstLevelSources.DataSource = sources;
                grdFirstLevelSources.DataBind();
            }
            //postback to same page, use viewstate to save results
            else if (postBackControl == "btnNext" || postBackControl == "btnPrevious" || postBackControl == "btnSubmit")
            {
                save_results();
                //grdFirstLevelSources.DataSource = sources;
                //grdFirstLevelSources.DataBind();
            }
            else if (postBackControl == "lnkAuthorsHeader" || postBackControl == "lnkTitleHeader" || postBackControl == "lnkSourceIdHeader" || postBackControl == "lnkYearHeader" || 
                     postBackControl == "lnkAEFHeader" || postBackControl == "lnkDepthHeader" || postBackControl == "lnkJournalHeader" || postBackControl == "lnkPredictionHeader")
            {
                save_results();
                grdFirstLevelSources.DataSource = sources;
                grdFirstLevelSources.DataBind();
            }
            else if (postBackControl == "btnMarkAsMetaAnalysis")
            {
                //save results on page
                save_results();
                
                //mark as meta analysis
                Theories dataSaver = new Theories();
                dataSaver.MarkSourceMetaAnalysis(theoryId, metaAnalysisId);

                //populate grid
                grdFirstLevelSources.DataSource = sources;
                grdFirstLevelSources.DataBind();
            }
            //redirect to theory
            else
            {
                save_results();
                Response.Redirect(Common.Pages.Theory + Common.Symbols.Question +
                    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.PageNumber + Common.Symbols.Eq + lastPageIndex.ToString());
            }
            #endregion
        }

        /// <summary>
        /// Bind data to grid view rows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The row control</param>
        protected void grdFirstLevelSources_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.Header)
            {
                //header
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

                string URL = Common.Pages.MetaAnalysis + Common.Symbols.Question +
                    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.MetaAnalysis + Common.Symbols.Eq + metaAnalysisId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.PageNumber + Common.Symbols.Eq + lastPageIndex.ToString() + Common.Symbols.Amp;

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
                LinkButton lnkAuthorsHeader = e.Row.Cells[0].Controls[1] as LinkButton;
                lnkAuthorsHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.Authors;

                LinkButton lnkTitleHeader = e.Row.Cells[1].Controls[1] as LinkButton;
                lnkTitleHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.Title;

                LinkButton lnkSourceIdHeader = e.Row.Cells[2].Controls[1] as LinkButton;
                lnkSourceIdHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.SourceId;

                LinkButton lnkYearHeader = e.Row.Cells[4].Controls[1] as LinkButton;
                lnkYearHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.Year;

                LinkButton lnkAEFHeader = e.Row.Cells[5].Controls[1] as LinkButton;
                lnkAEFHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.AEF;

                LinkButton lnkDepthHeader = e.Row.Cells[6].Controls[1] as LinkButton;
                lnkDepthHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.Depth;

                LinkButton lnkJournalHeader = e.Row.Cells[7].Controls[1] as LinkButton;
                lnkJournalHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.Journal;

                LinkButton lnkPredictionHeader = e.Row.Cells[8].Controls[1] as LinkButton;
                lnkPredictionHeader.PostBackUrl = URL + Common.QueryStrings.SortCol + Common.Symbols.Eq + Common.QueryStrings.PredictionProbability;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //ExtendedSource source = e.Row.DataItem as ExtendedSource;
                ExtendedSource source = e.Row.DataItem as ExtendedSource;
                
                #region populate cells

                //cell 0 - author
                char[] charsToTrim = { ',' };
                if (source.Authors != null)
                {
                    source.Authors = source.Authors.TrimEnd(charsToTrim);
                }
                else
                {
                    source.Authors = "";
                }
                Label lblAuthors = e.Row.Cells[0].Controls[1] as Label;
                lblAuthors.Text = source.Authors;

                //cell 1 - Title
                Label lblTitle = e.Row.Cells[1].Controls[1] as Label;
                lblTitle.Text = source.Title;

                //cell 2 - Source ID
                Label lblSourceId = e.Row.Cells[2].Controls[1] as Label;
                lblSourceId.Text = source.SourceId.ToString();

                //cell 3 - Contributing? Radio Buttons
                RadioButtonList rblContributing = e.Row.Cells[3].Controls[1] as RadioButtonList;
                //check if contributing
                //null is interpreted as false so this condition must be first
                if (source.Contributing == null)
                {
                    rblContributing.SelectedValue = Common.Symbols.Unknown;
                }
                else if (source.Contributing == true)
                {
                    rblContributing.SelectedValue = Common.Symbols.Yes;
                }
                else if (source.Contributing == false)
                {
                    rblContributing.SelectedValue = Common.Symbols.No;
                }
                HiddenField hdnSourceId = e.Row.Cells[3].Controls[3] as HiddenField;
                hdnSourceId.Value = source.SourceId.ToString();

                //cell 4 - year
                Label lblYear = e.Row.Cells[4].Controls[1] as Label;
                lblYear.Text = source.Year.ToString();

                //cell 5 - AEF
                Label lblEigenfactor = e.Row.Cells[5].Controls[1] as Label;
                lblEigenfactor.Text = source.AEF.ToString();

                //cell 6 - Depth
                Label lblDepth = e.Row.Cells[6].Controls[1] as Label;
                lblDepth.Text = source.Depth.ToString();

                //cell 7 - Journal
                Label lblJournal = e.Row.Cells[7].Controls[1] as Label;
                lblJournal.Text = source.Journal;

                //cell 8 - Prediction
                Label lblPrediction = e.Row.Cells[8].Controls[1] as Label;
                if (source.isContributingPrediction == true)
                {
                    lblPrediction.Text = "Contributing";
                }
                else if (source.isContributingPrediction == false)
                {
                    lblPrediction.Text = "Not Contributing";
                }

                Label lblPredictionScore = e.Row.Cells[8].Controls[3] as Label;
                lblPredictionScore.Text = (source.predictionProbability != null) ? "Probability: " + source.predictionProbability.ToString() : string.Empty;
                #endregion

            }
            else
            {
                //footer
            }
        }

        /// <summary>
        /// save the values of the meta analysis checkboxes
        /// </summary>
        protected void save_results()
        {
            Theories dataSaver = new Theories();
            int itr;
            bool? contributing;
            for (itr = 0; itr < Page.Request.Form.Keys.Count; itr++)
            {
                contributing = null;
                if (Page.Request.Form[itr] == Common.Symbols.Unknown)
                {
                    //unmarked by RA or reset to null
                    dataSaver.MarkSourceTheoryContribution(theoryId, long.Parse(Page.Request.Form[itr + 1]), contributing);
                }
                else if (Page.Request.Form[itr] == Common.Symbols.Yes)
                {
                    contributing = true;
                    dataSaver.MarkSourceTheoryContribution(theoryId, long.Parse(Page.Request.Form[itr + 1]), contributing);
                }
                else if(Page.Request.Form[itr] == Common.Symbols.No)
                {
                    contributing = false;
                    dataSaver.MarkSourceTheoryContribution(theoryId, long.Parse(Page.Request.Form[itr + 1]), contributing);
                }
                
            }

        }

    }
}