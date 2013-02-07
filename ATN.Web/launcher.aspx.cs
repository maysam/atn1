using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
        dt.Columns.Add(new DataColumn("PaperName", typeof(string)));
        dt.Columns.Add(new DataColumn("SocialCitationIndexId", typeof(string)));
        dt.Columns.Add(new DataColumn("MsAcademicSearchId", typeof(string)));
        dt.Columns.Add(new DataColumn("PubMedId", typeof(string)));

        dr = dt.NewRow();
        dr["PaperName"] = string.Empty;
        dr["SocialCitationIndexId"] = string.Empty;
        dr["MsAcademicSearchId"] = string.Empty;
        dr["PubMedId"] = string.Empty;
        dt.Rows.Add(dr);

        //Store the DataTable in ViewState
        ViewState["CurrentTable"] = dt;

        DataSourceGrid.DataSource = dt;
        DataSourceGrid.DataBind();
    }

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
                    TextBox box1 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[1].FindControl("txtPaperName");
                    TextBox box2 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[2].FindControl("txtSocialCitationIndexId");
                    TextBox box3 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[3].FindControl("txtMsAcademicSearchId");
                    TextBox box4 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[3].FindControl("txtPubMedId");

                    drCurrentRow = dtCurrentTable.NewRow();
                    drCurrentRow["PaperName"] = box1.Text;     
                    drCurrentRow["SocialCitationIndexId"] = box2.Text;
                    drCurrentRow["MsAcademicSearchId"] = box3.Text;     
                    drCurrentRow["PubMedId"] = box4.Text;     

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
                    TextBox box1 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[1].FindControl("txtPaperName");
                    TextBox box2 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[2].FindControl("txtSocialCitationIndexId");
                    TextBox box3 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[3].FindControl("txtMsAcademicSearchId");
                    TextBox box4 = (TextBox)DataSourceGrid.Rows[rowIndex].Cells[3].FindControl("txtPubMedId");


                    box1.Text = dt.Rows[i]["PaperName"].ToString();
                    box2.Text = dt.Rows[i]["SocialCitationIndexId"].ToString();
                    box3.Text = dt.Rows[i]["MsAcademicSearchId"].ToString();
                    box4.Text = dt.Rows[i]["PubMedId"].ToString();

                    rowIndex++;

                }
            }
        }
    }


    /// <summary>
    /// launches the web crawler with the given arguments
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_LaunchCrawler(object sender, CommandEventArgs e)
    {
        string networkName;
        networkName = txtNetworkName.Text;
  
        TextBox txtPaperNameTemp;
        TextBox txtMSAcademicSearchIdTemp;

        string[] paperNames = new string[DataSourceGrid.Rows.Count];
        string[] msAcademicSearchIds = new string[DataSourceGrid.Rows.Count];

        //header row makes iterator 1, footer row makes iterator only go up to count - 1
        for(int itr = 0; itr < DataSourceGrid.Rows.Count; itr++)
        {
            txtPaperNameTemp = DataSourceGrid.Rows[itr].Cells[1].FindControl("txtPaperName") as TextBox;
            paperNames[itr] = txtPaperNameTemp.Text;

            txtMSAcademicSearchIdTemp = DataSourceGrid.Rows[itr].Cells[3].FindControl("txtMsAcademicSearchId") as TextBox;
            msAcademicSearchIds[itr] = txtMSAcademicSearchIdTemp.Text;
        }
        

        string crawlerString;
        crawlerString = "alert('" +
                txtNetworkName.Text + "\\n" +
                "paper name     MS Academic Search ID \\n";

        for (int itr = 0; itr < paperNames.Count(); itr++)
        {
            crawlerString += paperNames[itr]+ "    " + msAcademicSearchIds[itr] + "\\n";
        }
        crawlerString += "');";
        

        ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", crawlerString, true);
    } 
}