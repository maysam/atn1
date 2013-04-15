using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Analysis
{
    public class Prediction
    {
        public bool IsContributingPrediction;
        public double PredictionProbability;

        public Prediction(bool pred, double prob)
        {
            IsContributingPrediction = pred;
            PredictionProbability = prob;
        }
    }
}
