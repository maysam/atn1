using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATN.Analysis;
using ATN.Data;

namespace ATN.Test
{
    [TestClass]
    public class AnalysisTests
    {
        [TestMethod]
        public void VerifyTAR()
        {
            Dictionary<long, SourceWithReferences> TARTree = new Dictionary<long, SourceWithReferences>();
            SourceWithReferences CanonicalSource = new SourceWithReferences(1, 1, 0);
            SourceWithReferences FirstLevelSource = new SourceWithReferences(2, 0, 1);
            SourceWithReferences SecondLevelSource = new SourceWithReferences(3, 1, 2);
            SourceWithReferences OutsideSource = new SourceWithReferences(4, 0, 2);
            SourceWithReferences InsideSource = new SourceWithReferences(5, 0, 1);

            FirstLevelSource.References.Add(CanonicalSource.SourceId);
            FirstLevelSource.References.Add(SecondLevelSource.SourceId);
            OutsideSource.References.Add(SecondLevelSource.SourceId);
            InsideSource.References.Add(FirstLevelSource.SourceId);

            TARTree.Add(CanonicalSource.SourceId, CanonicalSource);
            TARTree.Add(FirstLevelSource.SourceId, FirstLevelSource);
            TARTree.Add(SecondLevelSource.SourceId, SecondLevelSource);
            TARTree.Add(OutsideSource.SourceId, OutsideSource);
            TARTree.Add(InsideSource.SourceId, InsideSource);

            Dictionary<long, double> AEFScores = AEF.ComputeAEF(TARTree);

            foreach (KeyValuePair<long, double> AEFScore in AEFScores)
            {
                TARTree[AEFScore.Key].ArticleLevelEigenFactor = AEFScore.Value;
            }
            Dictionary<long, double?> TARScores = TAR.ComputeTAR1(TARTree);
            foreach (KeyValuePair<long, double?> TARScore in TARScores)
            {
                if (TARScore.Key == CanonicalSource.SourceId)
                {
                    Assert.IsTrue(!TARScore.Value.HasValue);
                    //Assert.AreEqual(0.0f, TARScore.Value);
                }
                else if (TARScore.Key == FirstLevelSource.SourceId)
                {
                    Assert.IsTrue(TARScore.Value.HasValue);
                    Assert.AreEqual(0.0f, TARScore.Value);
                }
                else if (TARScore.Key == SecondLevelSource.SourceId)
                {
                    Assert.IsFalse(TARScore.Value.HasValue);
                }
                else if (TARScore.Key == OutsideSource.SourceId)
                {
                    Assert.IsTrue(TARScore.Value.HasValue);
                    Assert.AreEqual(0.0f, TARScore.Value);
                }
                else if (TARScore.Key == InsideSource.SourceId)
                {
                    Assert.IsTrue(TARScore.Value.HasValue);
                    Assert.AreEqual(FirstLevelSource.ArticleLevelEigenFactor, TARScore.Value);
                }
            }
        }
    }
}
