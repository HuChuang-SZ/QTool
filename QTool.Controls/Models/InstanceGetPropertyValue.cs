
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTool.Controls.Models
{
    public class InstanceGetPropertyValue
    {

        public Type Type { get; }

        public DateTime LastAccessTime { get; private set; }

        private readonly ConcurrentDictionary<string, Func<object, object>> _map = new ConcurrentDictionary<string, Func<object, object>>();

        public InstanceGetPropertyValue(Type type)
        {
            Type = type;
            LastAccessTime = DateTime.Now;
        }

        public bool TryGetValue(string propertyName, out Func<object, object> func)
        {
            LastAccessTime = DateTime.Now;
            return _map.TryGetValue(propertyName, out func);
        }

        public Func<object, object> GetOrAdd(string propertyName, Func<object, object> func)
        {
            return _map.GetOrAdd(propertyName, func);
        }

        public bool CanRemove()
        {
            return (DateTime.Now - LastAccessTime).TotalMinutes > ExpirationMinutes;
        }


        private readonly static ConcurrentDictionary<Type, InstanceGetPropertyValue> _instanceMap = new ConcurrentDictionary<Type, InstanceGetPropertyValue>();

        public static Func<object, object> GetOrCreate(Type type, string propertyName)
        {
            InstanceGetPropertyValue propertyMap;
            lock (_instanceMap)
            {
                if (!_instanceMap.TryGetValue(type, out propertyMap))
                {
                    propertyMap = _instanceMap.GetOrAdd(type, new InstanceGetPropertyValue(type));
                }
            }

            Func<object, object> func;
            lock (propertyMap)
            {
                if (!propertyMap.TryGetValue(propertyName, out func))
                {
                    func = propertyMap.GetOrAdd(propertyName, ReflectHelper.GetPropertyValueFunc(type, propertyName));
                }
            }
            return func;
        }

        private const int ExpirationMinutes = 10;

        static InstanceGetPropertyValue()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private static void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            worker.DoWork -= Worker_DoWork;
            worker.RunWorkerCompleted -= Worker_RunWorkerCompleted;
            worker.Dispose();
        }

        private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            do
            {
                index++;
                if (index % 100 == 0)
                {
                    var items = _instanceMap.Values.ToArray();
                    foreach (var propertyMap in items)
                    {
                        if (propertyMap.CanRemove())
                        {
                            _instanceMap.TryRemove(propertyMap.Type, out InstanceGetPropertyValue map);
                        }
                    }
                }
                Task.Delay(100).Wait();
            }
            while (!AppHelper.IsExit);
        }
    }
}
