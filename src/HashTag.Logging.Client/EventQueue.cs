using HashTag.Diagnostics.Models;
using HashTag.Logging.Client.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HashTag.Logging.Client
{
    public class EventQueue:IDisposable
    {
        private BlockingCollection<LogEvent> _queue = new BlockingCollection<LogEvent>();
        private CancellationTokenSource _cancelToken = null;
        private Task _queueReaderTask;
        private EventOptions _options;
        
        public EventQueue(EventOptions options)
        {
            _options = options;
        }
        public void Enqueue(LogEvent evt)
        {
            _queue.TryAdd(evt, Timeout.Infinite, _cancelToken.Token);
        }

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }
            _cancelToken = new CancellationTokenSource();
            _queueReaderTask = Task.Run(() =>
            {
                while (!_cancelToken.IsCancellationRequested)
                {
                    LogEvent eventToLog = null;
                    if (_queue.TryTake(out eventToLog, Timeout.Infinite, _cancelToken.Token))
                    {
                        if (eventToLog != null)
                        {
                            writeEvent(eventToLog);
                        }
                    }                    
                }
            }, _cancelToken.Token);
        }

        public bool IsRunning
        {
            get
            {
                return _queueReaderTask != null && _queueReaderTask.Status == TaskStatus.Running;
            }
        }

        public void Stop()
        {
            
            if (_queueReaderTask.Status == TaskStatus.Running)
            {
                _cancelToken.Cancel();
                _queueReaderTask.Wait();
                Flush();

                _cancelToken.Dispose();
                _cancelToken = null;
                _queueReaderTask.Dispose();
                _queueReaderTask = null;
            }            
        }

        public void Flush()
        {
            List<LogEvent> _listToFlush = new List<LogEvent>();
            while (_queue.Count == 0)
            {
                LogEvent evt = null;
                _queue.TryTake(out evt, 100);
                if (evt != null)
                {
                    _listToFlush.Add(evt);
                }
            }

            _listToFlush.ForEach(evt => writeEvent(evt));
        }

        private void writeEvent(LogEvent evt)
        {
            _options.EventStoreConnector.Submit(evt);
        }

        public void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Stop();
                _queue.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        ~EventQueue()
        {
            Dispose(false);
        }
    }
}
