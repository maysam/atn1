using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATN.Crawler.WebCrawler;
using ATN.Data;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace ATN.Import
{
    public class ImportManualMetaAnalysis
    {
        private Theories _theories;
        private Sources _sources;
        private ICrawler _crawler;
        public ImportManualMetaAnalysis()
        {
            _theories = new Theories();
            _sources = new Sources();
            _crawler = new MASCrawler();
        }
        public void ImportTheory(int TheoryId, Stream SourceStream)
        {
            StreamReader FileReader = new StreamReader(SourceStream);

            CsvReader csv = new CsvReader(FileReader);
            csv.Configuration.ClassMapping<ImportSourceMap, ImportSource>();

            IEnumerable<ImportSource> ImportSources = csv.GetRecords<ImportSource>().ToList();

            int i = 1;
            int Count = ImportSources.Count();
            Dictionary<string, long> HumanMetaAnalysisIdentifierToSourceId = new Dictionary<string, long>();
            foreach (ImportSource SourceToImport in ImportSources)
            {
                Trace.WriteLine(string.Format("Writing import source {0}/{1}", i, Count));
                i++;
                long SourceId;
                //Handle meta analysis creation
                if (SourceToImport.IsMetaAnalysis == "Yes" || SourceToImport.IsContributing == "Yes")
                {
                    if (!Int64.TryParse(SourceToImport.SourceId, out SourceId))
                    {
                        //The source does not exist
                        CompleteSource SourceToComplete = new CompleteSource();
                        SourceToComplete.Source = new Source();
                        SourceToComplete.Source.ArticleTitle = SourceToImport.Title;
                        SourceToComplete.Source.Year = SourceToImport.Year == "0" || SourceToImport.Year == "None" ? (int?)null : (int?)Int32.Parse(SourceToImport.Year);
                        SourceToComplete.Journal = new Journal();
                        SourceToComplete.Journal.JournalName = SourceToImport.Journal;
                        SourceToComplete.Source.DataSourceId = (int)CrawlerDataSource.Human;
                        SourceToComplete.Source.DataSourceSpecificId = Guid.NewGuid().ToString();
                        SourceToComplete.Source.SerializedDataSourceResponse = "<Response />";

                        string[] AuthorNames = SourceToImport.Authors.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        string[][] AuthorLastFirst = AuthorNames.Select(a => a.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)).ToArray();
                        List<Author> Authors = new List<Author>();
                        /*foreach (string[] Author in AuthorLastFirst)
                        {
                            Author AuthorToAdd = new Author();
                            if (Author.Length == 2)
                            {
                                AuthorToAdd.LastName = Author[1];
                                AuthorToAdd.FirstName = Author[0];
                            }
                            else if (Author.Length == 1)
                            {
                                AuthorToAdd.FirstName = string.Empty;
                                AuthorToAdd.LastName = Author[0];
                            }
                            else
                            {
                                AuthorToAdd.FirstName = Author[0];
                                AuthorToAdd.LastName = Author[1] + " " + Author[2];
                                int x = 0;
                            }
                            AuthorToAdd.DataSourceId = (int)CrawlerDataSource.Human;
                            AuthorToAdd.DataSourceSpecificId = Guid.NewGuid().ToString();
                            Authors.Add(AuthorToAdd);
                        }*/
                        SourceToComplete.Authors = Authors.ToArray();
                        SourceToComplete.Subjects = new Subject[0];
                        SourceToComplete.IsDetached = true;
                        Source RetrievedSource = _sources.AddDetachedSource(SourceToComplete);
                        SourceToImport.SourceId = RetrievedSource.SourceId.ToString();
                        _theories.AddManualTheoryMember(TheoryId, RetrievedSource.SourceId);
                        if (SourceToImport.IsMetaAnalysis == "Yes")
                        {
                            _theories.MarkSourceMetaAnalysis(TheoryId, RetrievedSource.SourceId);
                            HumanMetaAnalysisIdentifierToSourceId.Add(SourceToImport.MasID, RetrievedSource.SourceId);
                        }
                    }
                    uint[] MetaAnalysisIDs = SourceToImport.MemberOfMetaAnalyses.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Where(u => u.Trim() != string.Empty).Select(u => uint.Parse(u)).ToArray();

                    foreach (uint MetaAnalysisMember in MetaAnalysisIDs)
                    {
                        Source s;
                        if (!HumanMetaAnalysisIdentifierToSourceId.ContainsKey(MetaAnalysisMember.ToString()))
                        {
                            s = _sources.GetSourceByDataSourceSpecificId(CrawlerDataSource.MicrosoftAcademicSearch, MetaAnalysisMember.ToString());
                            if (s == null)
                            {
                                CompleteSource SourceToAdd = _crawler.GetSourceById(MetaAnalysisMember.ToString());
                                s = _sources.AddDetachedSource(SourceToAdd);
                            }
                        }
                        else
                        {
                            s = _sources.GetSourceById(HumanMetaAnalysisIdentifierToSourceId[MetaAnalysisMember.ToString()]);
                        }
                        _theories.MarkSourceMetaAnalysis(TheoryId, s.SourceId);
                        _theories.MarkMetaAnalysisMember(TheoryId, s.SourceId, Int64.Parse(SourceToImport.SourceId), true);
                    }
                }
            }
        }
    }
}
