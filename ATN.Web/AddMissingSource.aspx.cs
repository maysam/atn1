using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATN.Data;
using System.Data.Objects.DataClasses;

namespace ATN.Web
{
    public partial class AddMissingSource : System.Web.UI.Page
    {

        int TheoryId;
        long newlyAddedSourceId; // to store the Id of the newly added source

        protected void Page_Load(object sender, EventArgs e)
        {
            TheoryId = Int32.Parse(Request["TheoryId"]);
            txtTheoryId.Text = Request["TheoryId"]; 
            //txtTheoryId.Text = "123";
           
            searchlnk.NavigateUrl= Common.Pages.Theory + Common.Symbols.Question + Common.QueryStrings.TheoryId + Common.Symbols.Eq + TheoryId.ToString();


        }


        protected void Submit_Click(object sender, EventArgs e)
        {


            
            Source sourceNew = new Source(); // to store the source for completed source parameter
            Theories theoriesnew = new Theories();
            Sources theorySources =  new Sources();

            sourceNew.DataSourceId = (int)CrawlerDataSource.Human;
            sourceNew.ArticleTitle = txtTitle.Text;
            sourceNew.Year = int.Parse(txtYear.Text);
            sourceNew.DataSourceSpecificId = Guid.NewGuid().ToString();
            sourceNew.SerializedDataSourceResponse = "<Response />";
      

            CompleteSource newSource = new CompleteSource(sourceNew, new Author[0], null);
            newSource.Subjects = new Subject[0];
            newSource.IsDetached = true;
            

            sourceNew= theorySources.AddDetachedSource(newSource); //add new source to our new sources collection
            newlyAddedSourceId = sourceNew.SourceId;
            
            
            
            theoriesnew.AddManualTheoryMember(TheoryId, newlyAddedSourceId);


            if (IsmetAnalysis.Checked)
            {
                theoriesnew.MarkSourceMetaAnalysis(TheoryId, newlyAddedSourceId);



                string singleId;
                long longID;
                List<string> str = new List<string>();

                foreach (ListItem current in citations.Items)
                {
                    str.Add(current.Text);
                }

                foreach ( string sourcestr in str){

                    singleId = sourcestr;
                    
                    longID = int.Parse(singleId);

                    theorySources.AddCitation(newlyAddedSourceId, longID);
                    
                }
            }


            theoriesnew.SaveTheory();

            
        }

        protected void metAnalysis_CheckedChanged(object sender, EventArgs e)
        {
            if (IsmetAnalysis.Checked)
            {
                citations.Enabled = true;
                addById.Enabled = true;

                lblCitations.ForeColor = System.Drawing.Color.Black;
                citations.ForeColor = System.Drawing.Color.Black;
                citations.BorderColor = System.Drawing.Color.Black;




                /* if (citations.SelectedValue == "add")
                 {

               

                 }*/



            }

            else
            {
                citations.Enabled = false;
                addById.Enabled = false;
             
                // CitationPanel.ForeColor = System.Drawing.Color.Silver;
                lblCitations.ForeColor = System.Drawing.Color.Silver;
                citations.ForeColor = System.Drawing.Color.Silver;
                citations.BorderColor = System.Drawing.Color.Silver;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        protected void Add_citation_to_list(object sender, EventArgs e)
        {

            /*Source newCitationSource= new Source();
            int citationId;
            citationId= int.Parse( addById.Text);
            ListItem newCitation= new ListItem(); 
            newCitation.Text= citationId;
 
            citations.Items.Add(newCitation);
             * 
               */

            string citationId;
            citationId = addById.Text;
            ListItem newCitation = new ListItem();
            newCitation.Text = citationId;

            citations.Items.Add(newCitation);
        }

        
/*
        protected void citations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (citations.SelectedIndex == 1)
            {

                CitationPanel.Enabled = true;
            }
          
        }
*/
       









    }
}