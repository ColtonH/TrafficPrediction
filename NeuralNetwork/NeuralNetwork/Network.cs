using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    [Serializable]
    public class Network
    {
        Layer[] layers;

        public Network(Layer hiddenLayer, Layer outputLayer)
        {
            this.layers = new Layer[2];
            this.layers[0] = hiddenLayer;
            this.layers[1] = outputLayer;
        }

        public double[] evaluate(double[] inputs)
        {
            double[] output = layers[0].evaluate(inputs);
            for (int i = 1; i < layers.Length; i++ )
            {
                output = layers[i].evaluate(output);
            }
            return output;
        }

        private void update(double[] inputs, double[] targets, double alpha, double eta)
        {
            double[] values = this.evaluate(inputs);
            if(values.Length != targets.Length)
            {
                throw new Exception(); //TODO: Exception?
            }
            

            // Update output layer weights
            for (int k=0; k<this.layers[1].getNeurons().Length; k++)
            {
                double[] weights = new double[this.layers[0].getNeurons().Length+1];

                Neuron n = this.layers[1].getNeurons()[k];
                for (int j=0; j<this.layers[0].getNeurons().Length; j++)
                {
                    double dw = eta*(targets[k]-values[k])*n.getActivationFunction().derivative(n.getInput())*this.layers[0].getNeurons()[j].getOutput();
                    weights[j] = n.getWeights()[j] + dw + alpha * n.getDWs()[j];
                    this.layers[1].getNeurons()[k].setDW(j, dw);
                }
                this.layers[1].getNeurons()[k].setWeights(weights);
            }

            // Update hidden layer weights
            for(int j=0; j<this.layers[0].getNeurons().Length; j++)
            {
                double[] weights = new double[inputs.Length+1];
                Neuron n = this.layers[0].getNeurons()[j];
                for (int i=0; i<n.getWeights().Length; i++)
                {
                    double dv = 0;
                    for (int k=0; k<values.Length; k++)
                    {
                        dv += (targets[k] - values[k])*this.layers[1].getNeurons()[k].getActivationFunction().derivative(this.layers[1].getNeurons()[k].getInput())*this.layers[1].getNeurons()[k].getWeights()[j];
                    }
                    dv = eta * dv * n.getActivationFunction().derivative(n.getInput()) * (i<inputs.Length ? inputs[i]: -1);
                    weights[i] = n.getWeights()[i] + dv + alpha * n.getDWs()[i];
                    this.layers[0].getNeurons()[j].setDW(i, dv);
                }
                this.layers[0].getNeurons()[j].setWeights(weights);
            }

        }

        public void train(List<Dataset> trainingData, List<Dataset> validationData, int maxIterations = 1000, double alpha = 0.01, double eta=0.9)
        {
            int iteration = 1;
            do{
                if(iteration > maxIterations)
                {
                    //throw new Exception(); //TODO: Exception?
                    break;
                }
                foreach (var tData in trainingData)
                {
                    update(tData.getInputs(), tData.getTargets(), alpha, eta);
                }
                iteration++;
            }while(validate(validationData) < .9);
        }


        private double validate(List<Dataset> validationData)
        {
            double failed = 0;
            foreach(var data in validationData)
            {
                double[] outputs = evaluate(data.getInputs());
                for(int i=0; i<outputs.Length; i++)
                {
                    double error = 0.0; // Error using Sum Squared Error
                    for (int j = 0; j < outputs.Length; j++)
                    {
                        error = Math.Pow(data.getTargets()[j] - outputs[j], 2);
                    }
                    error /= 2 * outputs.Length;
                    if(error<0.9)
                    {
                        failed++;
                    }
                }
            }
            
            return 1-(failed/validationData.Count);
        }

        public double scale(double min, double max, double value, double newMin, double newMax)
        {
            return (value-min)/(max-min)*(newMax-newMin)+newMin;
        }
        public double scaleInput(double min, double max, double value)
        {
            return scale(min, max, value, -Math.Sqrt(3), Math.Sqrt(3));
        }
        
    }
}
