using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Threading;


namespace SilgiFU.module
{
    class StopWatch
    {
        private DispatcherTimer timer;
        private Stopwatch sw = new Stopwatch();
        
        public StopWatch()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10);
        }

        public void Start()
        {
            sw.Start();
            timer.Start();
        }

        public void Stop()
        {
            sw.Stop();
            timer.Stop();
        }

        public void Reset()
        {
            sw.Reset();
        }

        public TimeSpan getElapsed()
        {
            TimeSpan ts = sw.Elapsed;
            return ts;
        }

        public void AddCallback(EventHandler evh)
        {
            timer.Tick += evh;
        }

        public void RemoveCallback(EventHandler evh)
        {
            timer.Tick -= evh;
        }
    }
}
