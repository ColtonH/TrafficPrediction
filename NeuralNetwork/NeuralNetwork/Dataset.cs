using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class Dataset
    {
        private double[] inputs;
        private double[] targets;

        public Dataset(double[] inputs, double[] targets)
        {
            this.inputs = inputs;
            this.targets = targets;
        }

        public double[] getInputs()
        {
            return this.inputs;
        }

        public double[] getTargets()
        {
            return this.targets;
        }
        
    }
}
