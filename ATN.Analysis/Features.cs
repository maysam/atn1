using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;

namespace ATN.Analysis
{
    class Features
    {
        /// <summary>
        /// This method calculates the TheoryAttributionRatio for the specified source, relative
        /// to the specified theory. The value calculated is stored in the database.
        /// </summary>
        /// <param name="SourceId"></param>
        /// <param name="TheoryId"></param>
        public static void CalculateTheoryAttributionRatioForSource(long SourceId, int TheoryId)
        {
            //Add AEF of citing, divide by count of references
            Theories Theories = new Theories();
            AnalysisInterface AnalysisInterface = new AnalysisInterface();
            
            Source[] CitingSources = Theories.GetCitingSourcesForSource(SourceId);

            double eigen_acc = 0;
            foreach (Source s in CitingSources)
            {
                TheoryMembership tm = AnalysisInterface.GetTheoryMembershipForSource(s.SourceId, TheoryId);
                eigen_acc += tm.ArticleLevelEigenFactor.Value;
            }

            double tar = (eigen_acc / CitingSources.Length);
            AnalysisInterface.UpdateTheoryAttentionRatio(TheoryId, SourceId, tar);
        }

        public static void CalculateImpactFactorForSource(long SourceId, int TheoryId)
        {
            //Number of citing papers
            AnalysisInterface AnalysisInterface = new AnalysisInterface();
            AnalysisInterface.UpdateImpactFactor(TheoryId, SourceId);
        }
    }
}
