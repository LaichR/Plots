using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLib.Interface
{
    public interface IRangeProvider: ILogicalRangeProvider
    {

        bool IsFromLeftToRight
        {
            get;
        }

        double ViewPortX
        {
            get;
        }

        double ViewPortWidth
        {
            get;
        }

        double ViewPortY
        {
            get;
        }

        double ViewPortHeight
        {
            get;
        }
    }
}
