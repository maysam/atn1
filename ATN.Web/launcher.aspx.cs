﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATN.Data;
using ATN.Crawler;

namespace ATN.Web
{
     public partial class launcher : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Set_Initial_Data_Source_Row();
            }
        }
        private void Set_Initial_Data_Source_Row()
        {

            DataTable dt = new DataTable();
            DataRow dr = null;
            
            dt.Columns.Add(new DataColumn("MsAcademicSearchId1", typeof(string)));
            dt.Columns.Add(new DataColumn("MsAcademicSearchId2", typeof(string)));
           
            dr = dt.NewRow();
            
            dr["MsAcademicSearchId1"] = string.Empty;
            dr["MsAcademicSearchId2"] = string.Empty;
           
            dt.Rows.Add(dr);

            //Store the DataTable in ViewState
            ViewState["CurrentTable"] = dt;

            DataSourceGrid.DataSource = dt;
            DataSourceGrid.DataBind();
        }

        /*protected void AddNewDataSourceID(object sender, EventArgs e)
        {
            TemplateField tf = new TemplateField();
            tf.HeaderTemplate = new GridViewTemplate(DataControlRowType.Header, "Col1", "Int32");
            tf.ItemTemplate = new GridViewLabelTemplate(DataControlRowType.DataRow, "Col1", "Int32");
            DataSourceGrid.Columns.Insert(2, tf);
        }
         * */
        protected void AddNewDataSourceToGrid(object sender, EventArgs e)
        {

            int rowIndex = 0;
            if (ViewState["CurrentTable"] != null)
            {
                DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
                DataRow drCurrentRow = null;
                if (dtCurrentTable.Rows.Count > 0)
                {
                    for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                    {
                        //extract the TextBox values
                        TextBox box1 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[1].FindControl("txtMasId1");
                        TextBox box2 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[2].FindControl("txtMasId2");
                        
                        drCurrentRow = dtCurrentTable.NewRow();
                        drCurrentRow["MsAcademicSearchId1"] = box1.Text;
                        drCurrentRow["MsAcademicSearchId2"] = box2.Text;
                        
                        rowIndex++;
                    }

                    //add new row to DataTable
                    dtCurrentTable.Rows.Add(drCurrentRow);
                    //Store the current data to ViewState
                    ViewState["CurrentTable"] = dtCurrentTable;

                    //Rebind the Grid with the current data
                    DataSourceGrid.DataSource = dtCurrentTable;
                    DataSourceGrid.DataBind();
                }
            }
            else
            {
                Response.Write("ViewState is null");
            }

            //Set Previous Data on Postbacks
            SetPreviousData();
        }

        private void SetPreviousData()
        {

            int rowIndex = 0;
            if (ViewState["CurrentTable"] != null)
            {
                DataTable dt = (DataTable)ViewState["CurrentTable"];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        TextBox box1 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[1].FindControl("txtMasId1");
                        TextBox box2 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[2].FindControl("txtMasId2");
                        

                        box1.Text = dt.Rows[i]["MsAcademicSearchId1"].ToString();
                        box2.Text = dt.Rows[i]["MsAcademicSearchId2"].ToString();
                        
                        rowIndex++;

                    }
                }
            }
        }


        /// <summary>
        /// launches the web crawler with the data the user entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_LaunchCrawler(object sender, CommandEventArgs e)
        {
            string TheoryName;
            string TheoryComment;
            TheoryName = txtNetworkName.Text;
            TheoryComment = txtNetworkComments.Text;

           
            TextBox txtMSAcademicSearchIdTemp;
            TextBox txtMSAcademicSearchIdTemp2;
            int CrawlIntervalInDays=0;
            string CrawlIntervalstringValue= crawlperiod.SelectedValue;

            string[] OnePaperMsAcademicSearchIds = new string[DataSourceGrid.Columns.Count];
            string[] OnePaperMsAcademicSearchIds2 = new string[DataSourceGrid.Columns.Count];


            //string[][] AllPapersMsAcademicSearchIds = new string[DataSourceGrid.Rows.Count][DataSourceGrid.GridViewColumnsGenerator.Count];
           
        
            //header row makes iterator 1, footer row makes iterator only go up to count - 1
            for (int itr = 0; itr < DataSourceGrid.Rows.Count; itr++)
            {
                for (int itr1 = 0; itr1 < DataSourceGrid.Columns.Count; itr1++)
                {
                    txtMSAcademicSearchIdTemp = DataSourceGrid.Rows[itr].Cells[itr1].FindControl("txtMasId1") as TextBox;
                    txtMSAcademicSearchIdTemp2 = DataSourceGrid.Rows[itr].Cells[itr1].FindControl("txtMasId2") as TextBox;


                    OnePaperMsAcademicSearchIds[itr1] = txtMSAcademicSearchIdTemp.Text;
                    OnePaperMsAcademicSearchIds2[itr1] = txtMSAcademicSearchIdTemp2.Text;
                }
            }

            if(recrawl.Checked){
                CrawlIntervalInDays = int.Parse(CrawlIntervalstringValue);

                }

            //prepare the datasource and specifications for the crawl
            CanonicalDataSource MASIds = new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, OnePaperMsAcademicSearchIds);
            CanonicalDataSource MASIds2 = new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, OnePaperMsAcademicSearchIds2);
            NewCrawlSpecifier CrawlSpecifier = new NewCrawlSpecifier(TheoryName,TheoryComment, MASIds, MASIds2);
            //start a new crawl
            CrawlRunner NewCrawler = new CrawlRunner();
            NewCrawler.StartNewCrawl(CrawlSpecifier, CrawlIntervalInDays);

            //pop an alert box to test if information is being passed correctly

            //string crawlerString;
            //crawlerString = "alert('" +
            //        txtNetworkName.Text + "\\n" +
            //        txtNetworkComments.Text + "\\n" +
            //        "paper name     MS Academic Search ID \\n";

            //for (int itr = 0; itr < paperNames.Count(); itr++)
            //{
            //    crawlerString += paperNames[itr] + "    " + msAcademicSearchIds[itr] + "\\n";
            //}
            //crawlerString += "');";

            //ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", crawlerString, true);
        }

    }
}