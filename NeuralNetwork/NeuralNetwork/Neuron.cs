using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    [Serializable]
    public class Neuron
    {


        private double input;
        private double[] weights;
        private double[] dws;
        private ActivationFunctions.ActivationFunction actFun;
        private AggregationFunctions.AggregationFunction aggFun;
        private double output;

        /*
        private delegate double AggregateFunction(double[] inputs);
        private AggregateFunction aggregateFunction;*/

        public Neuron(double[] weights, ActivationFunctions.ActivationFunction actFun, AggregationFunctions.AggregationFunction aggFun) 
        {
            this.weights = weights;
            this.actFun = actFun;
            this.aggFun = aggFun;
            this.input = 0;
            this.dws = new double[weights.Length];
            this.output = 0;
        }


        public double evaluate(double[] inputs)
        {
            double[] products = new double[weights.Length];
            if(inputs.Length+1 != this.weights.Length)
            {
                int abc = 0;
                abc++;
                return 0.0;
                throw new Exception(); //TODO: create exception type
            }
            for(int i=0; i<inputs.Length; i++)
            {
                products[i] = inputs[i] * this.weights[i];
            }
            products[inputs.Length] = -1 * this.weights.Last();
            double aggTotal = this.aggFun.aggregate(products);
            this.input = aggTotal;
            this.output = this.actFun.evaluate(aggTotal);
            return this.output;
        }       

        public double getOutput()
        {
            return this.output;
        }
        public double getInput()
        {
            return this.input;
        }
        public double[] getWeights()
        {
            return this.weights;
        }

        public void setWeights(double[] weights)
        {
            this.weights = weights;
        }

        public double[] getDWs()
        {
            return this.dws;
        }
        public void setDW(int index, double dw)
        {
            this.dws[index] = dw;
        }
        public ActivationFunctions.ActivationFunction getActivationFunction()
        {
            return this.actFun;
        }
    }
    
}
