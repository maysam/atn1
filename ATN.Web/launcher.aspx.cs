using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATN.Data;
using ATN.Crawler;
using System.IO;

namespace ATN.Web
{
    /// <summary>
    /// This page launches a new Crawl, or update an existing one
    /// </summary>
     public partial class launcher : System.Web.UI.Page
    {
         /// <summary>
         /// retrieves data if a crawl exists, or create blank fields
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            //fetching data of  the theory to be recrawled
            int TheoryId;
            if (Int32.TryParse(Request["TheoryId"], out TheoryId) && !IsPostBack)
            {
                btnForceAnalysis.Visible = true;
                DataSourceGrid.Visible = false;
                btnNewSource.Visible = false;

                Theories t = new Theories();
                Theory TheoryToGet = t.GetTheory(TheoryId);
                Crawl TheoryCrawl = TheoryToGet.Crawl.SingleOrDefault();
                txtNetworkName.Text = TheoryToGet.TheoryName;
                txtNetworkComments.Text = TheoryToGet.TheoryComment;

                if (TheoryCrawl.CrawlIntervalDays.HasValue)
                {
                    crawlperiod.SelectedValue = TheoryCrawl.CrawlIntervalDays.Value.ToString();
                }



                AEF.Checked = TheoryToGet.ArticleLevelEigenfactor;
                ImpactFactor.Checked = TheoryToGet.TheoryAttributionRatio;
                TAR.Checked = TheoryToGet.TheoryAttributionRatio;
                DataMining.Checked = TheoryToGet.DataMining;
                Clustring.Checked = TheoryToGet.Clustering;

                btnSubmit1.Text = "Update Crawl";
            }
            else if (!Page.IsPostBack)
            {
                Set_Initial_Data_Source_Row();
            }
        }
        private void Set_Initial_Data_Source_Row()
        {

            DataTable dt = new DataTable();
            DataRow dr = null;
            
            dt.Columns.Add(new DataColumn("MsAcademicSearchId1", typeof(string)));
           // dt.Columns.Add(new DataColumn("MsAcademicSearchId2", typeof(string)));
           
            dr = dt.NewRow();
            
            dr["MsAcademicSearchId1"] = string.Empty;
            //dr["MsAcademicSearchId2"] = string.Empty;
           
            dt.Rows.Add(dr);

            //Store the DataTable in ViewState
            ViewState["CurrentTable"] = dt;

            DataSourceGrid.DataSource = dt;
            DataSourceGrid.DataBind();
        }

 
        /// <summary>
        /// adds a new text field for adding a new canonical source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                            drCurrentRow = dtCurrentTable.NewRow();

  
                            //extract the TextBox values
                            TextBox box1 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[0].FindControl("txtMasId1");
                          
                           drCurrentRow = dtCurrentTable.NewRow();
                            drCurrentRow["MsAcademicSearchId1"] = box1.Text;
                       
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

         /// <summary>
         /// save the entered data so they could be retreived when a state changes
         /// </summary>

        private void SetPreviousData(){

            int rowIndex = 0;
            if (ViewState["CurrentTable"] != null)
            {
                DataTable dt = (DataTable)ViewState["CurrentTable"];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        
                        TextBox box1 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[0].FindControl("txtMasId1");
                      

                        box1.Text = dt.Rows[i]["MsAcademicSearchId1"].ToString();
                       



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

            int TheoryId;
            if (Int32.TryParse(Request["TheoryId"], out TheoryId))
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        File.Delete(@"C:\Program Files\A Theory Network\Processing\Processing.log");
                        break;
                    }
                    catch (IOException) { }
                }
                Theories t = new Theories();
                Theory TheoryToGet = t.GetTheory(TheoryId);
                Crawl TheoryCrawl = TheoryToGet.Crawl.SingleOrDefault();
                TheoryToGet.TheoryName = txtNetworkName.Text;
                TheoryToGet.TheoryComment = txtNetworkComments.Text;

                if (TheoryCrawl.CrawlIntervalDays.HasValue)
                {
                    TheoryCrawl.DateCrawled = DateTime.Now.AddMonths(-1);
                    TheoryCrawl.CrawlIntervalDays = Int32.Parse(crawlperiod.SelectedValue);
                }

                TheoryToGet.ArticleLevelEigenfactor = AEF.Checked;
                TheoryToGet.TheoryAttributionRatio = ImpactFactor.Checked;
                TheoryToGet.TheoryAttributionRatio = TAR.Checked;
                TheoryToGet.DataMining = DataMining.Checked;
                TheoryToGet.Clustering = Clustring.Checked;

