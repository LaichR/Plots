using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PlotLib.Interface;


namespace PlotLib.DataSources
{
    public class DynamicDataSource : IDataSource, ILogicalRangeProvider, IEnumerator<Point>, IEnumerator
    {
        double _startX;
        double _logicalX;
        double _locicalY;
        double _logicalWidth;
        double _logicalHeight;

        ConcurrentQueue<double> _buffer = new ConcurrentQueue<double>();
        double _deltaX;
        Point _current;

        public DynamicDataSource()
        {
            _startX = 0;
            _logicalX = 0;
            _locicalY = 0;
            _logicalHeight = 100;
            _logicalWidth = 100;
            _deltaX = 1;
        }

        public DynamicDataSource(double startX, double startY, double deltaX, double logicalWidth, double logicalHeight)
        {
            _startX = startX;
            _logicalX = _startX;
            _locicalY = startY;
            _logicalHeight = logicalHeight;
            _logicalWidth = logicalWidth;
            _deltaX = deltaX;
            UnitX = "";
            UnitY = "";
        }

        public void PutData(double data)
        {
            
            _buffer.Enqueue(data);
            //if ( _buffer.Count > 1 )
            {
                NotifyDataAvailable();
            }
        }

        public int AvailablePoints => _buffer.Count();

        public string UnitX { get; set; }

        public string UnitY { get; set; }

        public Point Current => _current;

        object IEnumerator.Current => _current;

        public double Xspacing
        {
            get => _deltaX;
            set => _deltaX = value;
        }

        public double LogicalX
        {
            get => _logicalX;
            set { _logicalX = value; NotifyRangeChanged(); }
        }

        public double LogicalWidth
        {
            get => _logicalWidth;
            set { _logicalWidth = value; NotifyRangeChanged(); }
        }


        public double LogicalY
        {
            get => _locicalY;
            set { _locicalY = value; NotifyRangeChanged(); }
        }

        public double LogicalHeight
        {
            get => _logicalHeight;
            set { _logicalHeight = value; NotifyRangeChanged(); }
        }

        public event EventHandler DataAvailable;

        public event EventHandler RangeChanged;

        public IEnumerator<Point> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public void Dispose(){}

        public bool MoveNext()
        {
            var hasMore = !_buffer.IsEmpty;
            if( hasMore )
            {
                _current.X = _startX;
                if( _buffer.TryDequeue( out double value ) )
                {
                    _current.Y = value;
                    _startX += _deltaX;
                    return true;
                }
            }
            else
            {
                if( _startX-_logicalX > _logicalWidth)
                {
                    _logicalX = _startX - _logicalWidth;
                    NotifyRangeChanged();
                }
            }
            return false;
        }

        public void Reset(){}

        void NotifyDataAvailable()
        {
            if (DataAvailable != null)
            {
                DataAvailable(this, EventArgs.Empty);
            }
        }

        void NotifyRangeChanged()
        {
            if (RangeChanged != null)
            {
                RangeChanged(this, EventArgs.Empty);
            }
        }
    }
}
