//#define VERBOSE
#define TIMING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using ATN.Data;
using alglib = ATN.Analysis.Support.alglib;
using System.IO;
using ATN.Export;

namespace ATN.Analysis
{
    public static class AEF
    {
        /// <summary>
        /// Retrieves the index for a particular source, and adds the Source and index
        /// to the translation dictionaries if it has not been seen before
        /// </summary>
        /// <param name="SourceId">The SourceId to retrieve an index for</param>
        /// <returns>The index corresponding to the passed SourceId</returns>
        static long GetIndexForSource(Dictionary<long, long> SourceIdToIndex, Dictionary<long, long> IndexToSourceId, ref long CurrentSourceIndex, long SourceId)
        {
            if (!SourceIdToIndex.ContainsKey(SourceId))
            {
                SourceIdToIndex[SourceId] = CurrentSourceIndex;
                IndexToSourceId[CurrentSourceIndex] = SourceId;
                CurrentSourceIndex++;
                return CurrentSourceIndex - 1;
            }
            else
            {
                return SourceIdToIndex[SourceId];
            }
        }

        public static Dictionary<long, double> ComputeAEF(Dictionary<long, SourceWithReferences> SourceTree)
        {
            //For storing the translation between Source ID and Index
            //in order to accommodate alglib's storage constraints
            Dictionary<long, long> SourceIdToIndex = new Dictionary<long, long>();
            Dictionary<long, long> IndexToSourceId = new Dictionary<long, long>();
            long CurrentSourceIndex = 0;

            //For storing the list of edges that will be passed to alglib
            List<SourceEdge> Edges = new List<SourceEdge>();

            Theories t;
            #if TIMING
                Trace.WriteLine("TIMING ON \n hour:min:sec:ms format");
                Stopwatch stopWatch = new Stopwatch();
                Stopwatch FullstopWatch = new Stopwatch();
                FullstopWatch.Start();
                stopWatch.Start();
                t = new Theories();
            #else
                t = new Theories();
            #endif

            //Translate the theory tree into a list of edges
            foreach (KeyValuePair<long, SourceWithReferences> SourceAndCitations in SourceTree)
            {
                if (SourceAndCitations.Value.References.Count == 0)
                {
                    GetIndexForSource(SourceIdToIndex, IndexToSourceId, ref CurrentSourceIndex, SourceAndCitations.Key);
                }
                else
                {
                    foreach (long Citation in SourceAndCitations.Value.References)
                    {
                        long EndIndex = GetIndexForSource(SourceIdToIndex, IndexToSourceId, ref CurrentSourceIndex, Citation);
                        long StartIndex = GetIndexForSource(SourceIdToIndex, IndexToSourceId, ref CurrentSourceIndex, SourceAndCitations.Key);
                        Edges.Add(new SourceEdge(StartIndex, EndIndex));
                    }
                }
            }

            //Compute how many times each source cites other sources
            //This is neccessary for alglib's sparse matrix storage
            Dictionary<long, int> TimesKeyCitesSomething = SourceTree.ToDictionary(kv => kv.Key, kv => kv.Value.OutFactor);
            Edges = Edges.OrderBy(e => e.StartSourceId).ThenBy(e => e.EndSourceId).ToList();

            #if TIMING
                stopWatch.Stop();
                TimeSpan ReadFromDB = stopWatch.Elapsed;
                // Format and display timing
                string ParsedReadFromDB = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ReadFromDB.Hours, ReadFromDB.Minutes, ReadFromDB.Seconds,
                    ReadFromDB.Milliseconds / 10);
                Trace.WriteLine(ParsedReadFromDB + " (read from DB)");
            #endif

            /* ----- COMPUTATION OF AEF ----- */
            //Vars
            int T0 = 0;
            int T1 = 0;
            int I, J;
            double V;
            #if VERBOSE
                double v;
            #endif
            int MatrixOrder = SourceTree.Keys.Count();
            int NumberOfEdges = Edges.Count();
            double[] p = new double[MatrixOrder];
            double[] RowSum = new double[MatrixOrder];

            /*  ----- READING IN FOR COMPUTATION ----- */
            // Compute the out degree of each node for CRS storage. OFarray provides the number of entries in each row 

            #if TIMING
                stopWatch.Restart();
                stopWatch.Start();
            #endif

            int[] OFArray = SourceTree.Keys.OrderBy(k => SourceIdToIndex[k]).Select(k => TimesKeyCitesSomething[k]).ToArray();
            // Read into CRS formatted sparse matrix. Info must be read into the sparse matrix left to right, top to bottom
            alglib.sparsematrix s;
            alglib.sparsecreatecrs(MatrixOrder, MatrixOrder, OFArray, out s);
            for (int i = 0; i < NumberOfEdges; i++)
                alglib.sparseset(s, (int)(Edges[i].StartSourceId), (int)(Edges[i].EndSourceId), 1.0);

