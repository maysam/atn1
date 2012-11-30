using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdjacencyMatrixBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            //Setup datamodel implementation
            IN3LP_TestEntities ite = new IN3LP_TestEntities();

            //Retreive data, setup data storage mechanism
            var Data = ite.Sources.OrderBy(s => s.SourceId).Select(s => new {Source = s, Citations = s.Sources}).ToArray();
            int Dimension = Data.Count();

            Console.WriteLine("Retrieved {0} sources", Dimension);
            bool[,] AdjacencyMatrix = new bool[Dimension, Dimension];

            //Correlate each index in the matrix with a particular source id
            Dictionary<int, int> SourceIdToIndex = new Dictionary<int, int>(Dimension);
            for (int i = 0; i < Dimension; i++)
            {
                SourceIdToIndex[Data[i].Source.SourceId] = i;
            }

            //Populate AdjacencyMatrix
            for (int i = 0; i < Dimension; i++)
            {
                var Citations = Data[i].Citations.ToArray();
                Console.WriteLine("Adding entries for source {0}", Data[i].Source.SourceId);
                foreach (var Citation in Citations)
                {
                    Console.WriteLine("AdjacencyMatrix[{0},{1}] = 1", i, SourceIdToIndex[Citation.SourceId]);
                    AdjacencyMatrix[i, SourceIdToIndex[Citation.SourceId]] = true;
                }
            }

            int CitationCount = 0;
            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    if (AdjacencyMatrix[i, j])
                    {
                        Console.WriteLine("{0} cites {1}", i, j);
                        CitationCount++;
                        if (i == j)
                        {
                            Console.WriteLine("{0} cites itself", i);
                        }
                    }
                }
            }
            Console.WriteLine("Citation count: {0}", CitationCount);
        }
    }
}
