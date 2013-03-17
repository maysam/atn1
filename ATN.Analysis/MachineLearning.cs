using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace ATN.Analysis
{
    public class MachineLearning
    {
        /// <summary>
        /// This method takes the train ARFF file exported
        /// by the corresponding methods in ATN.Export, and
        /// generates a decision tree binary file from the data.
        /// 
        /// AT THE MOMENT:
        /// This function using the soybean sample data, to decouple
        /// it from the database.
        /// </summary>
        public static void GenerateDecisionTree()
        {
            const string weka_archive_path = "weka.jar";
            const string training_data_path = "soybean-train.arff";
            const string decision_tree_path = "soybean-model.dat";

            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.CreateNoWindow = false;
            StartInfo.FileName = "java";
            StartInfo.WorkingDirectory = @"C:\Users\fault_000\";
            //StartInfo.WorkingDirectory = @"..\..\";
            StartInfo.Arguments = "-cp " +
                weka_archive_path + " weka.classifiers.trees.J48 -C 0.25 -M 1 -x 4 -t " +
                training_data_path + " -d " + decision_tree_path;

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

        /// <summary>
        /// This method takes the test ARFF file exported from the 
        /// corresponding method in ATN.Export, and the stored decision
        /// tree from GenerateDecisionTree(), and classifies the test
        /// data. The results are written to an output file.
        /// 
        /// AT THE MOMENT:
        /// This function uses the soybean sample data, to decouple
        /// it from the database.
        /// </summary>
        public static void ClassifyData()
        {
            const string weka_archive_path = "weka.jar";
            const string test_data_path = "soybean-test.arff";
            const string decision_tree_path = "soybean-model.dat";
            const string classification_outfile_path = @"C:\Users\fault_000\soybean-classification.txt";

            FileStream ClassificationStream = File.Open(classification_outfile_path, FileMode.Create);
            StreamWriter ClassificationDestination = new System.IO.StreamWriter(ClassificationStream, Encoding.ASCII);

            MemoryStream ms = new MemoryStream();
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.UseShellExecute = false;
            //StartInfo.CreateNoWindow = false;
            StartInfo.RedirectStandardOutput = true;
            StartInfo.RedirectStandardError = true;
            StartInfo.FileName = "java";
            StartInfo.WorkingDirectory = @"C:\Users\fault_000\";
            //StartInfo.WorkingDirectory = @"..\..\";
            StartInfo.Arguments = "-cp " +
                weka_archive_path + " weka.classifiers.trees.J48 -l " +
                decision_tree_path + " -T " + test_data_path + " -p 1-5";

            try
            {
                Process javaProcess = Process.Start(StartInfo);
                //javaProcess.WaitForExit();
                string stdout = javaProcess.StandardOutput.ReadToEnd();
                string stderr = javaProcess.StandardError.ReadToEnd();
                Console.Write(stdout);
                ClassificationDestination.WriteLine(stdout);
                ClassificationDestination.Close();
            }
            catch (Exception e)
            {
                Console.Write("Exception thrown: {0}", e);
            }
        }
    }
}
