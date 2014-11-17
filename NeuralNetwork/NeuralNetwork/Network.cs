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

        public Network(Layer hiddenLayer1, Layer hiddenLayer2, Layer outputLayer)
        {
            this.layers = new Layer[3];
            this.layers[0] = hiddenLayer1;
            this.layers[1] = hiddenLayer2;
            this.layers[2] = outputLayer;
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
            int outLayer = this.layers.Count()-1;
            for (int k=0; k<this.layers[outLayer].getNeurons().Length; k++)
            {
                double[] weights = new double[this.layers[outLayer-1].getNeurons().Length+1];

                Neuron n = this.layers[outLayer].getNeurons()[k];
                for (int j=0; j<this.layers[outLayer-1].getNeurons().Length; j++)
                {
                    double dw = eta*(targets[k]-values[k])*n.getActivationFunction().derivative(n.getInput())*this.layers[outLayer-1].getNeurons()[j].getOutput();
                    weights[j] = n.getWeights()[j] + dw + alpha * n.getDWs()[j];
                    this.layers[outLayer].getNeurons()[k].setDW(j, dw);
                }
                this.layers[outLayer].getNeurons()[k].setWeights(weights);
            }

            if (layers.Count() == 2)
            {
                // Update hidden layer weights
                for (int j = 0; j < this.layers[0].getNeurons().Length; j++)
                {
                    double[] weights = new double[inputs.Length + 1];
                    Neuron n = this.layers[0].getNeurons()[j];
                    for (int i = 0; i < n.getWeights().Length; i++)
                    {
                        double dv = 0;
                        for (int k = 0; k < values.Length; k++)
                        {
                            dv += (targets[k] - values[k]) * this.layers[1].getNeurons()[k].getActivationFunction().derivative(this.layers[1].getNeurons()[k].getInput()) * this.layers[1].getNeurons()[k].getWeights()[j];
                        }
                        dv = eta * dv * n.getActivationFunction().derivative(n.getInput()) * (i < inputs.Length ? inputs[i] : -1);
                        weights[i] = n.getWeights()[i] + dv + alpha * n.getDWs()[i];
                        this.layers[0].getNeurons()[j].setDW(i, dv);
                    }
                    this.layers[0].getNeurons()[j].setWeights(weights);
                }
            }
            else
            {
                // Update the second hidden layer weights
                for(int j=0; j<this.layers[1].getNeurons().Length; j++)
                {
                    double[] weights = new double[this.layers[0].getNeurons().Length + 1];
                    Neuron n = this.layers[1].getNeurons()[j];
                    for (int i=0; i<n.getWeights().Length; i++)
                    {
                        double dv = 0;
                        for(int k=0; k<values.Length; k++)
                        {
                             dv += (targets[k] - values[k]) * this.layers[2].getNeurons()[k].getActivationFunction().derivative(this.layers[2].getNeurons()[k].getInput()) * this.layers[2].getNeurons()[k].getWeights()[j];
                        }
                        dv = eta * dv * n.getActivationFunction().derivative(n.getInput()) * (i < inputs.Length ? this.layers[0].getNeurons()[i].getOutput() : -1);
                        weights[i] = n.getWeights()[i] + dv + alpha * n.getDWs()[i];
                        this.layers[1].getNeurons()[j].setDW(i, dv);
                    }
                    this.layers[1].getNeurons()[j].setWeights(weights);
                }

                // Update the first hidden layer weights
                for(int j=0; j<this.layers[0].getNeurons().Length; j++)
                {
                    double[] weights = new double[inputs.Length + 1];
                    Neuron n = this.layers[0].getNeurons()[j];
                    for(int i=0; i<n.getWeights().Length; i++)
                    {
                        double dv = 0;
                        for(int k=0; k<this.layers[1].getNeurons().Length; k++)
                        {
                            dv += this.layers[1].getNeurons()[k].getDWs()[j];
                            //for (int m = 0; m < this.layers[2].getNeurons().Length; m++)
                            //{
                            //    Neuron n2 = this.layers[2].getNeurons()[m];
                            //   dv += n.getActivationFunction().derivative(n.getInput()) * this.layers[1].getNeurons()[k].getDWs()[j]*n2.getActivationFunction().derivative(n2.getInput())*n2.getDWs()[k]; //TODO: 1st hidden layer update. This is where it gets complicated    
                            //}
                        }
                        dv = eta * dv * n.getActivationFunction().derivative(n.getInput()) * (i < inputs.Length ? inputs[i] : -1);
                        weights[i] = n.getWeights()[i] + dv + alpha * n.getDWs()[i];
                        this.layers[0].getNeurons()[j].setDW(i, dv);
                    }
                    this.layers[0].getNeurons()[j].setWeights(weights);
                }
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
                if(iteration%100 == 0)
                    Console.Out.WriteLine("Training iteration "+iteration);
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
