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

        protected void Page_Init(object sender, EventArgs e)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            theoryId = (Request.QueryString[Common.QueryStrings.TheoryId] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.TheoryId]) : 6;
            metaAnalysisId = (Request.QueryString[Common.QueryStrings.MetaAnalysis] != null) ?
                long.Parse(Request.QueryString[Common.QueryStrings.MetaAnalysis]) : 307;
            lastPageIndex = (Request.QueryString[Common.QueryStrings.PageNumber] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.PageNumber]) : 0;
            
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
            lnkNetworkName.PostBackUrl = Common.Pages.Theory + Common.Symbols.Question +
                Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                Common.QueryStrings.PageNumber + Common.Symbols.Eq + lastPageIndex.ToString();

            //display the meta analysis label
            lblMetaAnalysisName.Text = "Meta-Analysis ID: " + metaAnalysisId.ToString();

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

            if (!IsPostBack)
            {
                //Bind data to grid
                grdFirstLevelSources.DataSource = sources;
                grdFirstLevelSources.DataBind();
            }

        }

        protected void grdFirstLevelSources_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.Header)
            {
                //header
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
                #endregion

            }
            else
            {
                //footer
            }
        }

        protected void btnPrevious_OnClick(object sender, EventArgs e)
        {
            save_results();
        }

        protected void btnNext_OnClick(object sender, EventArgs e)
        {
            save_results();
        }

        protected void btnSubmit_OnClick(object sender, EventArgs e)
        {
            //save changes            
            save_results();
        }

        protected void lnkTitle_OnClick(object sender, EventArgs e)
        {
            //save changes
            save_results();
        }

        protected void lnkNetworkName_OnClick(object sender, EventArgs e)
        {
            save_results();
        }

        protected void save_results()
        {
            Theories dataSaver = new Theories();

            foreach (GridViewRow row in grdFirstLevelSources.Rows)
            {
                ExtendedSource rowSource = row.DataItem as ExtendedSource;
                RadioButtonList rblContributing = row.Cells[3].Controls[1] as RadioButtonList;
                bool? contributing = null;
                //mark the paper as contributing
                if (rblContributing.SelectedValue == Common.Symbols.Unknown)
                {
                    //unmarked by RA
                }
                else if (rblContributing.SelectedValue == Common.Symbols.Yes)
                {
                    //paper is contributing
                    contributing = true;
                }
                else if (rblContributing.SelectedValue == Common.Symbols.No)
                {
                    //paper is not contributing
                    contributing = false;
                }
                if (contributing != null)
                {
                    dataSaver.MarkSourceTheoryContribution(theoryId, rowSource.SourceId, contributing);
                }
            }
        }

        //protected void rblContributing_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    HiddenField hdnRadioValue = 
        //}


    }
}