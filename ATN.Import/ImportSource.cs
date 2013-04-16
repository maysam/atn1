using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace ATN.Import
{
    public class ImportSource
    {
        public string SourceId { get; set; }
        public string MasID { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Authors { get; set; }
        public string Journal { get; set; }
        public string IsContributing { get; set; }
        public string IsMetaAnalysis { get; set; }
        public string MemberOfMetaAnalyses {get; set;}
    }

    public sealed class ImportSourceMap : CsvClassMap<ImportSource>
    {
        public ImportSourceMap()
        {
            Map(m => m.SourceId).Name("Source ID");
            Map(m => m.MasID).Name("MAS ID");
            Map(m => m.Title).Name("Title");
            Map(m => m.Year).Name("Year");
            Map(m => m.Authors).Name("Authors");
            Map(m => m.Journal).Name("Journal");
            Map(m => m.IsContributing).Name("Contributing (Yes/No)");
            Map(m => m.IsMetaAnalysis).Name("Is Meta Analysis (Yes/No)");
            Map(m => m.MemberOfMetaAnalyses).Name("Meta Analysis Members (MAS IDs)");

        }
    }
}
