using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System.Xml.Linq;
using QTool.Controls.Utilities;
using System.Threading;


namespace QTool.Controls
{
    [TemplatePart(Name = PART_MediaElement, Type = typeof(MediaElement))]
    [TemplatePart(Name = PART_PlayPosition, Type = typeof(Slider))]
    public class VideoPlayer : Control
    {
        private const string PART_MediaElement = nameof(PART_MediaElement);
        private const string PART_PlayPosition = nameof(PART_PlayPosition);
        static VideoPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoPlayer), new FrameworkPropertyMetadata(typeof(VideoPlayer)));
        }

        private readonly DispatcherTimer _timer;
        public VideoPlayer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private TimeSpan _lastPosition;
        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_mediaElemant != null)
            {
                var position = Position;
                if (_lastPosition != position)
                {
                    _lastPosition = position;
                    _mediaElemant.Position = _lastPosition;
                }
                else
                {
                    _lastPosition = TimeSpan.FromTicks(_mediaElemant.Position.Ticks / 10000000 * 10000000);
                    SetCurrentValue(PositionProperty, _lastPosition);
                }
            }
            else
            {
                _lastPosition = TimeSpan.Zero;
                SetCurrentValue(PositionProperty, _lastPosition);
            }
        }



        private MediaElement _mediaElemant;
        private Slider _playPosition;
        public override void OnApplyTemplate()
        {
            if (_mediaElemant != null)
            {
                _mediaElemant.MediaOpened -= _mediaElemant_MediaOpened;
                _mediaElemant.MediaFailed -= _mediaElemant_MediaFailed;
            }
            base.OnApplyTemplate();

            _mediaElemant = GetTemplateChild(PART_MediaElement) as MediaElement;
            _playPosition = GetTemplateChild(PART_PlayPosition) as Slider;

            if (_mediaElemant != null)
            {
                _mediaElemant.MediaOpened += _mediaElemant_MediaOpened;
                _mediaElemant.MediaFailed += _mediaElemant_MediaFailed;
                _mediaElemant.LoadedBehavior = MediaState.Manual;
                SetIsPause(IsPause);
                SetIsMuted(IsMuted);
                SetVolume(Volume);

                BindSource();
            }

            if (_playPosition != null)
            {
                _playPosition.IsMoveToPointEnabled = true;
            }
        }

        private void _mediaElemant_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            SetCurrentValue(ErrorMsgProperty, e.ErrorException.Message);
        }

        private void _mediaElemant_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (_mediaElemant != null)
            {
                if (_mediaElemant.NaturalDuration.HasTimeSpan)
                    SetCurrentValue(NaturalDurationProperty, _mediaElemant.NaturalDuration.TimeSpan);
            }
            SetCurrentValue(ErrorMsgProperty, null);
        }

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(VideoPlayer), new PropertyMetadata(null, OnSourceChanged));


        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoPlayer)d).OnSourceChanged(e.NewValue as Uri, e.OldValue as Uri);
        }

        private int _sourceIndex = 0;
        private Uri _localFile;
        protected virtual void OnSourceChanged(Uri newValue, Uri oldValue)
        {
            if (newValue != null)
            {
                if (newValue.IsFile)
                {
                    _localFile = newValue;
                }
                else
                {
                    var sourceIndex = Interlocked.Increment(ref _sourceIndex);
                    SetCurrentValue(IsLoadingProperty, true);
                    var task = Downloader.DownloadFile(newValue.OriginalString);
                    task.ContinueWith((result) =>
                    {
                        if (sourceIndex == _sourceIndex)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                SetCurrentValue(IsLoadingProperty, false);
                                if (result.Exception == null)
                                {
                                    _localFile = new Uri(result.Result);
                                    BindSource();
                                }
                                else
                                {
                                    SetCurrentValue(ErrorMsgProperty, result.Exception.Message);
                                }
                            });
                        }
                    });
                }
            }
            else
            {
                _localFile = null;
            }

            BindSource();
        }

        private void BindSource()
        {
            SetCurrentValue(ErrorMsgProperty, null);
            if (_mediaElemant != null)
            {
                _mediaElemant.Source = _localFile;
            }
        }

        public TimeSpan Position
        {
            get { return (TimeSpan)GetValue(PositionProperty); }
            private set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(TimeSpan), typeof(VideoPlayer), new PropertyMetadata(TimeSpan.Zero));

        public TimeSpan NaturalDuration
        {
            get { return (TimeSpan)GetValue(NaturalDurationProperty); }
            private set { SetValue(NaturalDurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NaturalDuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NaturalDurationProperty =
            DependencyProperty.Register("NaturalDuration", typeof(TimeSpan), typeof(VideoPlayer), new PropertyMetadata(TimeSpan.Zero));


        public bool IsPause
        {
            get { return (bool)GetValue(IsPauseProperty); }
            set { SetValue(IsPauseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPause.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPauseProperty =
            DependencyProperty.Register("IsPause", typeof(bool), typeof(VideoPlayer), new PropertyMetadata(false, OnIsPauseChanged));

        private static void OnIsPauseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoPlayer)d).SetIsPause((bool)e.NewValue);
        }

        protected virtual void SetIsPause(bool isPause)
        {
            if (_mediaElemant != null)
            {
                if (isPause)
                {
                    _mediaElemant.Pause();
                }
                else
                {
                    _mediaElemant.Play();
                }
            }
        }


        public bool IsMuted
        {
            get { return (bool)GetValue(IsMutedProperty); }
            set { SetValue(IsMutedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMute.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMutedProperty =
            DependencyProperty.Register("IsMuted", typeof(bool), typeof(VideoPlayer), new PropertyMetadata(false, OnIsMutedChanged));

        private static void OnIsMutedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoPlayer)d).SetIsMuted((bool)e.NewValue);
        }

        protected virtual void SetIsMuted(bool isMuted)
        {
            if (_mediaElemant != null)
            {
                _mediaElemant.IsMuted = isMuted;
            }
        }


        /// <summary>
        /// 声音大小，0-100
        /// </summary>
        public int Volume
        {
            get { return (int)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volume.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(int), typeof(VideoPlayer), new PropertyMetadata(50, OnVolumeChanged));

        private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoPlayer)d).OnVolumeChanged((int)e.NewValue, (int)e.OldValue);
        }

        private void OnVolumeChanged(int newValue, int oldValue)
        {
            if (IsMuted)
                SetCurrentValue(IsMutedProperty, false);

            SetVolume(newValue);
        }

        protected virtual void SetVolume(int volume)
        {
            if (_mediaElemant != null)
            {
                _mediaElemant.Volume = volume / 100d;
            }
        }



        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(VideoPlayer), new PropertyMetadata(false));




        public string ErrorMsg
        {
            get { return (string)GetValue(ErrorMsgProperty); }
        }

        // Using a DependencyProperty as the backing store for ErrorMsg.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorMsgProperty =
            DependencyProperty.Register("ErrorMsg", typeof(string), typeof(VideoPlayer), new PropertyMetadata(null, OnErrorMsgChanged));

        private static void OnErrorMsgChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoPlayer)d).OnErrorMsgChanged(e.NewValue as string, e.OldValue as string);
        }

        protected virtual void OnErrorMsgChanged(string newValue, string odlValue)
        {
            SetCurrentValue(HasErrorProperty, !string.IsNullOrEmpty(newValue));
        }



        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
        }

        // Using a DependencyProperty as the backing store for HasError.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register("HasError", typeof(bool), typeof(VideoPlayer), new PropertyMetadata(false));
    }
}
