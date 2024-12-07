using QTool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QTool.Controls
{
    public abstract class ExecuteSchedule : INotifyPropertyChanged
    {
        public ExecuteSchedule(string title, ICommand finish, bool isRetry = false, int[] retryIntervalList = null)
        {
            Title = title;
            Finish = finish ?? throw new ArgumentNullException(nameof(finish));
            ExecuteItems = new ObservableCollection<IExecuteScheduleItem>();
            IsRetry = isRetry;
            RetryIntervalList = retryIntervalList ?? new int[] { -1, 0, 1, 3, 5, 10, 15, 30, 60 };
        }

        public string Title { get; }

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

        #region ExecuteMsg Property
        private string _executeMsg;
        /// <summary>
        /// 
        /// </summary>
        public string ExecuteMsg
        {
            get
            {
                return _executeMsg;
            }
            set
            {
                if (_executeMsg != value)
                {
                    _executeMsg = value;
                    OnPropertyChanged(nameof(ExecuteMsg));
                }
            }
        }
        #endregion ExecuteMsg Property

        public ObservableCollection<IExecuteScheduleItem> ExecuteItems { get; }

        #region IsCompleted Property
        private bool _isCompleted = false;
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                }
            }
        }
        #endregion IsCompleted Property

        #region RetryInterval Property
        private int _retryInterval = 5;
        /// <summary>
        /// 重试间隔
        /// </summary>
        public int RetryInterval
        {
            get
            {
                return _retryInterval;
            }
            set
            {
                if (_retryInterval != value)
                {
                    _retryInterval = value;
                    OnPropertyChanged(nameof(RetryInterval));
                    OnPropertyChanged(nameof(CanRetry));
                }
            }
        }
        public bool CanRetry
        {
            get
            {
                return IsRetry && RetryInterval >= 0;
            }
        }
        #endregion RetryInterval Property

        public bool IsRetry { get; }

        public object RetryIntervalList { get; }

        public void SetProgress(IExecuteScheduleItem item)
        {
            if (Application.Current == null)
                return;

            if (Application.Current.CheckAccess())
            {
                //if (ExecuteItems.Contains(item))
                //{
                //    ExecuteItems.Remove(item);
                //}
                //ExecuteItems.Insert(0, item);

                if (!ExecuteItems.Contains(item))
                {
                    ExecuteItems.Add(item);
                }
                SetExecuteMsg();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetProgress(item);
                });
            }
        }

        protected abstract void SetExecuteMsg();

        public ICommand Finish { get; }
    }


    public class ExecuteSchedule<T> : ExecuteSchedule
        where T : IExecuteScheduleItem
    {
        public ExecuteSchedule(IList<T> items, string title, ICommand finish, bool isRetry = false, int[] retryIntervalList = null)
            : base(title, finish, isRetry, retryIntervalList)
        {
            if (items == null || items.Count == 0)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Items = new ReadOnlyCollection<T>(items);
        }
        
        public ReadOnlyCollection<T> Items { get; }


        protected override void SetExecuteMsg()
        {
            ExecuteMsg = $"正在{Title}，{GetProgressMsg()}";
        }

        public string GetProgressMsg()
        {
            var counts = Items.GroupBy(i => i.Status).ToDictionary(g => g.Key, g => g.Count());
            return $"共 {Items.Count} 个，成功 {counts.GetValue(ExecuteStatus.Success, 0)} 个，失败 {counts.GetValue(ExecuteStatus.Error, 0)} 个，跳过 {counts.GetValue(ExecuteStatus.Skip, 0)} 个。";
        }

        public void BatchAction(Func<T, Task<bool>> action)
        {
            AsyncHelper.Exec(() =>
            {
                ExecuteMsg = $"正在{Title}，{GetProgressMsg()}";
                var tasks = new Task<bool>[Items.Count];
                for (int i = 0; i < Items.Count; i++)
                {
                    tasks[i] = ExecuteAction(action, Items[i]);
                }

                foreach (var task in tasks)
                {
                    task.Wait();
                }

            }, result =>
            {
                if (result.HasError)
                {
                    ExecuteMsg = $"执行中断，中断原因：{result.Error.GetInnerException().Message}。\r\n{GetProgressMsg()}";
                }
                else
                {
                    ExecuteMsg = "执行完成，" + GetProgressMsg();
                }

                IsCompleted = true;
            });
        }

        private async Task<bool> ExecuteAction(Func<T, Task<bool>> action, T item)
        {
            try
            {
                SetProgress(item);

                item.Status = ExecuteStatus.Executing;
                item.Message = $"正在{Title}...";
#if DEBUG
                Task.Delay(1000).Wait();
#endif
                return await action(item);
            }
            catch (Exception ex)
            {
                item.Status = ExecuteStatus.Error;
                if (CanRetry)
                {
                    SetProgress(item);

                    for (int i = RetryInterval; i > 0; i--)
                    {
                        i = Math.Min(i, RetryInterval);
                        if (!CanRetry)
                        {
                            item.Message = $"{ex.GetInnerException().Message}。";
                            return false;
                        }
                        else
                        {
                            item.Message = $"{ex.GetInnerException().Message}。{i} 秒后自动重试。";
                            await Task.Delay(1000);
                        }
                    }
                    return await ExecuteAction(action, item);
                }
                else
                {
                    item.Message = $"{ex.GetInnerException().Message}";
                    return false;
                }
            }
            finally
            {
                SetProgress(item);
            }
        }
    }
}
