using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.AggregationFunctions
{
    [Serializable]
    public class Product : AggregationFunction
    {
        public override double aggregate(double[] inputs)
        {
            double product = 0.0;
            foreach(var input in inputs)
            {
                product *= input;
            }
            return product;
        }
    }
}
