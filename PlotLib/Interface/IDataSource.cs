using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlotLib.Interface
{
    public interface IDataSource: IEnumerable<Point>
    {
        event EventHandler DataAvailable;

        int AvailablePoints
        {
            get;
        }
        
        string UnitX
        {
            get;
        }

        string UnitY
        {
            get;
        }

        //double MinX
        //{
        //    get;
        //}

        
        //double MinY
        //{
        //    get;
        //}
    }
}
