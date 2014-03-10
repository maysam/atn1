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
        bool validTheory = Int32.TryParse(Request["TheoryId"], out TheoryId);
        if (validTheory) {
            Theories t = new Theories();
            Theory TheoryToGet = t.GetTheory(TheoryId);
            int mas_count = 0;
            int wok_count = 0;
            DataTable dtCurrentTable = new DataTable();
            dtCurrentTable.Columns.Add(new DataColumn("txtMasId1", typeof(string)));
            dtCurrentTable.Columns.Add(new DataColumn("readonly", typeof(string)));
            DataTable dtCurrentTable2 = new DataTable();
            dtCurrentTable2.Columns.Add(new DataColumn("txtWokId1", typeof(string)));
            dtCurrentTable2.Columns.Add(new DataColumn("readonly", typeof(string)));
            
            foreach (TheoryDefinition td in TheoryToGet.TheoryDefinitions)
            {
                if (td.DataSourceId == 1)
                {
                    mas_count++;
                    DataRow drCurrentRow = dtCurrentTable.NewRow();
                    drCurrentRow["txtMasId1"] = td.CanonicalIds;
                    drCurrentRow["readonly"] = true;
                    dtCurrentTable.Rows.Add(drCurrentRow);
                }
                else
                if (td.DataSourceId == 2)
                {
                    wok_count++;
                    DataRow drCurrentRow = dtCurrentTable2.NewRow();
                    drCurrentRow["txtWokId1"] = td.CanonicalIds;
                    drCurrentRow["readonly"] = true;
                    dtCurrentTable2.Rows.Add(drCurrentRow);
                }
            }
                    //Store the current data to ViewState
            ViewState["CurrentTable1"] = dtCurrentTable;
            GridView1.DataSource = dtCurrentTable;
            GridView1.DataBind();
            ViewState["CurrentTable2"] = dtCurrentTable2;
            GridView2.DataSource = dtCurrentTable2;
            GridView2.DataBind();

            Menu1.Items[0].Text = "MAS (" + mas_count + ")";
            Menu1.Items[1].Text = "WOK (" + wok_count + ")";

            if (!Page.IsPostBack)
            {
                Menu1.Items[0].Selected = true;
                btnForceAnalysis.Visible = true;

                txtNetworkName.Text = TheoryToGet.TheoryName;
                txtNetworkComments.Text = TheoryToGet.TheoryComment;

                Crawl TheoryCrawl = TheoryToGet.Crawl.SingleOrDefault();
                if (TheoryCrawl != null && TheoryCrawl.CrawlIntervalDays.HasValue)
                {
                    crawlperiod.SelectedValue = TheoryCrawl.CrawlIntervalDays.Value.ToString();
                }

                Button1.Visible = false;
                Button2.Visible = false;

                AEF.Checked = TheoryToGet.ArticleLevelEigenfactor;
                ImpactFactor.Checked = TheoryToGet.TheoryAttributionRatio;
                TAR.Checked = TheoryToGet.TheoryAttributionRatio;
                DataMining.Checked = TheoryToGet.DataMining;
                Clustring.Checked = TheoryToGet.Clustering;

                btnSubmit1.Text = "Update Crawl";
            }
        }
        if (!Page.IsPostBack &&  !validTheory)
        {
            Set_Initial_Data_Source_Row();
            crawlperiod.SelectedValue = "30";
            Menu1.Items[0].Selected = true;
        }
    }

    private void Set_Initial_Data_Source_Row()
    {

        DataTable dt = new DataTable();
        DataRow dr = null;

        dt.Columns.Add(new DataColumn("txtMasId1", typeof(string)));
        dt.Columns.Add(new DataColumn("readonly", typeof(string)));
        
        dr = dt.NewRow();
        dr["txtMasId1"] = string.Empty;
        dt.Rows.Add(dr);

            //Store the DataTable in ViewState
        ViewState["CurrentTable1"] = dt;

        GridView1.DataSource = dt;
        GridView1.DataBind();

        DataTable dt2 = new DataTable();
        DataRow dr2 = null;
        dt2.Columns.Add(new DataColumn("txtWokId1", typeof(string)));
        dt2.Columns.Add(new DataColumn("readonly", typeof(string)));
        
        dr2 = dt2.NewRow();
        dr2["txtWokId1"] = string.Empty;
        dt2.Rows.Add(dr2);

            //Store the DataTable in ViewState
        ViewState["CurrentTable2"] = dt2;

        GridView2.DataSource = dt2;
        GridView2.DataBind();
    }
    
        /// <summary>
        /// adds a new text field for adding a new canonical source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    protected void AddNewDataSourceToGrid1(object sender, EventArgs e)
    {
        if (ViewState["CurrentTable1"] != null)
        {
            DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable1"];
            for (int rowIndex = 0; rowIndex < dtCurrentTable.Rows.Count; rowIndex++)
            {
                TextBox box1 = (TextBox)GridView1.Rows[rowIndex].Cells[0].FindControl("txtMasId1");
                dtCurrentTable.Rows[rowIndex]["txtMasId1"] = box1.Text;
            }
            DataRow drCurrentRow = dtCurrentTable.NewRow();
                //add new row to DataTable
            dtCurrentTable.Rows.Add(drCurrentRow);
                //Store the current data to ViewState
            ViewState["CurrentTable1"] = dtCurrentTable;

                //Rebind the Grid with the current data
            GridView1.DataSource = dtCurrentTable;
            GridView1.DataBind();
        }
        else
        {
            Response.Write("ViewState is null");
        }
    }

    protected void AddNewDataSourceToGrid2(object sender, EventArgs e)
    {
        if (ViewState["CurrentTable2"] != null)
        {
            DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable2"];
            for (int rowIndex = 0; rowIndex < dtCurrentTable.Rows.Count; rowIndex++)
            {
                TextBox box1 = (TextBox)GridView2.Rows[rowIndex].Cells[0].FindControl("txtWokId1");
                dtCurrentTable.Rows[rowIndex]["txtWokId1"] = box1.Text;
            }
            DataRow drCurrentRow = dtCurrentTable.NewRow();
                //add new row to DataTable
            dtCurrentTable.Rows.Add(drCurrentRow);
                //Store the current data to ViewState
            ViewState["CurrentTable2"] = dtCurrentTable;
                //Rebind the Grid with the current data
            GridView2.DataSource = dtCurrentTable;
            GridView2.DataBind();
        }
        else
        {
            Response.Write("ViewState is null");
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
            Theories t = new Theories();
            Theory TheoryToGet = t.GetTheory(TheoryId);
            Crawl TheoryCrawl = TheoryToGet.Crawl.SingleOrDefault();
            TheoryToGet.TheoryName = txtNetworkName.Text;
            TheoryToGet.TheoryComment = txtNetworkComments.Text;

            if (TheoryCrawl != null && TheoryCrawl.CrawlIntervalDays.HasValue)
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

                List<string[]> AllPapersIDs1 = new List<string[]>(); // list of arrays of all sources, each list item represents one source id/ids
                List<string[]> AllPapersIDs2 = new List<string[]>(); // list of arrays of all sources, each list item represents one source id/ids
                
                TextBox txtMSAcademicSearchIdTemp;

                //header row makes iterator 1, footer row makes iterator only go up to count - 1
                for (int itr = 0; itr < GridView1.Rows.Count; itr++)
                {
                    txtMSAcademicSearchIdTemp = GridView1.Rows[itr].Cells[0].FindControl("txtMasId1") as TextBox;
                    numberOfIds = txtMSAcademicSearchIdTemp.Text.Split(',').Length; // get how many id's are entered for a single paper
                    string[] OnePaperMsAcademicSearchIds = new string[numberOfIds]; // an array to save all single paper 
                    tempsingleLine = txtMSAcademicSearchIdTemp.Text; 
                    OnePaperMsAcademicSearchIds = tempsingleLine.Split(','); // split the single line of ids into multiple strings each represent one ID
                    if (tempsingleLine != string.Empty && OnePaperMsAcademicSearchIds.Length > 0)
                    {
                        AllPapersIDs1.Add(OnePaperMsAcademicSearchIds); // add the array of Ids for a single paper to the whole list all papers arrays
                    }
                }

                //header row makes iterator 1, footer row makes iterator only go up to count - 1
                for (int itr = 0; itr < GridView2.Rows.Count; itr++)
                {
                    txtMSAcademicSearchIdTemp = GridView2.Rows[itr].Cells[0].FindControl("txtWokId1") as TextBox;
                    numberOfIds = txtMSAcademicSearchIdTemp.Text.Split(',').Length; // get how many id's are entered for a single paper
                    string[] OnePaperMsAcademicSearchIds = new string[numberOfIds]; // an array to save all single paper 
                    tempsingleLine = txtMSAcademicSearchIdTemp.Text; 
                    OnePaperMsAcademicSearchIds = tempsingleLine.Split(','); // split the single line of ids into multiple strings each represent one ID
                    if (tempsingleLine != string.Empty && OnePaperMsAcademicSearchIds.Length > 0)
                    {
                        AllPapersIDs2.Add(OnePaperMsAcademicSearchIds); // add the array of Ids for a single paper to the whole list all papers arrays
                    }
                }

                CrawlIntervalInDays = int.Parse(CrawlIntervalstringValue);

                //prepare the datasource and specifications for the crawl
                CanonicalDataSource[] AllSourcesArray = new CanonicalDataSource[AllPapersIDs1.Count + AllPapersIDs2.Count];

                //create an array of all CanonicalDataSources so it could be passed to newCrawlSpecifier
                
                for (int itr = 0; itr < AllPapersIDs1.Count; itr++)
                {

                    AllSourcesArray[itr] = new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, AllPapersIDs1[itr]);
                }
                for (int itr = 0; itr < AllPapersIDs2.Count; itr++)
                {

                    AllSourcesArray[itr+AllPapersIDs1.Count] = new CanonicalDataSource(CrawlerDataSource.WebOfKnowledge, AllPapersIDs2[itr]);
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

        protected void btnForceAnalysis_Click(object sender, EventArgs e)
        {
            int TheoryId;
            if (Int32.TryParse(Request["TheoryId"], out TheoryId))
            {
                CrawlerProgress cp = new CrawlerProgress();
                cp.SetTheoryChanged(TheoryId);
            }
        }

        protected void Menu1_MenuItemClick(object sender, MenuEventArgs e)
        {
            MultiView1.ActiveViewIndex = Int32.Parse(e.Item.Value);
        }
    }
}
