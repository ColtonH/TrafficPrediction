using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.ActivationFunctions
{
    [Serializable]
    public class Step : ActivationFunction
    {
        private double offset;

        public Step(double offset = 0.0) : base()
        {
            this.offset = offset;
        }

        public override double evaluate(double input)
        {
            if (input > this.offset)
            {
                return 1.0;
            }
            return 0.0;
        }

        public override double derivative(double input)
        {
            throw new NotImplementedException();
        }
    }
}
