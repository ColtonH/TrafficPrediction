using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public abstract class DataItem : IComparable<DataItem>
    {
        protected DateTime date;
        protected double temp;
        protected double precip;
        protected double latitude;
        protected double longitude;
        protected Direction direction;

        public enum Direction { North, South, East, West}

        public DataItem(DateTime d, double t, double p, double lat, double lon, Direction dir)
        {
            this.date = d;
            this.temp = t;
            this.precip = p;
            this.latitude = lat;
            this.longitude = lon;
            this.direction = dir;
        }


        public abstract double[] getInputs();


        /// <summary>
        /// Compares the DataItem to another first by date and then by Latitude.
        /// </summary>
        /// <param name="other">The other item to compare to.</param>
        /// <returns>-1 if this item is less than the other. 0 if they are equal. 1 if this item is more than the other.</returns>
        public int CompareTo(DataItem other)
        {
            int result = DateTime.Compare(this.date, other.date);
            if (result != 0)
            {
                return result;
            }

            double diff = this.latitude - other.latitude;
            if (diff < 0)
            {
                return -1;
            }

            if (diff > 0)
            {
                return 1;
            }

            return 0;

        }

        public static double scale(double min, double max, double value, double newMin, double newMax)
        {
            return (value - min) / (max - min) * (newMax - newMin) + newMin;
        }
        public static double scaleInput(double min, double max, double value)
        {
            return scale(min, max, value, -Math.Sqrt(3), Math.Sqrt(3));
        }
    }
}
