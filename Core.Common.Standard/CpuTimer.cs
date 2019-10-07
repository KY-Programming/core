//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace KY.Core
//{
//    public class CpuTimer
//    {
//        private static readonly Runner runner = new Runner();
        
//        private static readonly Stopwatch stopwatch = new Stopwatch();

//        public static long Sleep(int ticks)
//        {
//            long turns = 0;
//            //runner.Run(ticks).WaitOne();
//            stopwatch.Reset();
//            stopwatch.Start();
//            while (stopwatch.ElapsedTicks < ticks)
//            {
//                turns++;
//            }
//            stopwatch.Stop();
//            return turns;
//        }

//        private class Runner
//        {
//            private readonly Stopwatch stopwatch = new Stopwatch();
//            private Task task;
//            private readonly List<Tuple<long, ManualResetEvent>> handles = new List<Tuple<long, ManualResetEvent>>();

//            public WaitHandle Run(int ticks)
//            {
//                ManualResetEvent waitHandle = new ManualResetEvent(false);
//                lock (this.handles)
//                {
//                    this.handles.Add(Tuple.Create(this.stopwatch.ElapsedTicks + ticks, waitHandle));
//                }
//                lock (this)
//                {
//                    if (this.task == null)
//                    {
//                        this.task = Task.Factory.StartNew(this.Measure);
//                    }
//                    this.stopwatch.Start();
//                }
//                return waitHandle;
//            }

//            private void Measure()
//            {
//                while (true)
//                {
//                    List<Tuple<long, ManualResetEvent>> temporaryHandles;
//                    lock (this.handles)
//                    {
//                        if (this.handles.Count == 0)
//                        {
//                            return;
//                        }
//                        temporaryHandles = this.handles.ToList();
//                    }
//                    foreach (Tuple<long, ManualResetEvent> tuple in temporaryHandles)
//                    {
//                        if (tuple.Item1 > this.stopwatch.ElapsedTicks)
//                        {
//                            continue;
//                        }
//                        lock (this.handles)
//                        {
//                            this.handles.Remove(tuple);
//                            tuple.Item2.Set();
//                        }
//                    }
//                }
//                lock (this)
//                {
//                    this.task = null;
//                    this.stopwatch.Stop();
//                    this.stopwatch.Reset();
//                }
//            }
//        }
//    }
//}