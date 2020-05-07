using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlotLib;
using PlotLib.Interface;
using PlotLib.DataSources;
using System.Timers;


namespace Plots
{
    class PlotViewModel
    {
        ArrayDataSource _dataSource;
        DynamicDataSource _dynamicDataSource;
        Timer _t;
        Random _r = new Random();

        public PlotViewModel()
        {
            List<double> data = new List<double>();
            for(int i = 0; i< 100; i++)
            {
                data.Add(i);
            }
            _dataSource = new ArrayDataSource(data.ToArray());
            _dynamicDataSource = new DynamicDataSource(0, 0, 1, 100.0, 100.0);
            for (int i = 0; i < 3; i++)
            {
                var nextRandomValue = _r.NextDouble() * 10.0 + 50.0;
                _dynamicDataSource.PutData(nextRandomValue);
            }
            _t = new Timer(10);
            _t.Elapsed += _t_Elapsed;
            _t.Start();
        }

        private void _t_Elapsed(object sender, ElapsedEventArgs e)
        {
            var nextRandomValue = _r.NextDouble() * 10.0 + 50.0;
            _dynamicDataSource.PutData(nextRandomValue);
        }

        public IDataSource SampleArray
        {
            get => _dataSource;
        }

        public IDataSource RandomData
        {
            get => _dynamicDataSource;
        }

    }
}
