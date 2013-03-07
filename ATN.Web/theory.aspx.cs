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

        protected void Page_Load(object sender, EventArgs e)
        {
            theoryId = (Request.QueryString[Common.QueryStrings.TheoryId] != null) ?
                Convert.ToInt32(Request.QueryString[Common.QueryStrings.TheoryId]) : 1;

            Theories sourceRetriever = new Theories();
            Theory theoryRetriever = new Theory();
            Source[] firstLevelSources = sourceRetriever.GetFirstLevelSourcesForTheory(theoryId);
            
            
            //lblNetworkName.Text = 
            grdFirstLevelSources.DataSource = firstLevelSources;
            grdFirstLevelSources.DataBind();

        }

        protected void grdFirstLevelSources_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {

            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
            else
            {

            }
        }
        protected void btnSubmit_OnClientClick(object sender, EventArgs e)
        {

        }
    }
}