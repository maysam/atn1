using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    public class Theories : DatabaseInterface
    {
        public Theories()
        {

        }
        public Theory AddTheory(string TheoryName, params CanonicalDataSource[] CanonicalSources)
        {
            Theory TheoryToAdd = new Theory();
            TheoryToAdd.TheoryName = TheoryName;
            TheoryToAdd.DateAdded = DateTime.Now;
            Context.Theories.AddObject(TheoryToAdd);
            Context.SaveChanges();

            foreach (CanonicalDataSource CanonicalSource in CanonicalSources)
            {
                TheoryDefinition TheoryCanonicalSource = new TheoryDefinition();
                TheoryCanonicalSource.TheoryId = TheoryToAdd.TheoryId;
                TheoryCanonicalSource.DataSourceId = (int)CanonicalSource.DataSource;
                TheoryCanonicalSource.CanonicalIds = String.Join(",", CanonicalSource.CanonicalIds);
                Context.TheoryDefinitions.AddObject(TheoryCanonicalSource);
            }

            Context.SaveChanges();
            return TheoryToAdd;
        }

        public string[][] GetCanonicalPapersForTheory(int TheoryId)
        {
            return Context.TheoryDefinitions.Where(td => td.TheoryId == TheoryId).ToArray().Select(td => td.CanonicalIds.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries)).ToArray();
        }

        public Source[] GetCanonicalSourcesForTheory(int TheoryId)
        {
            Sources s = new Sources();
            TheoryDefinition[] CanonicalPapers = Context.Theories.Single(t => t.TheoryId == TheoryId).TheoryDefinitions.ToArray();
            List<Source> Sources = new List<Source>(CanonicalPapers.Length);
            foreach (TheoryDefinition t in CanonicalPapers)
            {
                Sources.Add(s.GetSourceByDataSourceSpecificIds((CrawlerDataSource)t.DataSourceId, t.CanonicalIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)));
            }
            return Sources.ToArray();
        }
    }
}
