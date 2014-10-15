using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.ActivationFunctions
{
    [Serializable]
    public abstract class ActivationFunction
    {
        public abstract double evaluate(double input);
        public abstract double derivative(double input);
    }
}
