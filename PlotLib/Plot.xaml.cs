using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PlotLib.Interface;
using System.ComponentModel;

namespace PlotLib
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    
    public partial class Plot : UserControl
    {
        /// <summary>
        /// Defines the title of the plot
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
            typeof(Plot),
            new PropertyMetadata(OnTitleChanged));

        /// <summary>
        /// Defines the uppder bound of the plot in y dimiension
        /// </summary>
        public static readonly DependencyProperty LogicalHeightProperty = DependencyProperty.Register("LogicalHeight", typeof(double),
            typeof(Plot),
            new PropertyMetadata(OnLogicalHeightChanged));

        /// <summary>
        /// Defines the lower bound of the plot in the y dimension
        /// </summary>
        public static readonly DependencyProperty LogicalYProperty = DependencyProperty.Register("LogicalY", typeof(double),
            typeof(Plot),
            new PropertyMetadata(OnLogicalYChanged));

        /// <summary>
        /// Defines the uppder bound of the plot in y dimiension
        /// </summary>
        public static readonly DependencyProperty LogicalWidthProperty = DependencyProperty.Register("LogicalWidth", typeof(double),
            typeof(Plot),
            new PropertyMetadata(OnLogicalWidthChanged));

        /// <summary>
        /// Defines the lower bound of the plot in the y dimension
        /// </summary>
        public static readonly DependencyProperty LogicalXProperty = DependencyProperty.Register("LogicalX", typeof(double),
            typeof(Plot),
            new PropertyMetadata(OnLogicalXChanged));

        /// <summary>
        /// Defines the space between two points on x coordinates
        /// </summary>
        public static readonly DependencyProperty XspacingProperty = DependencyProperty.Register("Xspacing", typeof(double),
            typeof(Plot),
            new PropertyMetadata(OnXspacingChanged));

        /// <summary>
        /// Defines the Source of the data to be displayed!
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource",
            typeof(Interface.IDataSource), typeof(Plot),
            new PropertyMetadata(OnDataSourceChanged));

        /// <summary>
        /// Defines the Source of the data to be displayed!
        /// </summary>
        public static readonly DependencyProperty LogicalRangeProperty = DependencyProperty.Register("LogicalRange",
            typeof(Interface.ILogicalRangeProvider), typeof(Plot),
            new PropertyMetadata(OnLogicalRangeChanged));

        /// <summary>
        /// Defines the Source of the data to be displayed!
        /// </summary>
        public static readonly DependencyProperty PlotElementsProperty = DependencyProperty.Register("PlotElements",
            typeof(List<PlotFigure>), typeof(Plot),
            new PropertyMetadata(new List<PlotFigure>()));


        IDataSource _dataSource;


        public Plot()
        {
            InitializeComponent();
        }

        public IDataSource DataSource
        {
            get => (IDataSource)GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }

        public ILogicalRangeProvider LogicalRange
        {
            get => (ILogicalRangeProvider)GetValue(LogicalRangeProperty);
            set => SetValue(LogicalRangeProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public double LogicalHeight
        {
            get => (double)GetValue(LogicalHeightProperty);
            set => SetValue(LogicalHeightProperty, value);
        }

        public double LogicalWidth
        {
            get => (double)GetValue(LogicalWidthProperty);
            set => SetValue(LogicalWidthProperty, value);
        }

        public double LogicalY
        {
            get => (double)GetValue(LogicalYProperty);
            set => SetValue(LogicalYProperty, value);
        }

        public double LogicalX
        {
            get => (double)GetValue(LogicalXProperty);
            set => SetValue(LogicalXProperty, value);
        }

        [TypeConverter(typeof(PlotFigureConverter))]
        public List<PlotFigure> PlotElements
        {
            get => (List<PlotFigure>)GetValue(PlotElementsProperty);
            set => SetValue(PlotElementsProperty, value);
        }


        public void SaveAsPng(string fileName)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(this);
            //CroppedBitmap cropped = new CroppedBitmap(bmp, new Int32Rect(this.LeftX, Y, width, height));

            var encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(bmp));

            using (System.IO.Stream stm = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                encoder.Save(stm);
            }
        }


        private static void OnDataSourceChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var plot = source as Plot;
            if (plot != null)
            {
                if (e.NewValue != null)
                {
                    plot._dataSource = (IDataSource)e.NewValue;
                }

                foreach (var f in plot.PlotElements.OfType<PlotFigure>())
                {
                    f.AttacheDataSource(plot._dataSource);
                    plot.PlotCanvas.AddPlotFigure(f);
                }
            }
        }

        private static void OnLogicalRangeChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var plot = source as Plot;
            if( plot != null)
            {
                plot.PlotCanvas.SetLogicalRangeProvider((ILogicalRangeProvider)e.NewValue);
            }
        }

        private static void OnLogicalHeightChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var plot = source as Plot;
            if (plot != null)
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    plot.PlotCanvas.LogicalHeight = (double)e.NewValue;
                }
            }
        }

        private static void OnLogicalYChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var plot = source as Plot;
            if (plot != null)
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    plot.PlotCanvas.LogicalY = (double)e.NewValue;
                }
            }
        }

        private static void OnLogicalWidthChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var plot = source as Plot;
            if (plot != null)
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    plot.PlotCanvas.LogicalWidth = (double)e.NewValue;
                }
            }
        }

        private static void OnLogicalXChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var plot = source as Plot;
            if (plot != null)
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    plot.PlotCanvas.LogicalX = (double)e.NewValue;
                }
            }
        }

        private static void OnXspacingChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var plot = source as Plot;
            if (plot != null)
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    plot.PlotCanvas.Xspacing = (double)e.NewValue;
                }
            }
        }

        private static void OnTitleChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var plot = source as Plot;
            if (plot != null)
            {
                if (e.NewValue != null && e.NewValue != e.OldValue)
                {
                    plot.TitleControl.Text = (string)e.NewValue;
                }
            }
        }
    }
}
