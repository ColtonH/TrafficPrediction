using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;


namespace Network_Tester
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Random r = new Random();
            Network ann = null;
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Open a trained ANN";
            open.Filter = "Artificial Neural Network | *.ann";
            if (open.ShowDialog() != DialogResult.OK || !File.Exists(open.FileName))
            {
                NeuralNetwork.ActivationFunctions.Sigmoid sigmoid = new NeuralNetwork.ActivationFunctions.Sigmoid();
                NeuralNetwork.AggregationFunctions.Sum sum = new NeuralNetwork.AggregationFunctions.Sum();

                Layer hiddenLayer = new Layer(2, 20, sigmoid, sum);
                Layer hiddenLayer2 = new Layer(20, 20, sigmoid, sum);
                Layer outputLayer = new Layer(20, 1, sigmoid, sum);

                ann = new Network(hiddenLayer, hiddenLayer2, outputLayer);



                List<Dataset> inputs = new List<Dataset>();
                for (int i = 0; i < 500; i++)
                {
                    double[] input = new double[2];
                    double[] val = new double[1];

                    input[0] = (r.NextDouble() * 10) - 5;
                    input[1] = (r.NextDouble() * 10) - 5;

                    val[0] = ann.scale(-10, 100, Math.Pow(input[0],2)+Math.Pow(input[1],2), 0.2, .8);

                    input[0] = ann.scaleInput(-10, 10, input[0]);
                    input[1] = ann.scaleInput(-10, 10, input[1]);

                    inputs.Add(new Dataset(input, val));
                }
                List<Dataset> verify = new List<Dataset>();
                for (int i = 0; i < 50; i++)
                {
                    double[] input = new double[2];
                    double[] val = new double[1];

                    input[0] = (r.NextDouble() * 10) - 5;
                    input[1] = (r.NextDouble() * 10) - 5;

                    val[0] = ann.scale(-10, 100, Math.Pow(input[0], 2) + Math.Pow(input[1], 2), 0.2, .8);

                    input[0] = ann.scaleInput(-10, 10, input[0]);
                    input[1] = ann.scaleInput(-10, 10, input[1]);

                    verify.Add(new Dataset(input, val));
                }

                ann.train(inputs, verify);

                SaveFileDialog save = new SaveFileDialog();
                save.Title = "Save Artificial Neural Network";
                save.Filter = "Artifical Neural Network | *.ann";
                if (save.ShowDialog() == DialogResult.OK)
                {
                    using (Stream serialStream = File.Create(save.FileName))
                    {
                        BinaryFormatter serializer = new BinaryFormatter();
                        serializer.Serialize(serialStream, ann);
                    }
                }
            }
            else
            {
                using(Stream instream = File.OpenRead(open.FileName))
                { 
                    BinaryFormatter deserializer = new BinaryFormatter();
                    ann = (Network)deserializer.Deserialize(instream);
                }
            }

            List<string> lines = new List<string>();
            for (int i = 0; i < 200; i++)
            {
                double[] input = new double[1];

                input[0] = (r.NextDouble() * Math.PI*2);

                input[0] = ann.scaleInput(-5, 10, input[0]);


                lines.Add(ann.scale(-Math.Sqrt(3), Math.Sqrt(3), input[0], -5, 10) + "," +ann.scale(.2, .8, ann.evaluate(input)[0], -5, 5));
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\John\Desktop\test.csv"))
            {
                foreach (var line in lines)
                {
                    file.WriteLine(line);
                }
            }


        }
    }
}
