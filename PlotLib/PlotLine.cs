using PlotLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;


namespace PlotLib
{
    public class PlotLine : PlotFigure
    {

        /// <summary>
        /// Defines the LineColor of this line
        /// </summary>
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Color),
            typeof(PlotLine),
            new PropertyMetadata(OnLineColorChanged));

        PathGeometry _path;
        PathFigure _pathFigure;
        RectangleGeometry _clipGeometry;
        
        public PlotLine() 
        {
            Stroke = Brushes.Black;
            StrokeThickness = 1;
            this.Visibility = System.Windows.Visibility.Visible;
            _path = new PathGeometry();   
        }

        public Color LineColor
        {
            get
            {
                return (Color)GetValue(LineColorProperty);
            }
            set
            {
                SetValue(LineColorProperty, value);
            }
        }

        public override void AttacheDataSource(IDataSource dataSource)
        {
            base.AttacheDataSource(dataSource);
            _path.Figures.Clear();
            if (DataPoints.Count > 1)
            {
                _pathFigure = GetPathFigureFromPoints(DataPoints);
                _path.Figures.Add(_pathFigure);
            }
        }

        protected override void ComputeGeometry()
        {
            
            _path.Transform = GetTransform();
            _clipGeometry = new RectangleGeometry(GetClipArea());
            this.Clip = _clipGeometry;
        }

        protected override void OnRangeChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                base.OnRangeChanged(sender, e);
                _clipGeometry.Rect = GetClipArea();
            });
        }

        public override void OnDataAvailable(object sender, EventArgs e)
        {
            try
            {
                Dispatcher.Invoke( () =>
                {
                    IDataSource ds = sender as IDataSource;
                    if (this._pathFigure == null)
                    {
                        this._pathFigure = GetPathFigureFromPoints(ds);
                        _path.Figures.Add(_pathFigure);
                    }
                    else
                    {
                        UpdateLineSegmentsInPathFigure(this._pathFigure, ds, 200);
                    }
                    this.InvalidateVisual();
                });
            }
            catch
            {

            }
            
        }

        protected override Geometry DefiningGeometry => _path;

        private static void OnLineColorChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var line = source as PlotLine;
            if (line != null)
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    line.Stroke = new SolidColorBrush((Color)e.NewValue);
                }
            }
        }

    }
}
