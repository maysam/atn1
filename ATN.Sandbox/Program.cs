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
        static Dictionary<long, long> SourceIdToIndex = new Dictionary<long, long>();
        static Dictionary<long, long> IndexToSourceId = new Dictionary<long, long>();
        static long CurrentSourceIndex = 0;

        static Dictionary<long, List<long>> SourceIdCitedBy = new Dictionary<long, List<long>>();
        static List<SourceEdge> Edges = new List<SourceEdge>();

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

        static void AddSourceToGraph(Source Source, long[] Citations)
        {
            SourceIdCitedBy[Source.SourceId] = Citations.ToList();
        }
        static void Main(string[] args)
        {
            Theories t = new Theories();
            SourceIdWithDepth[] a = t.GetAllSourcesForTheory(7);
            Source[] CanonicalSources = t.GetCanonicalSourcesForTheory(7);

            Sources Sources = new Sources();
            
            //This works by building a graph of all possible nodes in the graph such
            //that ImpactFactor scores can be properly computed. Once all nodes have
            //been added to the graph, all citations that are not in the graph are
            //removed before edge objects are created

            //Build raw list of all possible edges in the graph
            foreach(Source CanonicalSource in CanonicalSources)
            {
                var CitingSources = CanonicalSource.CitingSources;

                AddSourceToGraph(CanonicalSource, CitingSources.Select(src => src.SourceId).ToArray());
                
                //Write citation nodes/edges
                foreach(Source CitingSource in CitingSources)
                {
                    AddSourceToGraph(CitingSource, CitingSource.CitingSources.Select(src => src.SourceId).ToArray());

                    //Write reference nodes/edges
                    foreach (Source ReferenceSource in CitingSource.References)
                    {
                        AddSourceToGraph(ReferenceSource, ReferenceSource.CitingSources.Select(src => src.SourceId).ToArray());
                    }
                }
            }

            //Prune raw list to remove citations which are not present
            long[] AllKeys = SourceIdCitedBy.Keys.ToArray();
            foreach (long SourceId in AllKeys)
            {
                List<long> CitedBySourceIds = new List<long>(SourceIdCitedBy[SourceId].Count);
                foreach (long CitedBySourceId in SourceIdCitedBy[SourceId])
                {
                    if (SourceIdCitedBy.ContainsKey(CitedBySourceId) && CitedBySourceId != SourceId)
                    {
                        CitedBySourceIds.Add(CitedBySourceId);
                    }
                }
                SourceIdCitedBy[SourceId] = CitedBySourceIds;
            }
            foreach (KeyValuePair<long, List<long>> SourceAndCitations in SourceIdCitedBy)
            {
                foreach(long Citation in SourceAndCitations.Value)
                {
                    long EndIndex = GetIndexForSource(SourceAndCitations.Key);
                    long StartIndex = GetIndexForSource(Citation);
                    Edges.Add(new SourceEdge(StartIndex, EndIndex));
                }
            }
            Dictionary<long, int> TimesKeyCitesSomething = new Dictionary<long, int>();
            foreach (long Key in SourceIdCitedBy.Keys)
            {
                int CitationCount = 0;
                foreach (List<long> Value in SourceIdCitedBy.Values)
                {
                    CitationCount += Value.Count(val => val == Key);
                }
                TimesKeyCitesSomething[Key] = CitationCount;
            }
            Edges = Edges.OrderBy(e => e.StartSourceId).ThenBy(e => e.EndSourceId).ToList();

            int MatrixOrder = SourceIdToIndex.Keys.Count();
            int NumberOfEdges = Edges.Count();

            // Use IF (of citation network) scores to determine # of row entries
            int[] OFArray = SourceIdCitedBy.OrderBy(kv => SourceIdToIndex[kv.Key]).Select(kv => TimesKeyCitesSomething[kv.Key]).ToArray();

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

           //FileStream DestinationNetStream = File.Open("Graph.net", FileMode.Create);
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