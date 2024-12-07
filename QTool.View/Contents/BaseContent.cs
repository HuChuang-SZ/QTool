using QTool.Controls;
using QTool.View.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace QTool.View.Contents
{
    public abstract class BaseContent : IBaseContent
    {
        public const string DefaultHeaderKey = "DefaultHeader";

        protected BaseContent(QModule module)
        {
            Module = module;
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

        public virtual string HeaderTemplate { get { return DefaultHeaderKey; } }

        public QModule Module { get; }

        #region IsWaiting Property
        private bool _isWaiting;
        /// <summary>
        /// 是否等着中
        /// </summary>
        public bool IsWaiting
        {
            get
            {
                return _isWaiting;
            }
            set
            {
                if (_isWaiting != value)
                {
                    _isWaiting = value;
                    OnPropertyChanged(nameof(IsWaiting));
                }
            }
        }
        #endregion IsWaiting Property

        #region WaitMsg Property
        private string _waitMsg;

        /// <summary>
        /// 等待消息
        /// </summary>
        public string WaitMsg
        {
            get
            {
                return _waitMsg;
            }
            set
            {
                if (_waitMsg != value)
                {
                    _waitMsg = value;
                    OnPropertyChanged(nameof(WaitMsg));
                }
            }
        }
        #endregion WaitMsg Property

        private readonly static Dictionary<QModule, IBaseContent> _contentMaps = new Dictionary<QModule, IBaseContent>();
        public static IBaseContent Create(QModuleInfo moduleInfo, int shopId)
        {
            if (Application.Current.CheckAccess())
            {
                if (moduleInfo.Module == QModule.AeBrowser)
                {
                    if (moduleInfo.Args is string uri)
                    {
                        return AeBrowserContent.Create(shopId, uri);
                    }
                }
                else if (moduleInfo.Module == QModule.TemuBrowser)
                {
                    if (moduleInfo.Args is string uri)
                    {
                        return TemuBrowserContent.Create(shopId, uri);
                    }
                }
                else
                {
                    return Create(moduleInfo.Module);
                }

                throw new QException($"“{moduleInfo.Title}”参数无效。");
            }
            else
            {
                return (IBaseContent)Application.Current.Dispatcher.Invoke(new Func<QModuleInfo, int, IBaseContent>(Create), moduleInfo, shopId);
            }
        }

        public static IBaseContent Create(QModule module)
        {
            if (module == QModule.AeBrowser)
            {
                throw new ArgumentOutOfRangeException(nameof(module));
            }

            lock (_contentMaps)
            {
                if (!_contentMaps.TryGetValue(module, out IBaseContent content))
                {
                    try
                    {
                        var type = Type.GetType($"QTool.View.Contents.{module}Content", true);
                        content = (IBaseContent)Activator.CreateInstance(type);
                        _contentMaps[module] = content;
                    }
                    catch (Exception ex)
                    {
                        QMessageBox.Show(ex);
                    }
                }
                return content;
            }
        }

        static BaseContent()
        {
            //Create(QModule.AeImSmart);
        }
    }
}