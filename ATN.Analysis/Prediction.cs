using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Analysis
{
    public class Prediction
    {
        public bool Prediction;
        public double Probability;

        public Prediction(bool pred, double prob)
        {
            Prediction = pred;
            Probability = prob;
        }
    }
}
