using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATN.Data;

namespace ATN.Analysis
{
    /// <summary>
    /// Class for calculating the theory attribution ratio scores for a network. Use the ComputeTAR method.
    /// </summary>
    public static class TAR
    {
        /// <summary>
        /// Computes the TAR score for each source in the graph using the entire graph representation of a theory
        /// </summary>
        /// <param name="SourceTreeWithAEF">The entire representation of the theory being analyzed, each with an AEF score</param>
        /// <returns>A dictionary corresponding each Source ID to a TAR score; with the TAR score being null for any papers without citations</returns>
        public static Dictionary<long, double?> ComputeTAR(Dictionary<long, SourceWithReferences> SourceTreeWithAEF)
        {
            Dictionary<long, double?> TARScores = new Dictionary<long, double?>(SourceTreeWithAEF.Count);
            foreach (SourceWithReferences Source in SourceTreeWithAEF.Values)
            {
                //If this paper doesn't cite any references then we can't compute the TAR score
                if (Source.References.Count > 0)
                {
                    double TAR = 0.0f;
                    foreach (long Reference in Source.References)
                    {
                        //TAR scores are only relevant to first leel papers
                        //Technically this should throw an error if a paper is missing an AEF score
                        if (SourceTreeWithAEF[Reference].Depth == 1 &&
                            SourceTreeWithAEF[Reference].ArticleLevelEigenFactor.HasValue)
                        {
                            TAR += SourceTreeWithAEF[Reference].ArticleLevelEigenFactor.Value;
                        }
                    }
                    TARScores[Source.SourceId] = TAR / Source.References.Count;
                }
                else
                {
                    TARScores[Source.SourceId] = null;
                }
            }
            return TARScores;
        }
    }
}
