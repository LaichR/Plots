using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLib.Interface
{
    public interface ILogicalRangeProvider
    {
        event EventHandler RangeChanged;
        double LogicalX
        {
            get;
        }

        double LogicalWidth
        {
            get;
        }

        double LogicalY
        {
            get;
        }

        double LogicalHeight
        {
            get;
        }

        double Xspacing
        {
            get;
        }
    }
}
