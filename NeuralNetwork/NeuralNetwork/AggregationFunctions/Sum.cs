using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.AggregationFunctions
{
    [Serializable]
    public class Sum : AggregationFunction
    {
        public override double aggregate(double[] inputs)
        {
            double sum = 0.0;
            foreach(var input in inputs)
            {
                sum += input;
            }
            return sum;
        }
    }
}
