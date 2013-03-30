using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;
using System.Xml;
using System.IO;

namespace ATN.Export
{
    public class Program
    {
        static void Main(string[] args)
        {
            Theories t = new Theories();
            ExtendedSource[] CanonicalSources = t.GetExtendedSourceReferencesForSource(2, 52).ToArray();

            FileStream DestinationNetStream = File.Open("SmallTheory.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(DestinationNetStream, Encoding.Unicode);
            sw.WriteLine("Source ID\tMAS ID\tTitle\tYear\tAuthors\tJournal\tContributing (Yes/No)\tIs Meta Analysis (Yes/No)\tMeta Analysis Members (MAS IDs)");
            foreach (ExtendedSource cs in CanonicalSources)
            {
                sw.Write(cs.SourceId + "\t");
                sw.Write(cs.MasID + "\t");
                sw.Write(string.Format("\"{0}\"", cs.Title.Replace("\"", "\"\"")) + "\t");
                if (cs.Year != 0)
                {
                    sw.Write(cs.Year + "\t");
                }
                else
                {
                    sw.Write("None Available\t");
                }
                if (cs.Authors != null)
                {
                    sw.Write(string.Format("\"{0}\"", cs.Authors.Remove(cs.Authors.LastIndexOf(","), 1).Replace("\"", "\"\"")) + "\t");
                }
                else
                {
                    sw.Write("None Available\t");
                }
                if (cs.Journal != null)
                {
                    sw.Write(string.Format("\"{0}\"", cs.Journal.Replace("\"", "\"\"")) + "\t");
                }
                else
                {
                    sw.Write("None\t");
                }
                sw.Write(" \t");
                sw.Write(" \t ");
                sw.Write(Environment.NewLine);
            }
            sw.Close();
        }
    }
}
