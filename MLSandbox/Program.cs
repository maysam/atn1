using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ATN.Data;
using ATN.Analysis;
using ATN.Export;

namespace MLSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            int TheoryId = 2;

            AnalysisRunner ar = new AnalysisRunner();
            CrawlerProgress cp = new CrawlerProgress();
            Crawl c = cp.GetCrawls().SingleOrDefault(ic => ic.TheoryId == TheoryId);
            ar.AnalyzeTheory(c, TheoryId);

            string DecisionTree = Environment.CurrentDirectory + "\\" + TheoryId + "-model.dat";
            string TrainArff = Environment.CurrentDirectory + "\\" + TheoryId + "-train.arff";
            string ClassifyArff = Environment.CurrentDirectory + "\\" + TheoryId + "-classify.arff";
            string ClassifyOutput = Environment.CurrentDirectory + "\\" + TheoryId + "-classified.txt";

            Theories t = new Theories();

            var AllExtendedSourcesForTheory = t.GetAllExtendedSourcesForTheory(TheoryId, 0, Int32.MaxValue).ToArray();
            var TrainSources = AllExtendedSourcesForTheory.Where(s => s.Contributing.HasValue && s.Depth < 3).ToArray();
            var ClassifySources = AllExtendedSourcesForTheory.Where(s => !s.Contributing.HasValue).ToArray();
            
            FileStream TrainStream = File.Open(TrainArff, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            ARFFExporter.Export(TrainSources, TheoryId, TrainStream);

            FileStream ClassifyStream = File.Open(ClassifyArff, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            ARFFExporter.Export(ClassifySources, TheoryId, ClassifyStream);

            MachineLearning.GenerateDecisionTree(TrainArff, DecisionTree);
            MachineLearning.ClassifyData(DecisionTree, ClassifyArff, ClassifyOutput);
            //MachineLearning.ParseClassificationOutput(@"soybean-classification.txt");
        }
    }
}
