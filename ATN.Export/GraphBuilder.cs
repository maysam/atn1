﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATN.Data;

namespace ATN.Export
{
    public class GraphBuilder
    {
        public Theories _theories;
        public GraphBuilder(ATNEntities Entities = null)
        {
            _theories = new Theories(Entities);
        }
        private double StandardDeviation(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }

        private double StandardDeviation(IEnumerable<int> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }

        public void RemoveFromTree(Dictionary<long, SourceWithReferences> SourceTree, long SourceToRemove)
        {
            List<long> Citations = SourceTree[SourceToRemove].Citations;
            List<long> References = SourceTree[SourceToRemove].References;

            foreach (long Citation in Citations)
            {
                if (SourceTree.ContainsKey(Citation))
                {
                    SourceTree[Citation].References.Remove(SourceToRemove);
                }
            }

            foreach (long Reference in References)
            {
                if (SourceTree.ContainsKey(Reference))
                {
                    SourceTree[Reference].Citations.Remove(SourceToRemove);
                }
            }

            SourceTree.Remove(SourceToRemove);
        }

        public Graph GetGraphForTheory(int TheoryId, bool ImpactFactorCutoff, bool AEFCutoff, bool TARCutoff, bool MachineLearningCutoff, bool YearCutoff)
        {
            const int nSigma = 2;
            Graph ExportGraph = new Graph();

            ExtendedSource[] AllSources = _theories.GetAllExtendedSourcesForTheory(TheoryId, 0, Int32.MaxValue).ToArray();
            Dictionary<long, SourceWithReferences> SourceTree = _theories.GetSourceTreeForTheory(TheoryId);
            foreach (var ExtendedSource in AllSources)
            {
                SourceTree[ExtendedSource.SourceId].IsContributingPrediction = ExtendedSource.isContributingPrediction;
                SourceTree[ExtendedSource.SourceId].PredictionProbability = ExtendedSource.predictionProbability;
            }

            if (ImpactFactorCutoff)
            {
                long CumulativeImpactFactor = 0;
                foreach (SourceWithReferences Source in SourceTree.Values)
                {
                    if (Source.ImpactFactor.HasValue)
                    {
                        CumulativeImpactFactor += Source.ImpactFactor.Value;
                    }
                }
                double CumulativeImpactFactorAverage = CumulativeImpactFactor / SourceTree.Count;
                double CumulativeImpactFactorDeviation = StandardDeviation(SourceTree.Values.Where(s => s.ImpactFactor.HasValue).Select(s => s.ImpactFactor.Value));
                foreach (SourceWithReferences Source in SourceTree.Values.ToArray())
                {
                    if (Source.ImpactFactor.HasValue && Source.ImpactFactor < CumulativeImpactFactorAverage + nSigma * CumulativeImpactFactorDeviation && Source.Depth != 0)
                    {
                        RemoveFromTree(SourceTree, Source.SourceId);
                    }
                }
            }
            if (AEFCutoff)
            {
                double CumulativeArticleLevelEigenFactor = 0;
                foreach (SourceWithReferences Source in SourceTree.Values)
                {
                    if (Source.ArticleLevelEigenFactor.HasValue)
                    {
                        CumulativeArticleLevelEigenFactor += Source.ArticleLevelEigenFactor.Value;
                    }
                }
                double CumulativeArticleLevelEigenFactorAverage = CumulativeArticleLevelEigenFactor / SourceTree.Count;
                double CumulativeArticleLevelEigenFactorDeviation = StandardDeviation(SourceTree.Values.Where(s => s.ArticleLevelEigenFactor.HasValue).Select(s => s.ArticleLevelEigenFactor.Value));
                foreach (SourceWithReferences Source in SourceTree.Values.ToArray())
                {
                    if (Source.ArticleLevelEigenFactor.HasValue && Source.ArticleLevelEigenFactor < CumulativeArticleLevelEigenFactorAverage + nSigma * CumulativeArticleLevelEigenFactorDeviation && Source.Depth != 0)
                    {
                        RemoveFromTree(SourceTree, Source.SourceId);
                    }
                }
            }

            if (TARCutoff)
            {
                double CumulativeTheoryAttributionRatio = 0;
                foreach (SourceWithReferences Source in SourceTree.Values)
                {
                    if (Source.TheoryAttributionRatio.HasValue)
                    {
                        CumulativeTheoryAttributionRatio += Source.TheoryAttributionRatio.Value;
                    }
                }
                double CumulativeTheoryAttributionRatioAverage = CumulativeTheoryAttributionRatio / SourceTree.Count;
                double CumulativeTheoryAttributionRatioDeviation = StandardDeviation(SourceTree.Values.Where(s => s.TheoryAttributionRatio.HasValue).Select(s => s.TheoryAttributionRatio.Value));
                foreach (SourceWithReferences Source in SourceTree.Values.ToArray())
                {
                    if (Source.TheoryAttributionRatio.HasValue && Source.TheoryAttributionRatio < CumulativeTheoryAttributionRatioAverage + nSigma * CumulativeTheoryAttributionRatioDeviation && Source.Depth != 0)
                    {
                        RemoveFromTree(SourceTree, Source.SourceId);
                    }
                }
            }

            //Commented as it does not quite make sense to remove a source
            //unless it cites a set number of other sources
            //if (false)
            //{
            //    long CumulativeOutFactor = 0;
            //    foreach (SourceWithReferences Source in SourceTree.Values)
            //    {
            //        CumulativeOutFactor += Source.OutFactor;
            //    }
            //    double CumulativeOutFactorAverage = CumulativeOutFactor / SourceTree.Count * 3;

            //    foreach (SourceWithReferences Source in SourceTree.Values.ToArray())
            //    {
            //        if (Source.OutFactor < CumulativeOutFactorAverage && Source.Depth != 0)
            //        {
            //            RemoveFromTree(SourceTree, Source.SourceId);
            //        }
            //    }
            //}

            //If MAS's data quality was better this could be beneificial; however,
            //some 2nd level sources may actually be first level, requiring that this
            //be commented for now
            //if (false)
            //{
            //    foreach (SourceWithReferences Source in SourceTree.Values.ToArray())
            //    {
            //        if (Source.Depth > 1)
            //        {
            //            RemoveFromTree(SourceTree, Source.SourceId);
            //        }
            //    }
            //}

            if (MachineLearningCutoff)
            {
                foreach (var Source in SourceTree.Values.Join(AllSources, st => st.SourceId, src => src.SourceId, (st, src) => new {SWR = st, ExtendedSource = src}).ToArray())
                {
                    if (
                        Source.SWR.Depth > 0 &&
                        (
                            !Source.SWR.IsContributingPrediction.HasValue ||
                            !Source.SWR.IsContributingPrediction.Value ||
                            !Source.SWR.PredictionProbability.HasValue ||
                            Source.SWR.PredictionProbability.Value < 1.0d))
                    {
                        RemoveFromTree(SourceTree, Source.SWR.SourceId);
                    }
                }
            }

            int MinYear;
            if (AllSources.Length > 0)
            {
                MinYear = AllSources.Where(s => s.Depth == 0 && s.Year != 0).Min(s => s.Year);
            }
            else
            {
                MinYear = Int32.MaxValue;
            }
            if(MinYear == 0)
            {
                MinYear = Int32.MaxValue;
            }

            if (YearCutoff)
            {
                foreach (var Source in SourceTree.Values.ToArray())
                {
                    if (Source.Year < MinYear)
                    {
                        RemoveFromTree(SourceTree, Source.SourceId);
                    }
                }
            }

            foreach (var Source in SourceTree.Values.ToArray())
            {
                if (Source.References.Count == 0 && Source.Citations.Count == 0)
                {
                    RemoveFromTree(SourceTree, Source.SourceId);
                }
            }

            foreach (var Source in SourceTree.Values.Join(AllSources, st => st.SourceId, src => src.SourceId, (st, src) => new { AlreadyContributing = src.Contributing, Year = src.Year, Title = src.Title, Source = st}))
            {
                ExportGraph.Nodes.Add(new SourceNode(Source.Source.SourceId, Source.Title, Source.Source.ImpactFactor,
                    Source.Year, Source.Source.ArticleLevelEigenFactor, Source.Source.TheoryAttributionRatio, Source.Source.PredictionProbability,
                    Source.Source.IsContributingPrediction, Source.Source.Depth));
                foreach (long Reference in Source.Source.References)
                {
                    if (SourceTree.ContainsKey(Reference))
                    {
                        ExportGraph.Edges.Add(new SourceEdge(Source.Source.SourceId, Reference));
                    }
                }
            }

            return ExportGraph;
        }
    }
}
