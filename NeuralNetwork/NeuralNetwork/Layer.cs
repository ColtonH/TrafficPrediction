using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    [Serializable]
    public class Layer
    {
        private Neuron[] neurons;

        public Layer(int numInputs, int numNeurons, ActivationFunctions.ActivationFunction actFun, AggregationFunctions.AggregationFunction aggFun)
        {
            neurons = new Neuron[numNeurons];
            Random rand = new Random();
            for(int i=0; i<numNeurons; i++)
            {
                double[] weights = new double[numInputs+1];
                for(int j=0; j<weights.Length; j++) // Use [0,numInputs] (numInputs+1) weights to allow for bias unit. 
                {
                    weights[j] = (rand.NextDouble()*2-1);
                }
                neurons[i] = new Neuron(weights, actFun, aggFun);
            }
        }

        public Layer(int numInputs, ActivationFunctions.ActivationFunction[] activationFunctions, AggregationFunctions.AggregationFunction[] aggregationFunctions)
        {
            Random rand = new Random();
            if(activationFunctions.Length != aggregationFunctions.Length)
            {
                throw new Exception(); //TODO: New exception type?
            }
            
            neurons = new Neuron[activationFunctions.Length];
            for(int i=0; i<neurons.Length; i++)
            {
                double[] weights = new double[numInputs+1];
                for(int j=0; j<=numInputs; j++)
                {
                    weights[i] = (rand.NextDouble()*2-1);
                }
                neurons[i] = new Neuron(weights, activationFunctions[i], aggregationFunctions[i]);
            }
        }

        public Layer(params Neuron[] neurons)
        {
            this.neurons = neurons;
        }

        public double[] evaluate(double[] inputs)
        {
            double[] outputs = new double[neurons.Length];
            for (int i = 0; i < neurons.Length; i++ )
            {
                outputs[i] = neurons[i].evaluate(inputs);
            }
            return outputs;
        }

        public Neuron[] getNeurons()
        {
            return this.neurons;
        }
    }
}
