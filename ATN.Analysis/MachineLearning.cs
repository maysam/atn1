using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using ATN.Export;
using ATN.Data;

namespace ATN.Analysis
{
    public class MachineLearning
    {
        private static readonly string CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().GetName().CodeBase).Replace(@"file:\", "");

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
            StartInfo.RedirectStandardOutput = true;
            StartInfo.RedirectStandardError = true;
            StartInfo.WorkingDirectory = CurrentDirectory;
            StartInfo.FileName = "java.exe";
            StartInfo.Arguments = string.Format("-cp weka.jar weka.classifiers.trees.J48 -C 0.25 -M 1 -x 4 -t \"{0}\" -d \"{1}\"", training_data_path, decision_tree_path);

            using (Process batProcess = Process.Start(StartInfo))
            {
                string stdout = batProcess.StandardOutput.ReadToEnd().Replace("\n", "\r\n");
                string stderr = batProcess.StandardError.ReadToEnd().Replace("\n", "\r\n");
                Trace.Write(stdout);
                Trace.Write(stderr);
                batProcess.WaitForExit(5000);
                batProcess.Close();
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
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.UseShellExecute = false;
            StartInfo.CreateNoWindow = false;
            StartInfo.RedirectStandardOutput = true;
            StartInfo.RedirectStandardError = true;
            StartInfo.WorkingDirectory = CurrentDirectory;
            StartInfo.FileName = "java.exe";
            StartInfo.Arguments = string.Format("-cp weka.jar weka.classifiers.trees.J48 -l \"{0}\" -T \"{1}\" -p 1-6", decision_tree_path, test_data_path);

            using (Process batProcess = Process.Start(StartInfo))
            {
                batProcess.WaitForExit(5000);
                string stdout = batProcess.StandardOutput.ReadToEnd();
                string stderr = batProcess.StandardError.ReadToEnd();
                batProcess.Close();
                Stream s = File.OpenWrite(classification_output_path);
                StreamWriter sw = new StreamWriter(s);
                sw.Write(stdout);
                sw.Close();
            }
        }

        public static Dictionary<long, Prediction> ParseClassificationOutput(string classification_output_path, int TheoryId, ExtendedSource[] ClassifySources)
        {
            Dictionary<long, Prediction> Classifications = new Dictionary<long, Prediction>(ClassifySources.Length);

            using (StreamReader sr = new StreamReader(classification_output_path))
            {
                String co = sr.ReadToEnd();
                string[] lines = co.Split(new string[] { "\n", "\r" }, StringSplitOptions.None).Skip(5).ToArray();

                foreach (string l in lines)
                {
                    try
                    {
                        if (l != string.Empty)
                        {
                            string[] data = l.Split(new string[] { "        ", "       " }, StringSplitOptions.None);

                            long instance_id = Convert.ToInt64(data[0]) - 1;

                            bool prediction;
                            if (data[1].Equals("1:? 1:contribu", StringComparison.Ordinal))
                            {
                                prediction = true;
                            }
                            else
                            {
                                prediction = false;
                            }

                            double probability = Convert.ToDouble(data[2].Split(new string[] { " " }, StringSplitOptions.None)[0]);

                            //Console.WriteLine("Probability: {0}, Prediciton: {1}", probability.ToString(), prediction.ToString());

                            Prediction p = new Prediction(prediction, probability);
                            Classifications[ClassifySources[instance_id].SourceId] = p;
                        }
                    }
                    catch { }
                }
            }
            return Classifications;
        }

        public static Dictionary<long, Prediction> RunML(ExtendedSource[] TrainSources, ExtendedSource[] ClassifySources, int TheoryId)
        {

            string DecisionTree = CurrentDirectory + "\\" + TheoryId + "-model.dat";
            string TrainArff = CurrentDirectory + "\\" + TheoryId + "-train.arff";
            string ClassifyArff = CurrentDirectory + "\\" + TheoryId + "-classify.arff";
            string ClassifyOutput = CurrentDirectory + "\\" + TheoryId + "-classified.txt";

            Theories t = new Theories();
            FileStream TrainStream = File.Open(TrainArff, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            //var TrainSources = t.GetAllExtendedSourcesForTheory(TheoryId, 0, Int32.MaxValue).Where(s => s.Contributing.HasValue && s.Depth < 3).ToArray();
            ARFFExporter.Export(TrainSources, TheoryId, TrainStream);

            FileStream ClassifyStream = File.Open(ClassifyArff, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            //var ClassifySources = t.GetAllExtendedSourcesForTheory(TheoryId, 0, Int32.MaxValue).Where(s => !s.Contributing.HasValue).ToArray();
            ARFFExporter.Export(ClassifySources, TheoryId, ClassifyStream);

            MachineLearning.GenerateDecisionTree(TrainArff, DecisionTree);
            MachineLearning.ClassifyData(DecisionTree, ClassifyArff, ClassifyOutput);
            Dictionary<long, Prediction> Classifications = MachineLearning.ParseClassificationOutput(ClassifyOutput, TheoryId, ClassifySources);

            File.Delete(DecisionTree);
            File.Delete(TrainArff);
            File.Delete(ClassifyArff);
            File.Delete(ClassifyOutput);

            return Classifications;
        }
    }
}
