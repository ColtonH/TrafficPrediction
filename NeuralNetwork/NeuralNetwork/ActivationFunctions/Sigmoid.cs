using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.ActivationFunctions
{
    [Serializable]
    public class Sigmoid : ActivationFunction
    {
        private double slope;
        public Sigmoid(double slope = 1.0) : base() 
        {
            this.slope = slope;
        }

        public override double evaluate(double input)
        {
            return 1 / (1 + Math.Exp(-1*slope * input));
        }

        public override double derivative(double input)
        {
            return this.evaluate(input) * (1 - this.evaluate(input));
        }
    }
}
