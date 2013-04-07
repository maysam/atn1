using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATN.Data;

namespace ATN.Web
{
    public partial class theory : System.Web.UI.Page
    {
        //the theory to evaluate
        int theoryId;
        //The page number
        int lastPageIndex;

        protected void Page_Init(object sender, EventArgs e)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            theoryId = (Request.QueryString[Common.QueryStrings.TheoryId] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.TheoryId]) : 6;
            lastPageIndex = (Request.QueryString[Common.QueryStrings.PageNumber] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.PageNumber]) : 0;
            
            Theories sourceRetriever = new Theories();
            Theory theoryRetriever = new Theory();
            List<ExtendedSource> sources = new List<ExtendedSource>(); 

            //show the papers contributing to the theory
            //ExtendedSource allSources = 
                
            sources = sourceRetriever.GetAllExtendedSourcesForTheory(theoryId, lastPageIndex, Common.Data.PageSize);
                
            theoryRetriever = sourceRetriever.GetTheory(theoryId);
            
            //set the network label
            lblNetworkName.Text = theoryRetriever.TheoryName;

            //first page
            if (lastPageIndex == 0)
            {
                btnPrevious.Visible = false;
            }
            else
            {
                btnPrevious.Visible = true;
                int pageNumber = lastPageIndex - 1;
                btnPrevious.PostBackUrl = Common.Pages.Theory + Common.Symbols.Question +
                    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.PageNumber + Common.Symbols.Eq + pageNumber.ToString();
            }
            //last page
            if (sources.Count < Common.Data.PageSize)
            {
                btnNext.Visible = false;
            }
            else
            {
                btnNext.Visible = true;
                int pageNumber = lastPageIndex + 1;
                btnNext.PostBackUrl = Common.Pages.Theory + Common.Symbols.Question +
                    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.PageNumber + Common.Symbols.Eq + pageNumber.ToString();
            }

            //if (!IsPostBack)
            //{
                grdFirstLevelSources.DataSource = sources;
                grdFirstLevelSources.DataBind();
            //}
            //else if(Session[Common.Data.MetaAnalysisIds] != null)
            //{
            //    save_results((List<long>)Session[Common.Data.MetaAnalysisIds]);
            //}

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

                #region set cell values

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
                LinkButton lnkTitle = e.Row.Cells[1].Controls[1] as LinkButton;
                lnkTitle.Text = source.Title;
                lnkTitle.PostBackUrl = Common.Pages.MetaAnalysis + Common.Symbols.Question +
                    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.MetaAnalysis + Common.Symbols.Eq + source.SourceId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.PageNumber + Common.Symbols.Eq + lastPageIndex.ToString(); 

                //cell 2 - source ID
                Label lblSourceId = e.Row.Cells[2].Controls[1] as Label;
                lblSourceId.Text = source.SourceId.ToString();
                    
                //cell 3 - meta analysis checkbox
                //if metaAnalysis then checked
                CheckBox chkMetaAnalysis = e.Row.Cells[3].Controls[1] as CheckBox;
                chkMetaAnalysis.Checked = source.isMetaAnalysis;

                //cell 4 - number of contributing papers in meta analysis
                //show number of papers contributing to this papers meta analysis
                Label lblContributing = e.Row.Cells[4].Controls[1] as Label;
                if(source.numContributing != null)
                    lblContributing.Text = source.numContributing.ToString();
                else
                    lblContributing.Text = Common.Symbols.Zero;

                //cell 5 - year
                Label lblYear = e.Row.Cells[5].Controls[1] as Label;
                lblYear.Text = source.Year.ToString();

                //cell 6 - AEF
                Label lblEigenfactor = e.Row.Cells[6].Controls[1] as Label;
                lblEigenfactor.Text = source.aef.ToString();

                //cell 7 - depth
                Label lblDepth = e.Row.Cells[7].Controls[1] as Label;
                lblDepth.Text = source.depth.ToString();

                //cell 8 - Journal
                Label lblJournal = e.Row.Cells[8].Controls[1] as Label;
                lblJournal.Text = source.Journal;
                #endregion
                
            }
            else
            {
                //footer
            }
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            //List<long> values = new List<long>();
            //HiddenField hdnSourceId;
            //for (int i = 0; i < grdFirstLevelSources.Rows.Count; i++)
            //{
            //    hdnSourceId = grdFirstLevelSources.Rows[i].Cells[3].Controls[3] as HiddenField;

            //    if (hdnSourceId.Value != null)
            //    {

            //        values.Add(long.Parse(hdnSourceId.Value));
            //    }
            //}
            //Session[Common.Data.MetaAnalysisIds] = values;
            //Response.Redirect(Common.Pages.Theory + Common.Symbols.Question +
            //    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp + 
            //    Common.QueryStrings.PageNumber + Common.Symbols.Eq + (lastPageIndex - 1).ToString());
            save_results();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            //List<long> values = new List<long>();
            //HiddenField hdnSourceId;
            //for (int i = 0; i < grdFirstLevelSources.Rows.Count; i++)
            //{
            //    hdnSourceId = grdFirstLevelSources.Rows[i].Cells[3].Controls[3] as HiddenField;

            //    if (hdnSourceId.Value != null)
            //    {
                    
            //        values.Add(long.Parse(hdnSourceId.Value));
            //    }
            //}
            //Session[Common.Data.MetaAnalysisIds] = values;
            //Response.Redirect(Common.Pages.Theory + Common.Symbols.Question +
            //    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
            //    Common.QueryStrings.PageNumber + Common.Symbols.Eq + (lastPageIndex + 1).ToString());
            save_results();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //save changes
            save_results();
        }

        protected void lnkTitle_Click(object sender, EventArgs e)
        {
            //save changes
            save_results();
        }

        protected void save_results()
        {
            Theories dataSaver = new Theories();

            foreach (GridViewRow row in grdFirstLevelSources.Rows)
            {
                ExtendedSource rowSource = row.DataItem as ExtendedSource;
                CheckBox chkMetaAnalysis = row.Cells[3].Controls[1] as CheckBox;
                //mark the paper as metaAnalysis
                if (chkMetaAnalysis.Checked == true)
                {
                    lblNetworkName.Text = rowSource.SourceId.ToString();
                    //save to database
                    //dataSaver.MarkSourceMetaAnalysis(theoryId, rowSource.SourceId);
                }
            }
        }

        //protected void save_results(List<long> values)
        //{
        //    Theories dataSaver = new Theories();

        //    foreach (GridViewRow row in grdFirstLevelSources.Rows)
        //    {
        //        foreach (long sourceId in values)
        //        {
        //            lblNetworkName.Text = sourceId.ToString();
        //            //dataSaver.MarkSourceMetaAnalysis(theoryId, sourceId);
        //        }
        //    }
        //}

        protected void chkMetaAnalysis_CheckedChanged(object sender, EventArgs e)
        {
            //CheckBox chb = sender as CheckBox;
            //HiddenField hdnSourceId = chb.Parent.Controls[3] as HiddenField;
            //ExtendedSource source = ((GridViewRow)chb.Parent.Parent).DataItem as ExtendedSource;
            //if (chb.Checked == true)
            //{
            //    hdnSourceId.Value = source.SourceId.ToString();
            //}
        }

        //protected void grdFirstLevelSources_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    List<long> values = new List<long>();
        //    CheckBox chb;
        //    ExtendedSource source;
        //    for (int i = 0; i < grdFirstLevelSources.Rows.Count; i++)
        //    {
        //        chb = grdFirstLevelSources.Rows[i].Cells[3].Controls[1] as CheckBox;

        //        if (chb.Checked == true)
        //        {
        //            source = grdFirstLevelSources.Rows[i].DataItem as ExtendedSource;
        //            values.Add(source.SourceId);
        //        }
        //    }
        //    Session["page" + grdFirstLevelSources.PageIndex] = values;
        //}

        //protected void rblContributing_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    HiddenField hdnRadioValue = 
        //}

        
    }
}