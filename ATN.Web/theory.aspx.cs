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
        int theoryId;
        int metaAnalysis;

        protected void Page_Load(object sender, EventArgs e)
        {
            theoryId = (Request.QueryString[Common.QueryStrings.TheoryId] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.TheoryId]) : 1;
            metaAnalysis = (Request.QueryString[Common.QueryStrings.MetaAnalysis] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.MetaAnalysis]) : 0;

            Theories sourceRetriever = new Theories();
            Theory theoryRetriever = new Theory();

            if (metaAnalysis == 0)
            {
                //show the papers contributing to the theory
                
                Source[] firstLevelSources = sourceRetriever.GetFirstLevelSourcesForTheory(theoryId);

                grdFirstLevelSources.DataSource = firstLevelSources;
                //show all columns

                //don't show footer
                grdFirstLevelSources.ShowFooter = false;
                
                theoryRetriever = sourceRetriever.GetTheory(theoryId);
                lblNetworkName.Text = theoryRetriever.TheoryName;
            }
            else
            {


                //hide the metaAnalysis checkbox
                grdFirstLevelSources.Columns[3].Visible = false;
                //show the manual entry footer
                grdFirstLevelSources.ShowFooter = true;
            }
            
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
                Source source = e.Row.DataItem as Source; 
                if (metaAnalysis == 0)
                {
                    LinkButton lnkTitle = e.Row.Cells[1].Controls[1] as LinkButton;
                    lnkTitle.PostBackUrl = Common.Pages.Theory + Common.Symbols.Question +
                        Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                        Common.QueryStrings.MetaAnalysis + Common.Symbols.Eq + source.SourceId.ToString();
                    Label lblTitle = e.Row.Cells[1].Controls[2] as Label;
                    lblTitle.Visible = false;

                    Label lblSourceId = e.Row.Cells[2].Controls[1] as Label;
                    lblSourceId.Text = source.SourceId.ToString();

                    CheckBox chkMetaAnalysis = e.Row.Cells[3].Controls[1] as CheckBox;
                    //if metaAnalysis then checked


                    //show number of papers contributing to this papers meta analysis
                    Label lblContributing = e.Row.Cells[4].Controls[1] as Label; 
                    //lblContributing.Text
                    RadioButtonList rblContributing = e.Row.Cells[4].Controls[2] as RadioButtonList;
                    rblContributing.Visible = false;

                    Label lblYear = e.Row.Cells[5].Controls[1] as Label;
                    lblYear.Text = source.Year.ToString();

                    Label lblEigenfactor = e.Row.Cells[6].Controls[1] as Label;
                    //lblEigenfactor.Text

                    Label lblCitationLevel = e.Row.Cells[7].Controls[1] as Label;
                    //lblCitationLevel.Text

                    Label lblAuthors = e.Row.Cells[8].Controls[1] as Label;
                    //lblAuthors.Text = source;

                    Label lblJournal = e.Row.Cells[9].Controls[1] as Label;
                    //lblJournal.Text = 

                }
                else
                {
                    //show col4 chkbox
                    LinkButton lnkTitle = e.Row.Cells[1].Controls[1] as LinkButton;
                    lnkTitle.Visible = false;
                    Label lblTitle = e.Row.Cells[1].Controls[2] as Label;
                    

                    Label lblSourceId = e.Row.Cells[2].Controls[1] as Label;
                    lblSourceId.Text = source.SourceId.ToString();

                    //show number of papers contributing to this papers meta analysis
                    Label lblContributing = e.Row.Cells[4].Controls[1] as Label;
                    //lblContributing.Text
                    RadioButtonList rblContributing = e.Row.Cells[4].Controls[2] as RadioButtonList;
                    //check if contributing
                    //if()
                    //rblContributing.SelectedValue 

                    Label lblYear = e.Row.Cells[5].Controls[1] as Label;
                    lblYear.Text = source.Year.ToString();

                    Label lblEigenfactor = e.Row.Cells[6].Controls[1] as Label;
                    //lblEigenfactor.Text

                    Label lblCitationLevel = e.Row.Cells[7].Controls[1] as Label;
                    //lblCitationLevel.Text

                    Label lblAuthors = e.Row.Cells[8].Controls[1] as Label;
                    //lblAuthors.Text = source;

                    Label lblJournal = e.Row.Cells[9].Controls[1] as Label;
                    //lblJournal.Text = 
                }
            }
            else
            {

            }
        }
        protected void btnSubmit_OnClientClick(object sender, EventArgs e)
        {
            //save changes
        }

        protected void lnkTitle_OnClientClick(object sender, EventArgs e)
        {
            //save changes
        }
    }
}