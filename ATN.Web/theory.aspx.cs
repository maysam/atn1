﻿using System;
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

        /// <summary>
        /// Retrieves data, populatees grid, sets values for conttrols, postback handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //retrieve arguments from url
            Int32.TryParse(Request.QueryString[Common.QueryStrings.TheoryId], out theoryId);
            Int32.TryParse(Request.QueryString[Common.QueryStrings.PageNumber], out lastPageIndex);
            
            Theories sourceRetriever = new Theories();
            Theory theoryRetriever = new Theory();
            List<ExtendedSource> sources = new List<ExtendedSource>();
            string postBackControl = Common.GetPostBackControlId(Page);

            string BaseAttributes = "channelmode=yes,directories=yes,resizable=yes,scrollbars=yes,location=yes,menubar=yes,status=yes,toolbar=yes";

            addnew.Attributes.Add("onclick", "window.open('AddMissingSource.aspx?TheoryId=" + theoryId.ToString() + "','_blank','" + BaseAttributes + "',false);");

            //show the papers contributing to the theory                
            if (postBackControl == "btnRandomize")
            {
                sources = sourceRetriever.GetAllExtendedSourcesForTheory(theoryId, lastPageIndex, Common.Data.PageSize, true);
            }
            else if (postBackControl == "btnFindSource")
            {
                string metaAnalysis = Page.Request.Form["ctl00$MainContent$txtFindSource"];
                sources = sourceRetriever.GetAllExtendedSourcesForTheory(theoryId, lastPageIndex, Common.Data.PageSize, false, metaAnalysis);
            }
            else
            {
                sources = sourceRetriever.GetAllExtendedSourcesForTheory(theoryId, lastPageIndex, Common.Data.PageSize);
            }

            theoryRetriever = sourceRetriever.GetTheory(theoryId);
            //set the network label
            if (theoryRetriever.TheoryName != null)
            {
                if (sources.Count == 0)
                {
                    lblNetworkName.Text = "There are no citations available for " + theoryRetriever.TheoryName;
                }
                else
                {
                    lblNetworkName.Text = theoryRetriever.TheoryName;
                }
            }
            else
            {
                lblNetworkName.Text = "Theory does not exist";
            }

            //set the paging and submit buttons
            //postback is to the same page to save data and then redirects if necessary
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
            //save button
            btnSubmit.PostBackUrl = Common.Pages.Theory + Common.Symbols.Question +
                Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                Common.QueryStrings.PageNumber + Common.Symbols.Eq + lastPageIndex.ToString();

            //save the results if necessary and display the correct information
            
            //not a post back
            if (postBackControl == string.Empty || postBackControl == "lnkTitleHeader")
            {
                if (postBackControl == "lnkTitleHeader")
                {
                    Common.Sort<ExtendedSource>(sources, "Title ASC");
                }
                grdFirstLevelSources.DataSource = sources;
                grdFirstLevelSources.DataBind();
            }
            //postback to same page, use viewstate to save results
            else if (postBackControl == "btnNext" || postBackControl == "btnPrevious" || postBackControl == "btnSubmit" || postBackControl == "btnRandomize" || postBackControl == "btnFindSource")
            {
                save_results();
                grdFirstLevelSources.DataSource = sources;
                grdFirstLevelSources.DataBind();
            }
            //redirect to metaAnalysis
            else if (postBackControl != "btnExportVisualization" && postBackControl != "btnExportTrain" && postBackControl != "btnExportClassify")
            {
                save_results();
                long metaAnalysis = (Request.QueryString[Common.QueryStrings.MetaAnalysis] != null) ?
                    long.Parse(Request.QueryString[Common.QueryStrings.MetaAnalysis]) : 0;
                Response.Redirect(Common.Pages.MetaAnalysis + Common.Symbols.Question +
                    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.MetaAnalysis + Common.Symbols.Eq + metaAnalysis.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.PageNumber + Common.Symbols.Eq + lastPageIndex.ToString());
            }
        }

        /// <summary>
        /// Binds data to each row of the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Grid view row object</param>
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
                Label lblAuthors = e.Row.Cells[3].Controls[1] as Label;
                lblAuthors.Text = source.Authors;

                //cell 1 - Title
                LinkButton lnkTitle = e.Row.Cells[5].Controls[1] as LinkButton;
                lnkTitle.Text = source.Title;
                lnkTitle.PostBackUrl = Common.Pages.Theory + Common.Symbols.Question +
                    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.MetaAnalysis + Common.Symbols.Eq + source.SourceId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.PageNumber + Common.Symbols.Eq + lastPageIndex.ToString(); 

                //cell 2 - source ID
                Label lblSourceId = e.Row.Cells[2].Controls[1] as Label;
                lblSourceId.Text = source.SourceId.ToString();
                    
                //cell 3 - meta analysis checkbox
                //if metaAnalysis then checked
                CheckBox chkMetaAnalysis = e.Row.Cells[0].Controls[1] as CheckBox;
                chkMetaAnalysis.Checked = source.IsMetaAnalysis;

                //put sourceId in hidden field for postBack 
                HiddenField hdnSourceId = e.Row.Cells[0].Controls[3] as HiddenField;
                hdnSourceId.Value = source.SourceId.ToString();

                //cell 4 - number of contributing papers in meta analysis
                //show number of papers contributing to this papers meta analysis
                Label lblContributing = e.Row.Cells[1].Controls[1] as Label;
                if(source.NumContributing != null)
                    lblContributing.Text = source.NumContributing.ToString();
                else
                    lblContributing.Text = Common.Symbols.Zero;

                //cell 5 - year
                Label lblYear = e.Row.Cells[4].Controls[1] as Label;
                lblYear.Text = source.Year.ToString();

                //cell 6 - AEF
                Label lblEigenfactor = e.Row.Cells[8].Controls[1] as Label;
                lblEigenfactor.Text = source.AEF.HasValue ? string.Format("{0:f7}", source.AEF.Value) : string.Empty;

                //cell 7 - depth
                Label lblDepth = e.Row.Cells[7].Controls[1] as Label;
                lblDepth.Text = source.Depth.ToString();

                //cell 8 - Journal
                Label lblJournal = e.Row.Cells[6].Controls[1] as Label;
                lblJournal.Text = source.Journal;

                //cell 9 - Prediction
                Label lblPrediction = e.Row.Cells[9].Controls[1] as Label;
                if(source.isContributingPrediction == true)
                {
                    lblPrediction.Text = "C";
                }
                else if (source.isContributingPrediction == false)
                {
                    lblPrediction.Text = "NC";
                }
                
                Label lblPredictionScore = e.Row.Cells[9].Controls[3] as Label;
                lblPredictionScore.Text = source.predictionProbability.HasValue ? string.Format("{0:f2}", source.predictionProbability.Value) : string.Empty;
                #endregion
                
            }
            else
            {
                //footer
            }
        }

        /// <summary>
        /// Saves the checked sources as meta analysis
        /// </summary>
        protected void save_results()
        {
            Theories dataSaver = new Theories();
            int itr;
            for (itr = 0; itr < Page.Request.Form.Keys.Count; itr++)
            {
                if (Page.Request.Form[itr] == "on")
                {
                    //lblNetworkName.Text = Page.Request.Form[itr + 1];
                    dataSaver.MarkSourceMetaAnalysis(theoryId, long.Parse(Page.Request.Form[itr + 1]));
                }
                else
                {
                    continue;
                }

            }

        }

        /// <summary>
        /// requests random set of sources for the given theory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRandomize_Click(object sender, EventArgs e)
        {
            Theories sourceCounter = new Theories();
            Random rand = new Random();
            int numSources = sourceCounter.GetAllSourcesForTheory(theoryId).Length;
            //get the number of pages 
            numSources = numSources % Common.Data.PageSize;    
            //choose a random page
            lastPageIndex = rand.Next(0,numSources);
            //go to random page
            Response.Redirect(Common.Pages.Theory + Common.Symbols.Question +
                    Common.QueryStrings.TheoryId + Common.Symbols.Eq + theoryId.ToString() + Common.Symbols.Amp +
                    Common.QueryStrings.PageNumber + Common.Symbols.Eq + lastPageIndex.ToString());
        }

        protected void btnExportVisualization_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("ExportVisualization.ashx?TheoryId={0}", theoryId), true);
        }

        protected void btnExportTrain_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("ExportTrain.ashx?TheoryId={0}", theoryId), true);
        }

        protected void btnExportClassify_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("ExportClassify.ashx?TheoryId={0}", theoryId));
        }
    }
}