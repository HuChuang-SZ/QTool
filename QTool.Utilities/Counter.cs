using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QTool
{
    public class Counter
    {
        private int _count;

        public int Count { get { return _count; } }

        public void Begin()
        {
            lock (this)
            {
                while (_count >= _threadCount)
                {
                    Task.Delay(100).Wait();
                }
                Interlocked.Increment(ref _count);
            }
        }

        public void End()
        {
            Interlocked.Decrement(ref _count);
        }

        private readonly int _threadCount;
        public Counter(int threadCount = 3)
        {
            _threadCount = threadCount;
        }
    }
}
