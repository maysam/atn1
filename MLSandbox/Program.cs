using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ATN.Data;
using ATN.Analysis;

namespace MLSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream TrainBatStream = File.Open(@"C:\Users\fault_000\Documents\WEKAtest\train_tree.bat", FileMode.Create);
            FileStream ClassifyBatStream = File.Open(@"C:\Users\fault_000\Documents\WEKAtest\classify_data.bat", FileMode.Create);

            MachineLearning.GenerateWekaTrainBatchFile(
                @"C:\Users\fault_000\Documents\WEKAtest\soybean-model.dat",
                @"C:\Users\fault_000\Documents\WEKAtest\arff\soybean-train.arff",
                TrainBatStream);
            MachineLearning.GenerateWekaClassificationBatchFile(
                @"C:\Users\fault_000\Documents\WEKAtest\soybean-model.dat",
                @"C:\Users\fault_000\Documents\WEKAtest\arff\soybean-test-noclass.arff",
                @"C:\Users\fault_000\Documents\WEKAtest\soybean-classification.txt",
                ClassifyBatStream);
            MachineLearning.GenerateDecisionTree(@"C:\Users\fault_000\Documents\WEKAtest", "train_tree.bat");
            MachineLearning.ClassifyData(@"C:\Users\fault_000\Documents\WEKAtest", "classify_data.bat");
            MachineLearning.ParseClassificationOutput(@"C:\Users\fault_000\Documents\WEKAtest\soybean-classification.txt");

            //FileStream DestinationARFFStream = File.Open("atn-train.arff", FileMode.Create);
            //ARFFExporter.ExportTrain(Nodes.Values.ToArray(), 6, DestinationARFFStream);
        }
    }
}
