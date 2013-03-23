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

namespace ATN.Analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Theories t = new Theories();
            Source[] CanonicalSources = t.GetCanonicalSourcesForTheory(7);

            Dictionary<long, SourceNode> Nodes = new Dictionary<long, SourceNode>();
            List<SourceEdge> Edges = new List<SourceEdge>();

            Dictionary<long, int> IndexToIF = new Dictionary<long, int>();
            Dictionary<long, long> SourceIdToIndex = new Dictionary<long, long>();
            Dictionary<long, long> IndexToSourceId = new Dictionary<long, long>();

            long CurrentSourceIndex = 0;
            Sources Sources = new Sources();
            List<Source> FirstLevelSources = new List<Source>();
            List<Source> ReferenceSources = new List<Source>();
            foreach(Source CanonicalSource in CanonicalSources)
            {
                IndexToIF.Add(CurrentSourceIndex, Sources.GetIFScoresBySourceId(CanonicalSource.SourceId, false));
                SourceIdToIndex.Add(CanonicalSource.SourceId, CurrentSourceIndex);
                IndexToSourceId.Add(CurrentSourceIndex, CanonicalSource.SourceId);
                CurrentSourceIndex++;

                var CitingSources = CanonicalSource.CitingSources.ToArray();

                //Write citation edges
                foreach(Source CitingSource in CitingSources)
                {
                    FirstLevelSources.Add(CitingSource);
                    if (CitingSource.SourceId != CanonicalSource.SourceId)
                    {
                        if (!SourceIdToIndex.ContainsKey(CitingSource.SourceId))
                        {
                            SourceIdToIndex.Add(CitingSource.SourceId, CurrentSourceIndex);
                            IndexToSourceId.Add(CurrentSourceIndex, CitingSource.SourceId);
                            IndexToIF.Add(CurrentSourceIndex, 0);

                            Edges.Add(new SourceEdge(CurrentSourceIndex, SourceIdToIndex[CanonicalSource.SourceId]));
                            CurrentSourceIndex++;
                        } else {
                            Edges.Add(new SourceEdge(SourceIdToIndex[CitingSource.SourceId], SourceIdToIndex[CanonicalSource.SourceId]));
                        }
                    }
                }

                //Write reference edges
                foreach (Source CitingSource in CitingSources)
                {
                    foreach (Source Reference in CitingSource.References)
                    {
                        if (CitingSource.SourceId != Reference.SourceId && Reference.SourceId != CanonicalSource.SourceId)
                        {
                            ReferenceSources.Add(Reference);
                            if (!SourceIdToIndex.ContainsKey(Reference.SourceId))
                            {
                                IndexToIF.Add(CurrentSourceIndex, Sources.GetIFScoresBySourceId(Reference.SourceId, true));
                                SourceIdToIndex.Add(Reference.SourceId, CurrentSourceIndex);
                                IndexToSourceId.Add(CurrentSourceIndex, Reference.SourceId);

                                Edges.Add(new SourceEdge(SourceIdToIndex[CitingSource.SourceId], CurrentSourceIndex, 1));
                                CurrentSourceIndex++;
                            }
                            else
                            {
                                Edges.Add(new SourceEdge(SourceIdToIndex[CitingSource.SourceId], SourceIdToIndex[Reference.SourceId], 1));
                            }
                        }
                    }
                }
                //Get all sources citing each 1st level sources that are in the graph, and up each 1st level source's IF
            }

            long[] SourceIdsInGraph = SourceIdToIndex.Keys.ToArray();
            foreach (Source FirstLevelSource in FirstLevelSources)
            {
                Source[] Citations = FirstLevelSource.CitingSources.ToArray().Where(src => SourceIdsInGraph.Contains(src.SourceId)).ToArray();
                foreach (Source Citation in Citations)
                {
                    if (!FirstLevelSources.Select(src => src.SourceId).Contains(Citation.SourceId))
                    {
                        Edges.Add(new SourceEdge(SourceIdToIndex[Citation.SourceId], SourceIdToIndex[FirstLevelSource.SourceId]));
                    }
                }
                IndexToIF[SourceIdToIndex[FirstLevelSource.SourceId]] = Citations.Count();
            }

            foreach (Source ReferenceSource in ReferenceSources)
            {
                Source[] Citations = ReferenceSource.CitingSources.ToArray().Where(src => SourceIdsInGraph.Contains(src.SourceId)).ToArray();
                IndexToIF[SourceIdToIndex[ReferenceSource.SourceId]] = Citations.Count();
            }

            int MatrixOrder = SourceIdToIndex.Keys.Count();
            int NumberOfEdges = Edges.Count();

            Edges = Edges.OrderBy(e => e.EndSourceId).ThenBy(e => e.StartSourceId).ToList();

            // Use IF (of citation network) scores to determine # of row entries
            int[] IFArray = IndexToIF.Values.ToArray();

            alglib.sparsematrix s;
            alglib.sparsecreatecrs(MatrixOrder, MatrixOrder, IFArray, out s);
            
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
                /*try
                {*/
                    alglib.sparseset(s, (int)(Edges[i].EndSourceId), (int)(Edges[i].StartSourceId), 1.0);
                /*}
                catch (Exception e)
                {
                    throw e;
                }*/
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
           double RowContrib;
           double ColContrib;
           double pSum = 0;

           for (int i = 0; i < MatrixOrder; i++)
           {
               for (int j = 0; j < MatrixOrder; j++)
               {
                   // Note: rewrite w/ sparse enumerate?
                   RowContrib = alglib.sparseget(s, i, j);
                   ColContrib = alglib.sparseget(s, j, i);
                   RowSum[i] += RowContrib;
                   p[i] += (RowContrib + ColContrib);
               }
               pSum += p[i];
           }
           
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
           int T0 = 0;
           int T1 = 0;
           int I, J;
           double V;
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