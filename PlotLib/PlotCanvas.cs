using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlotLib.Interface;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlotLib
{
    public class PlotCanvas: Canvas, IRangeProvider
    {
        

        public PlotCanvas()
        {
            IsFromLeftToRight = true;
            this.SizeChanged += PlotCanvas_SizeChanged;   
        }

        public event EventHandler RangeChanged;

        public void AddPlotFigure(PlotFigure figure)
        {
            Children.Add(figure);
            figure.Range = this;
        }

        public void SetLogicalRangeProvider( ILogicalRangeProvider rangeProvider )
        {
            rangeProvider.RangeChanged += RangeProvider_RangeChanged;
            AssignRange(rangeProvider);
        }

        public double LogicalWidth
        {
            get;
            set;
        }

        public bool IsFromLeftToRight
        {
            get;
            set;
        }

        public double LogicalHeight
        {
            get;
            set;
        }

        public double LogicalX
        {
            get;
            set;
        }

        public double LogicalY
        {
            get;
            set;
        }

        public double Xspacing
        {
            get;
            set;
        }

        public double ViewPortX => this.ActualWidth*0.1;

        public double ViewPortY => this.ActualHeight*0.01;
        public double ViewPortWidth => this.ActualWidth *0.9;

        public double ViewPortHeight => this.ActualHeight*0.9 ;

        //public Transform ToVisual => TransformToVisual;
        

        void NotifyRangeChanged()
        {
            
            if( RangeChanged!= null)
            {
                RangeChanged(this, EventArgs.Empty);
            }
        }

        private void PlotCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            NotifyRangeChanged();
        }

        private void AssignRange(ILogicalRangeProvider rangeProvider)
        {
            if (rangeProvider != null)
            {
                LogicalX = rangeProvider.LogicalX;
                LogicalY = rangeProvider.LogicalY;
                LogicalWidth = rangeProvider.LogicalWidth;
                LogicalHeight = rangeProvider.LogicalHeight;
                NotifyRangeChanged();
            }
        }

        private void RangeProvider_RangeChanged(object sender, EventArgs e)
        {
            ILogicalRangeProvider rangeProvider = sender as ILogicalRangeProvider;
            AssignRange(rangeProvider);
        }
    }
}
