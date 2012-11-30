using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AdjacencyMatrixBuild
{
    class Program
    {
        public static float GetCurrentMemoryUsage()
        {
            Process p = Process.GetCurrentProcess();
            return (float)p.PrivateMemorySize64 / 1000000.0f;
        }
        static void Main(string[] args)
        {

            Console.WriteLine("Background memory usage: {0:F} MB", GetCurrentMemoryUsage());
            //Setup datamodel implementation
            IN3LP_TestEntities ite = new IN3LP_TestEntities();

            //Retreive data, setup data storage mechanism
            var Data = ite.Sources.OrderBy(s => s.SourceId).Select(s => new {Source = s, Citations = s.Sources}).ToArray();
            int Dimension = Data.Count();
            
            Console.WriteLine("Retrieved {0} sources", Dimension);
            Console.WriteLine("Retrieved sources: {0:F} MB", GetCurrentMemoryUsage());
            bool[,] AdjacencyMatrix = new bool[Dimension, Dimension];

            //Correlate each index in the matrix with a particular source id
            Dictionary<int, int> SourceIdToIndex = new Dictionary<int, int>(Dimension);
            for (int i = 0; i < Dimension; i++)
            {
                SourceIdToIndex[Data[i].Source.SourceId] = i;
            }

            Console.WriteLine("Created SourceId to Index Dictionary: {0:F} MB", GetCurrentMemoryUsage());
            //Populate AdjacencyMatrix
            for (int i = 0; i < Dimension; i++)
            {
                var Citations = Data[i].Citations.ToArray();
                //Console.WriteLine("Adding entries for source {0}", Data[i].Source.SourceId);
                foreach (var Citation in Citations)
                {
                    //Console.WriteLine("AdjacencyMatrix[{0},{1}] = 1", i, SourceIdToIndex[Citation.SourceId]);
                    AdjacencyMatrix[i, SourceIdToIndex[Citation.SourceId]] = true;
                }
            }
            GC.Collect();
            Console.WriteLine("Created adjacency matrix: {0:F} MB", GetCurrentMemoryUsage());
            int CitationCount = 0;
            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    if (AdjacencyMatrix[i, j])
                    {
                        //Console.WriteLine("{0} cites {1}", i, j);
                        CitationCount++;
                        if (i == j)
                        {
                            //Console.WriteLine("{0} cites itself", i);
                        }
                    }
                }
            }
            GC.Collect();
            Console.WriteLine("Accessed adjacency matrix: {0:F} MB", GetCurrentMemoryUsage());
            Console.WriteLine("Citation count: {0}", CitationCount);
            Console.Read();
        }
    }
}
