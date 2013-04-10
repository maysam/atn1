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

        public static void GenerateWekaTrainBatchFile(string decision_tree_path, string training_data_path, Stream BatStream)
        {
            StreamWriter BatDestination = new System.IO.StreamWriter(BatStream, Encoding.ASCII);

            string weka_command = "java -cp weka.jar weka.classifiers.trees.J48 -C 0.25 -M 1 -x 4 -t \"" +
                training_data_path + "\" -d \"" + decision_tree_path + "\"\n";

            BatDestination.WriteLine(weka_command);
            BatDestination.Close();
        }

        public static void GenerateWekaClassificationBatchFile(string decision_tree_path, string test_data_path, string classification_output_path, Stream BatStream)
        {
            StreamWriter BatDestination = new System.IO.StreamWriter(BatStream, Encoding.ASCII);

            string weka_command = "java -cp weka.jar weka.classifiers.trees.J48 -l \"" + decision_tree_path + "\" -T \"" + test_data_path + "\" -p 1-4 > " + "\"" + classification_output_path + "\"\n";

            BatDestination.WriteLine(weka_command);
            BatDestination.Close();
        }
        /// <summary>
        /// This method takes the train ARFF file exported
        /// by the corresponding methods in ATN.Export, and
        /// generates a decision tree binary file from the data.
        /// 
        /// </summary>
        public static void GenerateDecisionTree(string training_data_path, string decision_tree_path)
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.UseShellExecute = false;
            StartInfo.CreateNoWindow = false;
            //StartInfo.RedirectStandardOutput = true;
            //StartInfo.RedirectStandardError = true;
            StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            StartInfo.FileName = "java.exe";
            StartInfo.Arguments = string.Format("-cp weka.jar weka.classifiers.trees.J48 -C 0.25 -M 1 -x 4 -t \"{0}\" -d \"{1}\"", training_data_path, decision_tree_path);

            using (Process batProcess = Process.Start(StartInfo))
            {
                //System.Threading.Thread.Sleep(5000);
                //batProcess.WaitForExit();
                //string stdout = batProcess.StandardOutput.ReadToEnd();
                //string stderr = batProcess.StandardError.ReadToEnd();
                batProcess.Close();
                //Console.Write(stdout);
                //Console.Write(stderr);
                //Console.Write(stdout);
            }
        }

        /// <summary>
        /// This method takes the test ARFF file exported from the 
        /// corresponding method in ATN.Export, and the stored decision
        /// tree from GenerateDecisionTree(), and classifies the test
        /// data. The results are written to the output file passed
        /// as a Stream to this method.
        /// 
        /// </summary>
        public static void ClassifyData(string decision_tree_path, string test_data_path, string classification_output_path)
        {
            //StreamWriter ClassificationDestination = new System.IO.StreamWriter(ClassificationStream, Encoding.ASCII);

            //MemoryStream ms = new MemoryStream();
            //ProcessStartInfo StartInfo = new ProcessStartInfo();
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.UseShellExecute = false;
            StartInfo.CreateNoWindow = false;
            StartInfo.RedirectStandardOutput = true;
            StartInfo.RedirectStandardError = true;
            StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            StartInfo.FileName = "java.exe";
            StartInfo.Arguments = string.Format("-cp weka.jar weka.classifiers.trees.J48 -l \"{0}\" -T \"{1}\" -p 2-8", decision_tree_path, test_data_path);

            //try
            //{
                using (Process batProcess = Process.Start(StartInfo))
                {
                    //batProcess.WaitForExit();
                    string stdout = batProcess.StandardOutput.ReadToEnd();
                    string stderr = batProcess.StandardError.ReadToEnd();
                    batProcess.Close();
                    //Console.Write(stdout);
                    //Console.Write(stderr);
                    Stream s = File.OpenWrite(classification_output_path);
                    StreamWriter sw = new StreamWriter(s);
                    sw.Write(stdout);
                    sw.Close();
                }
            //}
            //catch (Exception e)
            //{
            //    Console.Write("Exception thrown: {0}", e);
            //}
        }

        public static void ParseClassificationOutput(string classification_output_path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(classification_output_path))
                {
                    String co = sr.ReadToEnd();
                    string[] lines = co.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    foreach (string l in lines)
                    {
                        string[] data = l.Split(new string[] { "        ", "     ", " " }, StringSplitOptions.None);
                        foreach (string d in data)
                        {
                            if (d != "")
                            {
                                Console.WriteLine(d);
                            }
                        }
                        //Console.WriteLine(l);
                    }
                    //Console.WriteLine(line);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
