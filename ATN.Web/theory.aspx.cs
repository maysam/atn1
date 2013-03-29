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
        //the source id of the metaAnalysis
        int metaAnalysis;

        protected void Page_Load(object sender, EventArgs e)
        {
            theoryId = (Request.QueryString[Common.QueryStrings.TheoryId] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.TheoryId]) : 1;
            metaAnalysis = (Request.QueryString[Common.QueryStrings.MetaAnalysis] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.MetaAnalysis]) : 0;

            Theories sourceRetriever = new Theories();
            Theory theoryRetriever = new Theory();
            List<ExtendedSource> sources = new List<ExtendedSource>(); 

            if (metaAnalysis == 0)
            {
                //show the papers contributing to the theory
                //ExtendedSource allSources = 
                
                sources = sourceRetriever.GetAllExtendedSourcesForTheory(theoryId);

                //don't show footer
                grdFirstLevelSources.ShowFooter = false;
                
                theoryRetriever = sourceRetriever.GetTheory(theoryId);
                lblNetworkName.Text = theoryRetriever.TheoryName;
            }
            else
            {
                sources = sourceRetriever.GetReferencesForSourceId(metaAnalysis);
                
                //hide the metaAnalysis checkbox
                grdFirstLevelSources.Columns[3].Visible = false;
                //show the manual entry footer
                grdFirstLevelSources.ShowFooter = false;
            }

            grdFirstLevelSources.DataSource = sources;
            grdFirstLevelSources.DataBind();

        }

        protected void grdFirstLevelSources_RowDataBound(object sender, GridViewRowEventArgs e)
        {            

            if (e.Row.RowType == DataControlRowType.Header)
            {
                //column 3 needs to specify if what is going to be seen
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //ExtendedSource source = e.Row.DataItem as ExtendedSource;
                ExtendedSource source = e.Row.DataItem as ExtendedSource; 
                if (metaAnalysis == 0)
                {
                    #region non-metaAnalysis grid
                    LinkButton lnkTitle = e.Row.Cells[1].Controls[1] as LinkButton;
                    lnkTitle.Text = source.Source.ArticleTitle;
                    lnkTitle.PostBackUrl = Common.Pages.Theory + Common.Symbols.Question +
                        Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                        Common.QueryStrings.MetaAnalysis + Common.Symbols.Eq + source.Source.SourceId.ToString();
                    Label lblTitle = e.Row.Cells[1].Controls[2] as Label;
                    lblTitle.Visible = false;

                    Label lblSourceId = e.Row.Cells[2].Controls[1] as Label;
                    lblSourceId.Text = source.Source.SourceId.ToString();
                    
                    //if metaAnalysis then checked
                    CheckBox chkMetaAnalysis = e.Row.Cells[3].Controls[1] as CheckBox;
                    chkMetaAnalysis.Checked = source.metaAnalysis;

                    //show number of papers contributing to this papers meta analysis
                    Label lblContributing = e.Row.Cells[4].Controls[1] as Label;
                    if(source.numContributing != null)
                        lblContributing.Text = source.numContributing.ToString();
                    else
                        lblContributing.Text = Common.Symbols.Zero;

                    RadioButtonList rblContributing = e.Row.Cells[4].Controls[2] as RadioButtonList;
                    rblContributing.Visible = false;

                    Label lblYear = e.Row.Cells[5].Controls[1] as Label;
                    lblYear.Text = source.Source.Year.ToString();

                    Label lblEigenfactor = e.Row.Cells[6].Controls[1] as Label;
                    lblEigenfactor.Text = source.aefScore.ToString();

                    Label lblDepth = e.Row.Cells[7].Controls[1] as Label;
                    lblDepth.Text = source.depth.ToString();

                    string authorString = "";
                    foreach (Author author in source.Authors)
                    {
                        authorString += author.FullName + ", ";
                    }
                    Label lblAuthors = e.Row.Cells[8].Controls[1] as Label;
                    lblAuthors.Text = authorString;

                    Label lblJournal = e.Row.Cells[9].Controls[1] as Label;
                    lblJournal.Text = source.Journal.JournalName;
                    #endregion
                }
                else
                {
                    #region metaAnalysis grid
                    //show col4 chkbox
                    LinkButton lnkTitle = e.Row.Cells[1].Controls[1] as LinkButton;
                    lnkTitle.Visible = false;
                    Label lblTitle = e.Row.Cells[1].Controls[2] as Label;
                    lblTitle.Text = source.Source.ArticleTitle;

                    Label lblSourceId = e.Row.Cells[2].Controls[1] as Label;
                    lblSourceId.Text = source.Source.SourceId.ToString();

                    //show number of papers contributing to this papers meta analysis
                    Label lblContributing = e.Row.Cells[4].Controls[1] as Label;
                    //lblContributing.Text
                    RadioButtonList rblContributing = e.Row.Cells[4].Controls[2] as RadioButtonList;
                    //check if contributing
                    if (source.isContributing == true)
                    {
                        rblContributing.SelectedValue = Common.Symbols.Yes;
                    }
                    else if(source.isContributing == false)
                    {
                        rblContributing.SelectedValue = Common.Symbols.No;
                    }
                    else
                    {
                        rblContributing.SelectedValue = Common.Symbols.Unknown;
                    }

                    Label lblYear = e.Row.Cells[5].Controls[1] as Label;
                    lblYear.Text = source.Source.Year.ToString();

                    Label lblEigenfactor = e.Row.Cells[6].Controls[1] as Label;
                    lblEigenfactor.Text = source.aefScore.ToString();

                    Label lblDepth = e.Row.Cells[7].Controls[1] as Label;
                    lblDepth.Text = source.depth.ToString();

                    string authorString = "";
                    foreach (Author author in source.Authors)
                    {
                        authorString += author.FullName + ", ";
                    }
                    Label lblAuthors = e.Row.Cells[8].Controls[1] as Label;
                    lblAuthors.Text = authorString;

                    Label lblJournal = e.Row.Cells[9].Controls[1] as Label;
                    lblJournal.Text = source.Journal.JournalName; 
                    #endregion
                }
            }
            else
            {

            }
        }
        protected void btnSubmit_OnClientClick(object sender, EventArgs e)
        {
            //save changes
            save_results();
        }

        protected void lnkTitle_OnClientClick(object sender, EventArgs e)
        {
            //save changes
            save_results();

        }

        protected void save_results()
        {
            Theories dataSaver = new Theories();

            //not metaAnalysis
            if (metaAnalysis == 0)
            {

                foreach (GridViewRow row in grdFirstLevelSources.Rows)
                {
                    ExtendedSource rowSource = row.DataItem as ExtendedSource;
                    CheckBox chkMetaAnalysis = row.Cells[3].Controls[1] as CheckBox;
                    //mark the paper as metaAnalysis
                    if (chkMetaAnalysis.Checked == true)
                    {
                        //save to database
                        dataSaver.MarkSourceMetaAnalysis(theoryId, rowSource.Source.SourceId);
                    }
                }
            }
            else
            {
                foreach (GridViewRow row in grdFirstLevelSources.Rows)
                {
                    ExtendedSource rowSource = row.DataItem as ExtendedSource;
                    RadioButtonList rblContributing = row.Cells[4].Controls[1] as RadioButtonList;
                    bool? contributing = null;
                    //mark the paper as contributing
                    if (rblContributing.SelectedValue == "Yes")
                    {
                        //paper is contributing
                        contributing = true;
                    }
                    else if (rblContributing.SelectedValue == "No")
                    {
                        //paper is not contributing
                        contributing = false;
                    }
                    else
                    {
                        //unmarked by RA
                    }
                    
                    dataSaver.MarkSourceTheoryContribution(theoryId, rowSource.Source.SourceId, contributing);
                }
            }
        }
    }
}