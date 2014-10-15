using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.AggregationFunctions
{
    [Serializable]
    public abstract class AggregationFunction
    {
        public abstract double aggregate(double[] inputs);
    }
}
