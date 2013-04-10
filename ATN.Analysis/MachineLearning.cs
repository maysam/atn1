using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using ATN.Data;

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

            string weka_command = "java -cp weka.jar weka.classifiers.trees.J48 -l \"" +
                decision_tree_path + "\" -T \"" + test_data_path + "\" -p 1-4 > " +
                "\"" + classification_output_path + "\"\n";

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
            StartInfo.Arguments = string.Format("-cp weka.jar weka.classifiers.trees.J48 -l \"{0}\" -T \"{1}\" -p 2-7", decision_tree_path, test_data_path);

            //try
            //{
                using (Process batProcess = Process.Start(StartInfo))
                {
                    batProcess.WaitForExit(2000);
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
            AnalysisInterface AnalysisInterface = new AnalysisInterface();
            try
            {
                using (StreamReader sr = new StreamReader(classification_output_path))
                {
                    String co = sr.ReadToEnd();
                    string[] lines = co.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(3).ToArray();

                    foreach (string l in lines)
                    {
                        string[] data = l.Split(new string[] { "        ", "     ", " ", "\n" }, StringSplitOptions.None);

                        for (int i = 0; i < data.Length; i += 4)
                        {
                            //long source_id = Convert.ToInt64(data[i]);

                            bool prediction;
                            if (data[i + 1].Equals("1:contribu", StringComparison.Ordinal))
                            {
                                prediction = true;
                            }
                            else
                            {
                                prediction = false;
                            }

                            double probability = Convert.ToDouble(data[i + 2]);

                            string class_values = data[i + 3];

                            class_values.Trim('(', ')');
                            string[] values = class_values.Split(new string[] { "," }, StringSplitOptions.None);

                            int TheoryId = Convert.ToInt32(values[0]);
                            long SourceId = Convert.ToInt64(values[1]);

                            //AnalysisInterface.UpdateIsContributingPrediction(
                        }
                        /*
                        foreach (string d in data)
                        {
                            if (d != "")
                            {
                                Console.WriteLine(d);
                            }
                        }
                        */
                        Console.ReadLine();
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
