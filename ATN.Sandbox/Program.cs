using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATN.Crawler;
using ATN.Crawler.MAS;
using ATN.Data;
using alglib = ATN.Analysis.AEF.alglib;
using ATN.Export;
using System.IO;

namespace ATN.Analysis
{
    class Program
    {
        //For storing the translation between Source ID and Index
        //in order to accommodate alglib's storage constraints
        static Dictionary<long, long> SourceIdToIndex = new Dictionary<long, long>();
        static Dictionary<long, long> IndexToSourceId = new Dictionary<long, long>();
        static long CurrentSourceIndex = 0;

        //For storage of the theory source tree
        static Dictionary<long, List<SourceIdWithDepth>> SourceIdCitedBy;

        //For storing the list of edges that will be passed to alglib
        static List<SourceEdge> Edges = new List<SourceEdge>();

        /// <summary>
        /// Retrieves the index for a particular source, and adds the Source and index
        /// to the translation dictionaries if it has not been seen before
        /// </summary>
        /// <param name="SourceId">The SourceId to retrieve an index for</param>
        /// <returns>The index corresponding to the passed SourceId</returns>
        static long GetIndexForSource(long SourceId)
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

        static void Main(string[] args)
        {
            Theories t = new Theories();
            SourceIdCitedBy = t.GetSourceTreeForTheory(7);

            //Translate the theory tree into a list of edges
            foreach (KeyValuePair<long, List<SourceIdWithDepth>> SourceAndCitations in SourceIdCitedBy)
            {
                foreach (SourceIdWithDepth Citation in SourceAndCitations.Value)
                {
                    //This check is to ignore the canonical IDs
                    if (SourceAndCitations.Key != -1)
                    {
                        long EndIndex = GetIndexForSource(SourceAndCitations.Key);
                        long StartIndex = GetIndexForSource(Citation.SourceId);
                        Edges.Add(new SourceEdge(StartIndex, EndIndex));
                    }
                }
            }

            //Compute how many times each source cites other sources
            //This is neccessary for alglib's sparse matrix storage
            Dictionary<long, int> TimesKeyCitesSomething = new Dictionary<long, int>();
            foreach (long Key in SourceIdToIndex.Keys)
            {
                int CitationCount = 0;
                //The Skip(1) is to skip over the SourceIdCitedBy[-1] entry;
                //as these are canonical IDs which should not be counted
                foreach (List<SourceIdWithDepth> Value in SourceIdCitedBy.Values.Skip(1))
                {
                    CitationCount += Value.Count(val => val.SourceId == Key);
                }
                TimesKeyCitesSomething[Key] = CitationCount;
            }
            Edges = Edges.OrderBy(e => e.StartSourceId).ThenBy(e => e.EndSourceId).ToList();

            int MatrixOrder = SourceIdToIndex.Keys.Count();
            int NumberOfEdges = Edges.Count();

            //Compute the out factor for each source in the tree
            int[] OFArray = SourceIdToIndex.Keys.Where(k => k != -1).OrderBy(k => SourceIdToIndex[k]).Select(k => TimesKeyCitesSomething[k]).ToArray();

            //Compute the ImpactFactor for each source in the tree
            //int[] IFArray = SourceIdToIndex.Keys.Where(k => k != -1).OrderBy(k => SourceIdToIndex[k]).Select(k => !SourceIdCitedBy.ContainsKey(k) ? 0 : SourceIdCitedBy[k].Count).ToArray();

            alglib.sparsematrix s;
            alglib.sparsecreatecrs(MatrixOrder, MatrixOrder, OFArray, out s);
            
            // Manual placement
            //alglib.sparseset(s, 0, 1, 1.0);
            //alglib.sparseset(s, 0, 3, 1.0);
            //alglib.sparseset(s, 1, 2, 1.0);
            //alglib.sparseset(s, 3, 0, 1.0);
            //alglib.sparseset(s, 3, 1, 1.0);
            //alglib.sparseset(s, 3, 2, 1.0);

            /* Write E to sparse matrix. Must go left to right (will have to order in pull from db) */
            for (int i = 0; i < NumberOfEdges; i++)
            {
                alglib.sparseset(s, (int)(Edges[i].StartSourceId), (int)(Edges[i].EndSourceId), 1.0);
            }


            /* Print Sparse Matrix */
            // For the test, we should have adj matrix of the form
            //   [0 1 0 1]
            //   [0 0 1 0]
            //   [0 0 0 0]
            //   [1 1 1 0]
            double v;
            //Console.Write("-- Sparse matrix read results --\n");
            //for (int i = 0; i < MatrixOrder; i++)
            //{
            //    Console.Write("[ ");
            //    for (int j = 0; j < MatrixOrder; j++)
            //    {
            //        v = alglib.sparseget(s, i, j);
            //        Console.Write("{0:F2} ", v);
            //   }
            //    Console.Write("]\n");
            //}
            //Console.Write("\n");

           /* Sum both the in and out degree of the matrix, placing this in vector p */
           double[] p = new double[MatrixOrder];
           double[] RowSum = new double[MatrixOrder];
           //double RowContrib;
           //double ColContrib;

           int T0 = 0;
           int T1 = 0;
           int I, J;
           double V;
           while (alglib.sparseenumerate(s, ref T0, ref T1, out I, out J, out V))
           {
               p[I] += V;
               p[J] += V;
               RowSum[I] += V;
           }

           double pSum = p.Sum();
           //for (int i = 0; i < MatrixOrder; i++)
           //{
           //    for (int j = 0; j < MatrixOrder; j++)
           //    {
           //        // Note: rewrite w/ sparse enumerate?
           //        RowContrib = alglib.sparseget(s, i, j);
           //        ColContrib = alglib.sparseget(s, j, i);
           //        RowSum[i] += RowContrib;
           //        p[i] += (RowContrib + ColContrib);
           //    }
           //    pSum += p[i];
           //}
           
           /* print out p */
           //Console.Write("-- Out/In degree vector -- \n p = [ ");
           //for (int i = 0; i < MatrixOrder; i++)
           //    Console.Write("{0:F2} ", p[i]);
           //Console.Write("]^T \n");

           /* Normalize p */
           for (int i = 0; i < MatrixOrder; i++)
               p[i] /= pSum;


           /* PRINTS */
           //Console.Write("-- Normalized p -- \n p_norm = [ ");
           //for (int i = 0; i < MatrixOrder; i++)
           //    Console.Write("{0:F4} ", p[i]);
           //Console.Write("]^T \n");
           //Console.Write("\n");

           //Console.Write("-- Row Sum-- \n RowSum = [ ");
           //for (int i = 0; i < MatrixOrder; i++)
           //    Console.Write("{0:F4} ", RowSum[i]);
           //Console.Write("]^T \n");

           /* Indices for enumeration */
           //int T0 = 0;
           //int T1 = 0;
           //int I, J;
           //double V;
           while (alglib.sparseenumerate(s, ref T0, ref T1, out I, out J, out V))
               alglib.sparserewriteexisting(s, I, J, V/RowSum[I]);

            /* Print Stochastic matrix */
           //Console.Write("-- Stochastic matrix Z --\n");
           for (int i = 0; i < MatrixOrder; i++)
           {
               //Console.Write("[ ");
               for (int j = 0; j < MatrixOrder; j++)
               {
                   v = alglib.sparseget(s, i, j);
                 //  Console.Write("{0:F2} ", v);
               }
               //Console.Write("]\n");
           }
           //Console.Write("\n");

           //Console.Write("-- AEF (pre-normalization) p^T*Z --\n");
           double[] AEFScore = new double[0];
           alglib.sparsemtv(s, p, ref AEFScore);
           //Console.WriteLine("{0}", alglib.ap.format(AEFScore, 4));
           Console.Write("\n");

           /* Normalize AEF scores */
           double AEFsum = AEFScore.Sum();
           /* Normalize p */
           for (int i = 0; i < MatrixOrder; i++)
               AEFScore[i] /= AEFsum;

           Console.Write("-- AEF (post-normalization) --\n");
           Console.WriteLine("{0}", alglib.ap.format(AEFScore, 4));
                      
            /* Free s */
           alglib.sparsefree(out s);

           //FileStream DestinationNetStream = File.Open("TheoryId2Export.txt", FileMode.Create);
           //StreamWriter sw = new StreamWriter(DestinationNetStream);
           //sw.WriteLine(" V   \tAEF              IF \n");
           //foreach (KeyValuePair<long, long> KV in IndexToSourceId)
           //{
           //    sw.Write(string.Format("\"{0}\"\t{1:F10} \t          {2}\n", KV.Value, AEFScore[KV.Key], IFArray[KV.Key]));
           //}
           //sw.Close();

           Console.ReadKey();

        
        } // end main


        public static void multEX()
        {
            /* MATRIX MULT EXAMPLE */
            double[,] a = new double[,] {
            {1,2,3},
            {4,5,6}
            };
            double[,] b = new double[,] {
            {7,8,9,10},
            {11,12,13,14},
            {15,16,17,18}
            };

            int m = a.GetLength(0);
            int n = b.GetLength(1);
            int k = a.GetLength(1);
            double[,] c = new double[m, n];
            alglib.rmatrixgemm(m, n, k, 1, a, 0, 0, 0, b, 0, 0, 0, 0, ref c, 0, 0);
            //c = {{74, 80, 86, 92}, {173, 188, 203, 218}}

        } // end mult
    }
}