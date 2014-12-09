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
            
            Network ann = null;
            Random r = new Random();
            List<Dataset> testing = new List<Dataset>();

            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Open a trained ANN";
            open.Filter = "Artificial Neural Network | *.ann";
            if (open.ShowDialog() != DialogResult.OK || !File.Exists(open.FileName))
            {
                NeuralNetwork.ActivationFunctions.Sigmoid sigmoid = new NeuralNetwork.ActivationFunctions.Sigmoid();
                NeuralNetwork.AggregationFunctions.Sum sum = new NeuralNetwork.AggregationFunctions.Sum();

                Layer hiddenLayer = new Layer(16, 20, sigmoid, sum);
                Layer hiddenLayer2 = new Layer(20, 10, sigmoid, sum);
                Layer outputLayer = new Layer(10, 1, sigmoid, sum);

                ann = new Network(hiddenLayer, hiddenLayer2, outputLayer);

                #region Data Import
                OpenFileDialog dataOpen = new OpenFileDialog();

                open.Title = "Open the data file";
                open.Filter = "Comma Separated Values File | *.csv";

                if(dataOpen.ShowDialog() != DialogResult.OK || !File.Exists(dataOpen.FileName))
                {
                    MessageBox.Show("The data file could not be opened.\nThe project will now terminate.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                List<NNData> allData = new List<NNData>();

                StreamReader reader = new StreamReader(File.OpenRead(dataOpen.FileName));

                Console.Out.WriteLine("Importing Data");
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    if(values[0] == "Sensor ID")
                    {
                        continue;
                    }
                    double lat = Convert.ToDouble(values[1]);
                    double lon = Convert.ToDouble(values[2]);
                    DataItem.Direction direction = DataItem.Direction.North;
                    switch(values[3])
                    {
                        case "North":
                            direction = DataItem.Direction.North; break;
                        case "South":
                            direction = DataItem.Direction.South; break;
                        case "East":
                            direction = DataItem.Direction.East; break;
                        case "West":
                            direction = DataItem.Direction.West; break;
                    }
                    DateTime date = Convert.ToDateTime(values[4]);
                    double temp = Convert.ToDouble(values[5]);
                    double precip = Convert.ToDouble(values[6]);
                    double speed = Convert.ToDouble(values[7]);

                    allData.Add(new NNData(date, temp, precip, lat, lon, direction,speed));
                }
                Console.Out.WriteLine("Data Imported.");
                Console.Out.WriteLine("Sorting Data.");
                List<Dataset> training = new List<Dataset>();
                
                List<Dataset> validation = new List<Dataset>();
                
                foreach(NNData item in allData)
                {
                    double[] inputs = item.getInputs();
                    double[] targets = new double[1];
                    targets[0] = DataItem.scale(0,100,item.getSpeed(),0,1);

                    double prob = r.NextDouble(); // Assign data items to training with a probability of .5, validation with a probability of .3, and testing with a probability of .2
                    if(prob <= .5)
                    {
                        training.Add(new Dataset(inputs, targets));
                    }
                    else if(prob > .8)
                    {
                        testing.Add(new Dataset(inputs, targets));
                    }
                    else
                    {
                        validation.Add(new Dataset(inputs, targets));
                    }
                }
                Console.Out.WriteLine("Data Sorted.");
                Console.Out.WriteLine("Number in Training set: "+training.Count);
                Console.Out.WriteLine("Number in Validation set: " + validation.Count);
                Console.Out.WriteLine("Number in Testing set: " + testing.Count);

#endregion Data Import

                Console.Out.WriteLine("\n\nTraining");
                ann.train(training, validation, 100);

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

                OpenFileDialog dataOpen = new OpenFileDialog();

                open.Title = "Open the data file";
                open.Filter = "Comma Separated Values File | *.csv";

                if (dataOpen.ShowDialog() != DialogResult.OK || !File.Exists(dataOpen.FileName))
                {
                    MessageBox.Show("The data file could not be opened.\nThe project will now terminate.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                List<NNData> allData = new List<NNData>();

                StreamReader reader = new StreamReader(File.OpenRead(dataOpen.FileName));

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    if (values[0] == "Sensor ID")
                    {
                        continue;
                    }
                    double lat = Convert.ToDouble(values[1]);
                    double lon = Convert.ToDouble(values[2]);
                    DataItem.Direction direction = DataItem.Direction.North;
                    switch (values[3])
                    {
                        case "North":
                            direction = DataItem.Direction.North; break;
                        case "South":
                            direction = DataItem.Direction.South; break;
                        case "East":
                            direction = DataItem.Direction.East; break;
                        case "West":
                            direction = DataItem.Direction.West; break;
                    }
                    DateTime date = Convert.ToDateTime(values[4]);
                    double temp = Convert.ToDouble(values[5]);
                    double precip = Convert.ToDouble(values[6]);
                    double speed = Convert.ToDouble(values[7]);

                    allData.Add(new NNData(date, temp, precip, lat, lon, direction, speed));
                }

                foreach(DataItem di in allData)
                {
                   if( r.NextDouble() < 0.2)
                   {
                       double[] targets = new double[1];
                       targets[0] = DataItem.scale(0, 100, di.getSpeed(), 0, 1);
                       testing.Add(new Dataset(di.getInputs(), targets));
                   }
                }

            }

            List<string> lines = new List<string>();
            lines.Add("Target,Value,Error");
            double error = 0;

           foreach (Dataset ds in testing)
           {
               string line = "";
               double target = DataItem.scale(0, 1, ds.getTargets()[0], 0, 100);
               double value = DataItem.scale(0, 1, ann.evaluate(ds.getInputs())[0], 0, 100);

               error += Math.Abs(target - value);

               line = target + "," + value + "," + Math.Abs(target - value);
               lines.Add(line);

           }
           error /= testing.Count;

           Console.Out.WriteLine("The Mean Absolute Error for the ANN is :" + error);
           Console.Out.WriteLine("");
           Console.Out.WriteLine("Press Enter to exit...");
           Console.In.ReadLine();

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
