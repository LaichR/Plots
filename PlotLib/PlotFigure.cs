using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using PlotLib.Interface;
using System.Windows;

namespace PlotLib
{
    public abstract class PlotFigure : Shape
    {



        List<Point> _dataPoints;
        IDataSource _dataSource;
        MatrixTransform _transform;
        IRangeProvider _range;
        
        public PlotFigure()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        public virtual void AttacheDataSource(IDataSource dataSource)
        {
            // axis do not require a data source but only a range
            if (dataSource != null)
            {
                _dataSource = dataSource;
                _dataPoints = new List<Point>(_dataSource.Select<Point, Point>(
                    (x) => x)); // _transform.Transform(x)));
                _dataSource.DataAvailable += OnDataAvailable;
            }
        }

        public virtual MatrixTransform GetTransform()
        {
            return _transform;
        }

        public virtual Rect GetClipArea()
        {
            System.Windows.Rect clipBounds = new System.Windows.Rect(Range.ViewPortX,
                Range.ViewPortY, Range.ViewPortWidth, Range.ViewPortHeight);
            return clipBounds;
        }

        public virtual void OnDataAvailable(object sender, EventArgs e)
        {
            _dataPoints.AddRange(_dataSource);
        }

        public IRangeProvider Range
        {
            get => _range;
            set
            {
                if( _range != null)
                {
                    _range.RangeChanged -= OnRangeChanged;
                }
                _range = value;
                _range.RangeChanged += OnRangeChanged;
                _transform = new MatrixTransform(ComputeTransformationMatrix());
                ComputeGeometry();
            }
        }

        protected List<Point> DataPoints
        {
            get => _dataPoints;
        }

        protected IDataSource DataSource
        {
            get => _dataSource;
        }

        protected abstract void ComputeGeometry();

        protected virtual void OnRangeChanged(object sender, EventArgs e)
        {
           Dispatcher.Invoke(() =>
           {
               _transform.Matrix = ComputeTransformationMatrix();
               
           });
        }

        public static void UpdateLineSegmentsInPathFigure(PathFigure pf, IEnumerable<Point> newPoints, int maxNumOfSegements)
        {
            if (pf == null) 
                return;

            foreach( var p in newPoints)
            {
                var ls = new LineSegment(p, true);
                pf.Segments.Add(ls);
            }
            var nrOfSegments = pf.Segments.Count();
            if (nrOfSegments > maxNumOfSegements+1)
            {
                while (nrOfSegments > maxNumOfSegements + 1)
                {
                    pf.Segments.RemoveAt(0);
                    nrOfSegments = pf.Segments.Count();
                }
                var lineSegment = pf.Segments[0] as LineSegment;
                pf.StartPoint = lineSegment.Point;
                pf.Segments.RemoveAt(0);
            }
            
        }

        public static PathFigure GetPathFigureFromPoints(IEnumerable<Point> linePoints)
        {
            var pf = new PathFigure();
            PathSegmentCollection pathSegments = new System.Windows.Media.PathSegmentCollection();
            var start = linePoints.First();
            foreach (var p in linePoints.Skip(1))
            {
                pathSegments.Add(new System.Windows.Media.LineSegment(p, true));
            }
            pf.StartPoint = start;
            pf.Segments = pathSegments;
            pf.IsClosed = false;
            return pf;
        }

        protected virtual Matrix ComputeTransformationMatrix( )
        {
            double xScaling = Range.ViewPortWidth / Range.LogicalWidth;
            double offsetX = Range.ViewPortX-xScaling*Range.LogicalX;

            if (!Range.IsFromLeftToRight)
            {
                xScaling = -xScaling;
                offsetX = Range.ViewPortWidth - xScaling*(Range.LogicalWidth-Range.LogicalX);
            }
            return new Matrix(xScaling, 0,
               0, -Range.ViewPortHeight / Range.LogicalHeight,
               offsetX, Range.ViewPortHeight);
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            
        }
    }
}