            #if TIMING
                stopWatch.Stop();
                TimeSpan ReadToCRS = stopWatch.Elapsed;
                // Format and display timing
                string ParsedReadToCRS = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ReadToCRS.Hours, ReadToCRS.Minutes, ReadToCRS.Seconds,
                    ReadToCRS.Milliseconds / 10);
                Trace.WriteLine(ParsedReadToCRS + " (read to CRS)");
            #endif

            // out
            #if VERBOSE
                Console.Write("-- Citation network in CRS format --\n");
                for (int i = 0; i < MatrixOrder; i++)
                {
                    Console.Write("[ ");
                    for (int j = 0; j < MatrixOrder; j++)
                    {
                        v = alglib.sparseget(s, i, j);
                        Console.Write("{0:F2} ", v);
                    }
                    Console.Write("]\n");
                }
                Console.Write("\n");
            #endif
            /*  ----- END OF READING IN FOR COMPUTATION ----- */


            /* ----- COMPUTE P ----- */
            // Create p vector by summing the in and our degree of each node by enumerating over non-zero elements
            // RowSum is used to make s stochastic

            #if TIMING
                stopWatch.Restart();
                stopWatch.Start();
            #endif

            while (alglib.sparseenumerate(s, ref T0, ref T1, out I, out J, out V))
            {
                p[I] += V;
                p[J] += V;
                RowSum[I] += V;
            }

            //out (pre normalized p)
            #if VERBOSE
               Console.Write("-- Out/In degree vector -- \n p = [ ");
               for (int i = 0; i < MatrixOrder; i++)
                   Console.Write("{0:F2} ", p[i]);
               Console.Write("]^T \n");
            #endif

            //p normalization
            double pSum = p.Sum();
            for (int i = 0; i < MatrixOrder; i++)
                p[i] /= pSum;

            //out p post normalization
            #if VERBOSE
               Console.Write("-- Normalized p -- \n p_norm = [ ");
               for (int i = 0; i < MatrixOrder; i++)
                   Console.Write("{0:F4} ", p[i]);
               Console.Write("]^T \n");
               Console.Write("\n");
            #endif


            /* ----- MAKE s STOCHASTIC ----- */
            while (alglib.sparseenumerate(s, ref T0, ref T1, out I, out J, out V))
                alglib.sparserewriteexisting(s, I, J, V / RowSum[I]);

            //out stochastic s
            #if VERBOSE
               Console.Write("-- Stochastic matrix Z --\n");
               for (int i = 0; i < MatrixOrder; i++)
               {
                   //Console.Write("[ ");
                   for (int j = 0; j < MatrixOrder; j++)
                   {
                       v = alglib.sparseget(s, i, j);
                       Console.Write("{0:F2} ", v);
                   }
                   Console.Write("]\n");
               }
               Console.Write("\n");
            #endif

            /* ----- SCORE COMPUTATION ----- */

            // Take one step on the graph (compute p^T*Z)
            double[] AEFScore = new double[0];
            alglib.sparsemtv(s, p, ref AEFScore);

            // Normalize for AEF approximation
            double AEFsum = AEFScore.Sum();
            for (int i = 0; i < MatrixOrder; i++)
                AEFScore[i] /= AEFsum;

            #if TIMING
              stopWatch.Stop();
              TimeSpan AEFComputation = stopWatch.Elapsed;
              string ParsedAEFComputation = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
              AEFComputation.Hours, AEFComputation.Minutes, AEFComputation.Seconds, AEFComputation.Milliseconds / 10);
              Trace.WriteLine(ParsedAEFComputation + " (AEF computation)");
              stopWatch.Restart();
            #endif

            //out AEF 
            #if VERBOSE
               Console.Write("-- AEF (post-normalization) --\n");
               Trace.WriteLine("{0}", alglib.ap.format(AEFScore, 4));
            #endif

            /* Free s */
            alglib.sparsefree(out s);

            //Set AEF values
            Dictionary<long, double> SourceIDToAEF = new Dictionary<long, double>(SourceIdToIndex.Count);
            for (int i = 0; i < AEFScore.Length; i++)
            {
                SourceIDToAEF.Add(IndexToSourceId[i], AEFScore[i]);
            }

            #if TIMING
                stopWatch.Stop();
                TimeSpan Storage = stopWatch.Elapsed;
                string ParsedStorage = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                Storage.Hours, Storage.Minutes, Storage.Seconds, Storage.Milliseconds / 10);
                Trace.WriteLine(ParsedStorage + " (AEF storage)");

                FullstopWatch.Stop();
                TimeSpan FullRunTiming = FullstopWatch.Elapsed;
                string ParsedFullRunTiming = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                FullRunTiming.Hours, FullRunTiming.Minutes, FullRunTiming.Seconds, FullRunTiming.Milliseconds / 10);
                Trace.WriteLine(ParsedFullRunTiming + " (full run)");
                stopWatch.Restart();
            #endif

            return SourceIDToAEF;
        } // end main
    }
}