                t.SaveTheory();
            }
            else
            {
                //retreiving user enterd data 
                string TheoryName;
                string TheoryComment;
                TheoryName = txtNetworkName.Text;
                TheoryComment = txtNetworkComments.Text;
                

                
                string tempsingleLine; //to store all Ids of a single paper


                int CrawlIntervalInDays = 0;
                string CrawlIntervalstringValue = crawlperiod.SelectedValue; 

                int numberOfIds; // to store the number of IDs for one paper

                List<string[]> AllPapersIDs = new List<string[]>(); // list of arrays of all sources, each list item represents one source id/ids
               

                TextBox txtMSAcademicSearchIdTemp;


               

                //header row makes iterator 1, footer row makes iterator only go up to count - 1
                for (int itr = 0; itr < DataSourceGrid.Rows.Count; itr++)
                {
                   

                    txtMSAcademicSearchIdTemp = DataSourceGrid.Rows[itr].Cells[0].FindControl("txtMasId1") as TextBox;
                    


                    numberOfIds = txtMSAcademicSearchIdTemp.Text.Split(',').Length; // get how many id's are entered for a single paper
                    string[] OnePaperMsAcademicSearchIds = new string[numberOfIds]; // an array to save all single paper ids


                    tempsingleLine = txtMSAcademicSearchIdTemp.Text; 

                    OnePaperMsAcademicSearchIds = tempsingleLine.Split(','); // split the single line of ids into multiple strings each represent one ID

                   


                    
                    if (tempsingleLine != string.Empty && OnePaperMsAcademicSearchIds.Length > 0)
                    {
                        AllPapersIDs.Add(OnePaperMsAcademicSearchIds); // add the array of Ids for a single paper to the whole list all papers arrays
                    }
                }

                CrawlIntervalInDays = int.Parse(CrawlIntervalstringValue);

                //prepare the datasource and specifications for the crawl
                CanonicalDataSource[] AllSourcesArray = new CanonicalDataSource[AllPapersIDs.Count];

                //create an array of all CanonicalDataSources so it could be passed to newCrawlSpecifier
                for (int itr = 0; itr < AllPapersIDs.Count; itr++)
                {

                    AllSourcesArray[itr] = new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, AllPapersIDs[itr]);
                }
                

                NewCrawlSpecifier CrawlSpecifier = new NewCrawlSpecifier(TheoryName, TheoryComment, AEF.Checked, ImpactFactor.Checked, TAR.Checked, DataMining.Checked, Clustring.Checked, AllSourcesArray);


                //start a new crawl
                CrawlRunner NewCrawler = new CrawlRunner();
                NewCrawler.StartNewCrawl(CrawlSpecifier, CrawlIntervalInDays);

                confirmation.Visible = true;
            }
        }
         /// <summary>
         /// sets availability of analysis engin part based on user choices
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>

        protected void ImpactFactor_CheckedChanged(object sender, EventArgs e)
        {

            if (ImpactFactor.Checked && AEF.Checked)
            {
                TAR.Enabled = true;
                TAR.Text = "TAR";
                TAR.ForeColor = System.Drawing.Color.Black;
                TAR.DataBind();
            }
            else
            {
                TAR.Checked = false;
                TAR.Enabled = false;
                TAR.Text = "TAR- Can't be computed";
                TAR.ForeColor = System.Drawing.Color.Silver;
                TAR.DataBind();
            }
        }


        protected void AEF_CheckedChanged(object sender, EventArgs e)
        {

            if (ImpactFactor.Checked && AEF.Checked)
            {
                TAR.Enabled = true;
                TAR.Text = "TAR";
                TAR.ForeColor = System.Drawing.Color.Black;

                TAR.DataBind();
            }
            else
            {
                TAR.Checked = false;
                TAR.Enabled = false;
                TAR.Text = "TAR- Can't be computed";
                TAR.ForeColor = System.Drawing.Color.Silver;
                TAR.DataBind();
            }

        }

        protected void btnForceAnalysis_Command(object sender, CommandEventArgs e)
        {

        }

        protected void btnForceAnalysis_Click(object sender, EventArgs e)
        {
            int TheoryId;
            if (Int32.TryParse(Request["TheoryId"], out TheoryId))
            {
                CrawlerProgress cp = new CrawlerProgress();
                cp.SetTheoryChanged(TheoryId);
            }
        }       
    }
}
