using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using PlotLib.Interface;

namespace PlotLib
{
    public class PlotAxis : PlotFigure
    {

        /// <summary>
        /// Defines the NrOfTicks of this axis
        /// </summary>
        public static readonly DependencyProperty NrOfTicksProperty =
            DependencyProperty.Register("NrOfTicks", typeof(int), typeof(PlotAxis), new PropertyMetadata(OnNrOfTicksChanged));

        public static readonly DependencyProperty UpdateOnPositionChangeProperty =
            DependencyProperty.Register("UpdateOnPositionChange", typeof(bool), typeof(PlotAxis), new PropertyMetadata(true));

        double _tickDistance = 20.0;
        const double _tickLen = 1.0;

        GeometryGroup _myGeometry;
        Point _start, _end;
        LineGeometry _axis;
        //List<LineGeometry> _ticks = new List<LineGeometry>();
        //List<Point> _lablePositions = new List<Point>();
        //List<Geometry> _tickLabels = new List<Geometry>();
        AxisOrientation _myOrientation;
        double _xScaling = 1.0;
        double _yScaling = 1.0;
        double _logicalHight = 100.0;
        double _logicalWidth = 100.0;
        double _xcoord = 0.0;
        bool _redrawAxis = false;
        bool _updateXisOnPositionChange = false;
        

        public PlotAxis()
        {
            Stroke = Brushes.Black;
            StrokeThickness = 1;
            this.Visibility = System.Windows.Visibility.Visible;
            _myGeometry = new GeometryGroup();
            _axis = new LineGeometry();
            NrOfTicks = 5;

        }

        public AxisOrientation Orientation
        {
            get => _myOrientation;
            set
            {
                _myOrientation = value;
            }
        }

        public bool UpdateOnPositionChange
        {
            get => (bool)GetValue(UpdateOnPositionChangeProperty);
            set
            {
                SetValue(UpdateOnPositionChangeProperty, value);
            }
        }

        public int NrOfTicks
        {
            get => (int)GetValue(NrOfTicksProperty);
            set { SetValue(NrOfTicksProperty, value); }
        }

        protected override void ComputeGeometry()
        {
            _myGeometry.Transform = GetTransform();
            DrawAxis(_myOrientation);
        }

        void DrawAxis(AxisOrientation orientation)
        {
            _myGeometry.Children.Clear();
            //_ticks.Clear();
            if (orientation == AxisOrientation.Horizontal)
            {
                var tickLen = _tickLen;
                _start.X = 0;
                _start.Y = 100;
                _end.X = _start.X + 100;
                if ( !Range.IsFromLeftToRight)
                {
                    _end.X = _start.X - Range.LogicalWidth;
                }
                _end.Y = _start.Y;
                var delta = _tickDistance;
                
                var pos = _start.X + delta;
                
                for( int i = 0; i< NrOfTicks; i++)
                {
                    var tick = new LineGeometry(new Point(pos, _start.Y), new Point(pos, _start.Y - tickLen));
                    
                    //_ticks.Add(tick);
                    _myGeometry.Children.Add(tick);
                    pos += delta;
                }
                CreateTickLabelGeometries(Range.LogicalWidth / 100 * _tickDistance, AxisOrientation.Horizontal);
            }
            else
            {
                _start.X = 0;
                _start.Y = 100;
                _end.X = 0;
                _end.Y = 0;
                var delta = _tickDistance;
                var tickLen = _tickLen;
                var pos = 100-delta;
                for (int i = 0; i < NrOfTicks; i++)
                {
                    var tick = new LineGeometry(new Point(_start.X, pos), new Point(_start.X+tickLen, pos));
                    //_ticks.Add(tick);
                    _myGeometry.Children.Add(tick);
                    pos -= delta;
                }
                CreateTickLabelGeometries(Range.LogicalHeight/100*_tickDistance, AxisOrientation.Vertical);
            }
            _axis.StartPoint = _start;
            _axis.EndPoint = _end;
            _myGeometry.Children.Add(_axis);
        }

        protected override Geometry DefiningGeometry => _myGeometry;

        protected override void OnRangeChanged(object sender, EventArgs e)
        {

            Dispatcher.Invoke(() =>
            {
                base.OnRangeChanged(sender, e);
                if (_redrawAxis)
                {

                    DrawAxis(_myOrientation);
                    _redrawAxis = false;
                }
            });
            
        }

        void CreateTickLabelGeometries(double delta, AxisOrientation dir)
        {
            //_tickLabels.Clear();
            
            var lableValue = Range.LogicalX;
            
            var typeFace = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.ExtraLight,
                FontStretches.UltraCondensed);
            
            Func<double, Point> getPos = (value) => new Point(value, 100 + 5);

            if( dir == AxisOrientation.Vertical)
            {
                getPos = (value) => new Point( - 5, 100 - value);
                lableValue = Range.LogicalY;
            }
     
            for ( int i = 0; i < NrOfTicks + 1; i++)
            {
                var formattedLable = new FormattedText(lableValue.ToString(),
                System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.RightToLeft, typeFace, 2.5, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip );
                            
                var tickGeometry = formattedLable.BuildGeometry(getPos(i*_tickDistance));
    
                //_tickLabels.Add(tickGeometry);
                _myGeometry.Children.Add(tickGeometry);
                lableValue += delta;
            }
        }

        protected override Matrix ComputeTransformationMatrix()
        {
            var scaling = Range.ViewPortWidth / 100;
            if ( scaling != _xScaling )
            {
                _xScaling = scaling; _redrawAxis = true;
            }

            scaling = Range.ViewPortHeight / 100;
            if (scaling != _yScaling)
            {
                _yScaling = scaling; _redrawAxis = true;

            }
            if(Range.LogicalX != _xcoord && _myOrientation == AxisOrientation.Horizontal && UpdateOnPositionChange )
            {
                _xcoord = Range.LogicalX; _redrawAxis = true;
            }
            
            if( Range.LogicalHeight != _logicalHight )
            {
                _logicalHight = Range.LogicalHeight; _redrawAxis = true;
            }

            if (Range.LogicalWidth != _logicalWidth)
            {
                _logicalWidth = Range.LogicalWidth; _redrawAxis = true;
            }

            double offsetX = Range.ViewPortX;
            
            return new Matrix(_xScaling, 0, 0, _yScaling, offsetX, 0);
        }


        private static void OnNrOfTicksChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var line = source as PlotAxis;
            if (line != null)
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    line._tickDistance = 100/(int)e.NewValue;
                }
            }
        }

    }

}
