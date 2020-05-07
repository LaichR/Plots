using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PlotLib.Interface;

namespace PlotLib.DataSources
{
    public class ArrayDataSource : IDataSource
    {
        List<Point> _data = new List<Point>();
        public ArrayDataSource(double[] data)
        {
            int i = 0;
            foreach( var d in data)
            {
                _data.Add(new Point(i++, d));
            }
        }
        public int AvailablePoints => _data.Count;

        public string UnitX => "";

        public string UnitY => "";

        //public double MinX => throw new NotImplementedException();

        //public double MinY => throw new NotImplementedException();

        public event EventHandler DataAvailable;

        public IEnumerator<Point> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}
