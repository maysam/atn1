using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ATN.Analysis
{
    public class MachineLearning
    {
        public static void GenerateDecisionTree()
        {
            const string weka_archive_path = "C:\\Program Files\\Weka-3-6\\weka.jar";
            const string training_data_path = "..\\..\\..\\soybean-train.arff";
            const string decision_tree_path = "..\\..\\..\\soybean-model.dat";

            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.CreateNoWindow = false;
            StartInfo.FileName = "java";
            StartInfo.Arguments = "-cp \"" +
                weka_archive_path + "\" weka.classifiers.trees.J48 -C 0.25 -M 1 -x 4 -t \"" +
                training_data_path + "\" -d \"" + decision_tree_path + "\"";

            try
            {
                using (Process javaProcess = Process.Start(StartInfo))
                {
                    javaProcess.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Console.Write("Exception thrown: {0}", e);
            }
        }
    }
}
