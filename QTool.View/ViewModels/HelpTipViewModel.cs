using QTool.Controls;
using QTool.View.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace QTool.View.ViewModels
{
    public class HelpTipViewModel : INotifyPropertyChanged
    {
        public HelpTipViewModel(Window parentWindow, HelpTipStep[] steps)
        {
            Steps = new ReadOnlyCollection<HelpTipStep>(steps);
            ParentWindow = parentWindow;
            WindowRect = new Rect(parentWindow.Left, parentWindow.Top, parentWindow.Width, parentWindow.Height);
            CurrentIndex = 1;
        }

        public ReadOnlyCollection<HelpTipStep> Steps { get; }

        public Window ParentWindow { get; }

        #region CurrentIndex Property
        private int _currentIndex;
        /// <summary>
        /// 当前位置
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }
            private set
            {
                if (_currentIndex != value)
                {
                    _currentIndex = value;
                    OnPropertyChanged(nameof(CurrentIndex));
                    CurrentStep = Steps[value - 1];
                }
            }
        }
        #endregion CurrentIndex Property

        #region CurrentStep Property
        private HelpTipStep _currentStep;
        /// <summary>
        /// 当前位置
        /// </summary>
        public HelpTipStep CurrentStep
        {
            get
            {
                return _currentStep;
            }
            private set
            {
                if (_currentStep != value)
                {
                    _currentStep = value;
                    OnPropertyChanged(nameof(CurrentStep));
                    OnPropertyChanged(nameof(CanPrev));
                    OnPropertyChanged(nameof(CanNext));
                    OnPropertyChanged(nameof(NextText));
                    Calc(CurrentStep);
                }
            }
        }
        #endregion CurrentStep Property

        private void Calc(HelpTipStep step)
        {
            if (string.IsNullOrEmpty(step.ElementName))
            {
                HighlightRect = new Rect(0, 0, 0, 0);
                TipLocation = new Point((WindowRect.Width - TipSize.Width) / 2, (WindowRect.Height - TipSize.Height) / 2);
            }
            else
            {
                HighlightRect = ParentWindow.GetRect(step.ElementName);

                var positions = new Point[]
                {
                    new Point(HighlightRect.Left, HighlightRect.Bottom + 5),
                    new Point(HighlightRect.Right - TipSize.Width,HighlightRect.Bottom + 5),
                    new Point(HighlightRect.Right + 5,HighlightRect.Top),
                    new Point(HighlightRect.Right + 5,HighlightRect.Bottom - TipSize.Height),
                    new Point(HighlightRect.Left, HighlightRect.Top - TipSize.Height - 5),
                    new Point(HighlightRect.Left - 5 - TipSize.Width,HighlightRect.Top),
                    new Point(HighlightRect.Left + (HighlightRect.Width - TipSize.Width) / 2,HighlightRect.Top + (HighlightRect.Height - TipSize.Height) / 2),
                };

                var rect = new Rect(new Point(0, 0), WindowRect.Size);
                foreach (var position in positions)
                {
                    if (rect.Contains(new Rect(position, TipSize)))
                    {
                        TipLocation = position;
                        break;
                    }
                }
            }
        }

        public bool CanNext { get { return CurrentStep != Steps[Steps.Count - 1]; } }

        public bool CanPrev { get { return CurrentStep != Steps[0]; } }

        public bool Prev()
        {
            if (CanPrev)
            {
                CurrentIndex -= 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Next()
        {
            if (CanNext)
            {
                CurrentIndex += 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public string NextText
        {
            get
            {
                return CanNext ? "下一步(_N)" : "完成(_F)";
            }
        }

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

        #region WindowRect Property
        private Rect _windowRect;
        /// <summary>
        /// 遮盖层
        /// </summary>
        public Rect WindowRect
        {
            get
            {
                return _windowRect;
            }
            set
            {
                if (_windowRect != value)
                {
                    _windowRect = value;
                    OnPropertyChanged(nameof(WindowRect));
                }
            }
        }
        #endregion WindowRect Property

        #region HighlightRect Property
        private Rect _highlightRect;
        /// <summary>
        /// 可见层
        /// </summary>
        public Rect HighlightRect
        {
            get
            {
                return _highlightRect;
            }
            set
            {
                if (_highlightRect != value)
                {
                    _highlightRect = value;
                    OnPropertyChanged(nameof(HighlightRect));
                }
            }
        }
        #endregion HighlightRect Property

        public Size TipSize { get; private set; } = new Size(365, 200);

        #region TipLocation Property
        private Point _tipPosition;
        /// <summary>
        /// 提示层
        /// </summary>
        public Point TipLocation
        {
            get
            {
                return _tipPosition;
            }
            private set
            {
                if (_tipPosition != value)
                {
                    _tipPosition = value;
                    OnPropertyChanged(nameof(TipLocation));
                }
            }
        }
        #endregion TipLocation Property
    }
}
