using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace QTool.Controls
{
    public class AppContext : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer;
        private AppContext()
        {
            NowTime = DateTime.Now;
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            NowTime = DateTime.Now;
        }

        public static AppContext Current { get; private set; } = new AppContext();

        #region INotifyPropertyChanged
        [NonSerialized]
        private PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged

        #region NowTime Property
        private DateTime _nowTime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime NowTime
        {
            get
            {
                return _nowTime;
            }
            set
            {
                if (_nowTime != value)
                {
                    _nowTime = value;
                    OnPropertyChanged(nameof(NowTime));
                }
            }
        }
        #endregion NowTime Property

    }
}
