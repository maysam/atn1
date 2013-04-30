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

        protected void Page_Load(object sender, EventArgs e)
        {
            TheoryId = Int32.Parse(Request["TheoryId"]);
            txtTheoryId.Text = Request["TheoryId"]; 
            //txtTheoryId.Text = "123";
           

            searchlnk.PostBackUrl = Common.Pages.Theory + Common.Symbols.Question + Common.QueryStrings.TheoryId + Common.Symbols.Eq + TheoryId.ToString();



        }


        protected void Submit_Click(object sender, EventArgs e)
        {

            // tp store the entered title 
            //string TheoryId;  //to store the entered theory id
            long newlyAddedSourceId; // to store the Id of the newly added source

            //Author newAuthor= new Author();
            //Authors newAuthors = new Authors(); // = new Authors();
            // Author doesn't have enpugh properties

            Source sourceNew = new Source(); // store the source for completed source parameter
            Sources theorySources = new Sources(); // to add the newly created source to
            Theories theoriesnew = new Theories();


            //TheoryId = txtTheoryId.Text;
            //int intTheoryId = int.Parse(TheoryId); // store theory id provided

            sourceNew.ArticleTitle = txtTitle.Text;
           


            CompleteSource newSource = new CompleteSource(sourceNew, new Author[0], null);


            newSource.IsDetached = true;



            theorySources.AddDetachedSource(newSource); //add new source to our new sources collection
            newlyAddedSourceId = theorySources.AddDetachedSource(newSource).SourceId;
            // newlyAddedSourceId= theoriesnew.GetCanonicalSourcesForTheory(intTheoryId).AddDetachedSource(newSource).SourceId;

            theoriesnew.AddManualTheoryMember(TheoryId, newlyAddedSourceId);


            if (IsmetAnalysis.Checked)
            {
                theoriesnew.MarkSourceMetaAnalysis(TheoryId, newlyAddedSourceId);





                string singleId;
                int intID;
                int num = citations.Items.Count;

                for (int index = 2; index <= num; index++)
                {
                    citations.SelectedIndex = index;
                    singleId = citations.SelectedItem.Text;
                    intID = int.Parse(singleId);

                    theorySources.AddCitation(newlyAddedSourceId, intID);
                }
            }


        }

        protected void metAnalysis_CheckedChanged(object sender, EventArgs e)
        {
            if (IsmetAnalysis.Checked)
            {
                citations.Enabled = true;
                //CitationPanel.ForeColor = System.Drawing.Color.Black; 

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


        protected void Add_citation(object sender, EventArgs e)
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

        

        protected void citations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (citations.SelectedIndex == 1)
            {

                CitationPanel.Enabled = true;
            }
          
        }

       









    }
}