using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;

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
            SourceAndCitations[] Data = ite.Sources.OrderBy(s => s.SourceId).Select(s => new SourceAndCitations{Source = s, Citations = s.Sources}).ToArray();
            int Dimension = Data.Count();
            
            Console.WriteLine("Retrieved {0} sources", Dimension);
            Console.WriteLine("Retrieved sources: {0:F} MB", GetCurrentMemoryUsage());
            bool[,] AdjacencyMatrix = new bool[Dimension, Dimension];

            
            //Correlate each index in the matrix with a particular source id
            Dictionary<int, int> SourceIdToIndex = new Dictionary<int, int>(Dimension);
            Dictionary<int, int> IndexToSourceId = new Dictionary<int, int>(Dimension);
            for (int i = 0; i < Dimension; i++)
            {
                IndexToSourceId[i] = Data[i].Source.SourceId;
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
            for (int i = 89; i < Dimension; i++)
            {
                ExportMatrix(Data, IndexToSourceId, AdjacencyMatrix, Dimension, 0, i);
                Console.ReadLine();
            }
            GC.Collect();
            Console.WriteLine("Accessed adjacency matrix: {0:F} MB", GetCurrentMemoryUsage());
            Console.WriteLine("Citation count: {0}", CitationCount);
            Console.Read();
        }

        public static void ExportMatrix(SourceAndCitations[] Sources, Dictionary<int, int> IndexToSourceId, bool[,] AdjacencyMatrix, int Dimension, int Level = 0, int FocalIndex = 0)
        {
            if (Level > 10)
            {
                //return;
            }
            if (Level > 0)
            {
                for (int i = 1; i < Level; i++)
                {
                    Console.Write("|");
                }
                Console.Write("|-");
            }
            Console.WriteLine("{0}", IndexToSourceId[FocalIndex]);
            for (int i = 0; i < Dimension; i++)
            {
                //AdjacencyMatrix[FocalIndex, i], if we want the graph of all the papers this one cites, and the ones those cite, etc.
                if (AdjacencyMatrix[i, FocalIndex])
                {
                    ExportMatrix(Sources, IndexToSourceId, AdjacencyMatrix, Dimension, Level + 1, i);
                }
            }
            if (Level == 0)
            {
                Console.WriteLine("Source ID: {0}, Index: {1}", IndexToSourceId[FocalIndex], FocalIndex);
            }
        }
    }
}
