using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class AsyncHelper
    {
        private static void Exec<TResult>(Delegate func, Action<AsyncExecuteResult<TResult>> callback, object[] args)
        {
            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (sender, e) =>
            {
                e.Result = (TResult)func.DynamicInvoke(args);
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                worker.Dispose();
                if (e.Error == null)
                {
                    callback?.Invoke(new AsyncExecuteResult<TResult>((TResult)e.Result));
                }
                else
                {
                    var error = e.Error;
                    if (error.InnerException != null && error.GetType() == typeof(TargetInvocationException))
                    {
                        error = error.InnerException;
                    }

                    if (callback == null)
                    {
                        QMessageHelper.Show(error);
                    }
                    else
                    {
                        callback.Invoke(new AsyncExecuteResult<TResult>(error));
                    }
                }
            };

            worker.RunWorkerAsync();
        }

        private static void Exec(Delegate action, Action<AsyncExecuteResult> callback, object[] args)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (sender, e) =>
            {
                action.DynamicInvoke(args);
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                worker.Dispose();
                if (e.Error == null)
                {
                    callback?.Invoke(new AsyncExecuteResult());
                }
                else
                {
                    var error = e.Error;
                    while (error.InnerException != null)
                    {
                        error = error.InnerException;
                    }

                    if (callback == null)
                    {
                        QMessageHelper.Show(error);
                    }
                    else
                    {
                        callback.Invoke(new AsyncExecuteResult(error));
                    }
                }
            };

            worker.RunWorkerAsync();
        }


        /// <summary>
        /// 异步执行方法
        /// </summary>
        /// <param name="action">执行委托</param>
        /// <param name="callbcak">回调委托</param>
        public static void Exec(Action action, Action<AsyncExecuteResult> callbcak = null)
        {
            Exec(action, callbcak, new object[0]);
        }



        /// <summary>
        /// 异步执行方法
        /// </summary>
        /// <typeparam name="T1">入参1</typeparam>
        /// <param name="action">执行委托</param>
        /// <param name="callbcak">回调委托</param>
        /// <param name="t1">入参1</param>
        public static void Exec<T1>(Action<T1> action, Action<AsyncExecuteResult> callbcak, T1 t1)
        {
            Exec(action, callbcak, new object[] { t1 });
        }
        public static void Exec<T1, T2>(Action<T1, T2> action, Action<AsyncExecuteResult> callbcak, T1 t1, T2 t2)
        {
            Exec(action, callbcak, new object[] { t1, t2 });
        }


        /// <summary>
        /// 异步执行方法
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="func">执行委托</param>
        /// <param name="callbcak">回调委托</param>
        public static void Exec<TResult>(Func<TResult> func, Action<AsyncExecuteResult<TResult>> callbcak)
        {
            Exec(func, callbcak, new object[] { });
        }


        /// <summary>
        /// 异步执行方法
        /// </summary>
        /// <typeparam name="T1">入参1</typeparam>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="func">执行委托</param>
        /// <param name="callbcak">回调委托</param>
        /// <param name="arg1">入参1</param>
        public static void Exec<T1, TResult>(Func<T1, TResult> func, Action<AsyncExecuteResult<TResult>> callbcak, T1 arg1)
        {
            Exec(func, callbcak, new object[] { arg1 });
        }

        /// <summary>
        /// 异步执行方法
        /// </summary>
        /// <typeparam name="T1">入参1</typeparam>
        /// <typeparam name="T2">入参2</typeparam>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="func">执行委托</param>
        /// <param name="callbcak">回调委托</param>
        /// <param name="arg1">入参1</param>
        /// <param name="arg2">入参2</param>
        public static void Exec<T1, T2, TResult>(Func<T1, T2, TResult> func, Action<AsyncExecuteResult<TResult>> callbcak, T1 arg1, T2 arg2)
        {
            Exec(func, callbcak, new object[] { arg1, arg2 });
        }

        public static void Exec<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, Action<AsyncExecuteResult<TResult>> callbcak, T1 arg1, T2 arg2, T3 arg3)
        {
            Exec(func, callbcak, new object[] { arg1, arg2, arg3 });
        }

        public static void Exec<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, Action<AsyncExecuteResult<TResult>> callbcak, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Exec(func, callbcak, new object[] { arg1, arg2, arg3, arg4 });
        }
    }


    public class AsyncExecuteResult
    {

        public AsyncExecuteResult(Exception error = null)
        {
            Error = error;
        }

        public Exception Error { get; }

        public bool HasError
        {
            get
            {
                return Error != null;
            }
        }
    }

    public class AsyncExecuteResult<TResult> : AsyncExecuteResult
    {
        public AsyncExecuteResult(Exception error)
            : base(error)
        {

        }

        public AsyncExecuteResult(TResult result)
        {
            _result = result;
        }

        private readonly TResult _result;

        public TResult Result
        {
            get
            {
                if (HasError)
                {
                    throw Error;
                }
                return _result;
            }
        }
    }
}
