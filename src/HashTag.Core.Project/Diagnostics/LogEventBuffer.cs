using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HashTag.Diagnostics
{
    public class LogEventBuffer : IDisposable
    {
        private ReaderWriterLockSlim _bufferLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private ConcurrentQueue<LogEvent> _buffer = new ConcurrentQueue<LogEvent>();

        private Timer _bufferTimer;

        /// <summary>
        /// Write a block of messages to store. NOTE: Fired on separate thread
        /// </summary>
        public Action<List<LogEvent>> PersistMessageHandler;

        private void onTimer(object state)
        {
            try
            {
                StopTimer();
                if (_buffer.Count == 0) return;

                _bufferLock.EnterWriteLock();
                var task = Task.Factory.StartNew(() =>
                {
                    PersistMessagesToLoggingStore();
                });
                task.Wait(CoreConfig.Log.MessageBufferWriteTimeoutMs);
            }
            finally
            {
                if (_bufferLock.IsWriteLockHeld)
                {
                    _bufferLock.ExitWriteLock();
                }
                StartTimer();
            }
        }

        public void PersistMessagesToLoggingStore()
        {
            if (PersistMessageHandler == null) return;
            if (_buffer.Count == 0) return;

            var listOfMessagesToPersist = new List<LogEvent>();
            LogEvent msg;
            while (_buffer.TryDequeue(out msg))
            {
                listOfMessagesToPersist.Add(msg);
            }
            if (listOfMessagesToPersist.Count == 0) return;
            PersistMessageHandler(listOfMessagesToPersist);
        }

        public void StartTimer()
        {
            _bufferTimer.Change(CoreConfig.Log.MessageBufferIntervalMs, CoreConfig.Log.MessageBufferIntervalMs);
        }

        public void StopTimer()
        {
            _bufferTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public LogEventBuffer()
        {
            _bufferTimer = new Timer(onTimer, null, Timeout.Infinite, Timeout.Infinite);
            StartTimer();
        }

        public void SubmitMessage(LogEvent msg)
        {
            _buffer.Enqueue(msg);
            
            if (_buffer.Count > CoreConfig.Log.MessageBufferCacheSize)
            {
                onTimer(null);
            }
        }

        public void Dispose(bool isDisposing)
        {
            if (isDisposing == true)
            {
                try
                {
                    StopTimer();
                    PersistMessagesToLoggingStore();
                }
                catch (Exception ex)
                {
                    Log.Internal.Write(ex);
                }
            }
            GC.SuppressFinalize(this);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        ~LogEventBuffer()
        {
            Dispose(false);
        }
    }
}
