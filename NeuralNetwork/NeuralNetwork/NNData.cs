using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class NNData : DataItem 
    {

        public NNData(DateTime date, double temp, double precip, double lat, double lon, Direction dir)
            : base(date, temp, precip, lat, lon, dir)
        {
        }
        
        /// <summary>
        /// Returns the inputs for the NeuralNetwork. 
        /// </summary>
        /// <returns>
        /// Double array with the following values:
        /// [0] - 1 if Sunday, 0 otherwise
        /// [1] - 1 if Monday, 0 otherwise
        /// [2] - 1 if Tuesday, 0 otherwise
        /// [3] - 1 if Wednesday, 0 otherwise
        /// [4] - 1 if Thursday, 0 otherwise
        /// [5] - 1 if Friday, 0 otherwise
        /// [6] - 1 if Saturday, 0 otherwise
        /// 
        /// [7] - Time of day 
        /// [8] - Temperature scaled from (-20, 120)
        /// [9] - Precipitation scaled from (0, 50) inches 
        /// [10] - Latitude scaled from (-90,90)
        /// [11] - Longitude scaled from (-180,180)
        /// 
        /// [12] - 1 if North, 0 otherwise
        /// [13] - 1 if South, 0 otherwise
        /// [14] - 1 if East, 0 otherwise
        /// [15] - 1 if West, 0 otherwise
        /// </returns>
        public override double[] getInputs()
        {
            double[] inputs = new double[16];

            switch(this.date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    inputs[0] = 1; 
                    break;

                case DayOfWeek.Monday:
                    inputs[1] = 1;
                    break;

                case DayOfWeek.Tuesday:
                    inputs[2] = 1;
                    break;

                case DayOfWeek.Wednesday:
                    inputs[3] = 1;
                    break;

                case DayOfWeek.Thursday:
                    inputs[4] = 1;
                    break;

                case DayOfWeek.Friday:
                    inputs[5] = 1;
                    break;

                case DayOfWeek.Saturday:
                    inputs[6] = 1;
                    break;
            }
            
            inputs[7] = DataItem.scaleInput(0,86400000,this.date.TimeOfDay.TotalMilliseconds);
            inputs[8] = DataItem.scaleInput(-20, 130, this.temp);
            inputs[9] = DataItem.scaleInput(0, 50, this.precip);
            inputs[10] = DataItem.scaleInput(-90, 90, this.latitude);
            inputs[11] = DataItem.scaleInput(-180, 180, this.longitude);

            switch(this.direction)
            {
                case Direction.North:
                    inputs[12] = 1;
                    break;

                case Direction.South:
                    inputs[13] = 1;
                    break;

                case Direction.East:
                    inputs[14] = 1;
                    break;

                case Direction.West:
                    inputs[15] = 1;
                    break;
            }

            return inputs;
        }
    }
}
