#pragma warning disable 0168
/**
/// HashTag.Core Library
/// Copyright © 2010-2014
///
/// This module is Copyright © 2010-2014 Steve Powell
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the Microsoft Public License (Ms-PL)
/// 
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See theMicrosoft Public License (Ms-PL) License for more
/// details.
///
/// You should have received a copy of the Microsoft Public License (Ms-PL)
/// License along with this library; if not you may 
/// find it here: http://www.opensource.org/licenses/ms-pl.html
///
/// Steve Powell, hashtagdonet@gmail.com
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using HashTag.Configuration;
using HashTag.Diagnostics;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace HashTag.Collections
{
    /// <summary>
    /// Action to take when buffer determines it is time to flush it's items
    /// </summary>
    /// <typeparam name="T">Type of object that is being buffered</typeparam>
    /// <param name="bufferedItems">List of items that will be acted on by the buffers Flush action</param>
    public delegate void AsyncBufferAction<T>(List<T> bufferedItems);

    /// <summary>
    /// This class is responsible for collecting (buffering) objects before spooling them off to the
    /// action supplied in the constructor.  This is the first level of caching.  The action might also implement
    /// it's own caching and asynchronous behavior (e.g HashTag rolling log file writers).  
    /// This is the second level of caching.
    /// </summary>
    /// <typeparam name="T">Type of object that is being buffered</typeparam>
    public class AsyncBuffer<T> : IDisposable
    {

        //-----------------------------------------------------------
        //  Default values if not provided from .config file
        //-----------------------------------------------------------
        /// <summary>
        /// 300
        /// </summary>
        private const int MAX_PAGE_SIZE = 300;

        /// <summary>
        /// 1000 MS
        /// </summary>
        private const int BUFFER_SWEEP_MS = 1000;

        /// <summary>
        /// 1000 MS
        /// </summary>
        private const int CACHE_TIMEOUT_MS = 1000;

        //-----------------------------------------------------------
        //  working member variables with default values set
        //-----------------------------------------------------------

        /// <summary>
        /// Number of objects (called events in log4Net or LogEntry in Enterprise Library) which will be sent to the buffer's action
        /// Default: 300 Records
        /// </summary>
        private int _maxPageSize = MAX_PAGE_SIZE;

        /// <summary>
        /// Number of milliseconds appender will wait before executing any accumulated records (for partial pages)
        /// Default: 1000 Ms.  Config Settings: HashTag.Buffer.Sweep
        /// </summary>
        private int _bufferSweepMs = BUFFER_SWEEP_MS;

        /// <summary>
        /// Number of milliseconds the cache may lay dormant (without new records being added) until the buffer is automatically flushed
        /// Default: 1000 ms.  Config Settings: HashTag.Buffer.CacheTimeOutMs
        /// </summary>
        private int _cacheTimeOutMs = CACHE_TIMEOUT_MS;

        /// <summary>
        /// Number of objects (called events in log4Net) which will be acted on 
        /// Default: 300 Records.  Config Settings: HashTag.Buffer.MaxPageSize
        /// </summary>
        public int MaxPageSize
        {
            get { return _maxPageSize; }
            set { _maxPageSize = value; }
        }


        /// <summary>
        /// Number of milliseconds appender sleep between writting full buffers to storage
        /// Default: 1000 Ms. Config Settings: HashTag.Buffer.Sweep
        /// </summary>
        public int BufferSweepMs
        {
            get { return _bufferSweepMs; }
            set { _bufferSweepMs = value; }
        }

        /// <summary>
        /// Number of milliseconds the cache may lay dormant (without new records being added) until the buffer is automatically flushed
        /// Default 1000 ms, Config Settings: HashTag.Buffer.CacheTimeOutMs
        /// </summary>
        public int CacheTimeOutMs
        {
            get { return _cacheTimeOutMs; }
            set { _cacheTimeOutMs = value; }
        }

        private AsyncBufferAction<T> _bufferAction;

        //-----------------------------------------------------------
        // caching locks
        //-----------------------------------------------------------
        private ReaderWriterLockSlim _queueLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion); //locks buffer of queued up pages of objects messages

        //-----------------------------------------------------------
        // queued objects and list of pages
        //-----------------------------------------------------------
        //  private List<T> _currentPage = new List<T>(); //buffer to accumulate messages from object subsystem

        private ConcurrentQueue<T> _eventQueue = new ConcurrentQueue<T>(); //preallocate MaxPageSize*10. (3000 objects/second for default).  

        private DateTime _lastMessageReceived = DateTime.Now.AddDays(-365); //used for calculating cache timeouts

        private Timer _timer = null; //Used for sweeping the queue of messages


        /// <summary>
        /// Default constructor.  Start buffer sweep timer which will look through log queue and persist all pages of log messages is finds
        /// </summary>
        public AsyncBuffer()
        {
            _bufferSweepMs = ConfigManager.AppSetting<int>("Logging.Default.BufferSweepMs", BUFFER_SWEEP_MS);
            _maxPageSize = ConfigManager.AppSetting<int>("Logging.Default.MaxPageSize", MAX_PAGE_SIZE);
            _cacheTimeOutMs = ConfigManager.AppSetting<int>("Logging.Default.CacheTimeOutMs", CACHE_TIMEOUT_MS);

            _timer = new Timer(sweepQueue, null, _bufferSweepMs, _bufferSweepMs);
            _timer.Change(0, _bufferSweepMs);
            _bufferAction = OnBufferAction;
        }

        /// <summary>
        /// Event handler to execute when buffer needs to flush page(s) of objects
        /// </summary>
        /// <param name="objectItems"></param>
        protected void OnBufferAction(List<T> objectItems)
        {
            if (_bufferAction == null)
            {
                _bufferAction(objectItems);
            }
        }

        /// <summary>
        /// Constructor that supplies an explict Action />
        /// </summary>
        /// <param name="bufferAction"></param>
        public AsyncBuffer(AsyncBufferAction<T> bufferAction)
            : this()
        {
            _bufferAction = bufferAction;
        }


        /// <summary>
        /// Add an object to the list of queued up objects.  Only queuing is done 
        /// so the enqueing thread can return as quickly as possible.
        /// </summary>
        /// <param name="objectToBuffer">Entry that will be persisted</param>
        public void Submit(T objectToBuffer)
        {
            _lastMessageReceived = DateTime.Now; //mark _currentPage as not stale
            _eventQueue.Enqueue(objectToBuffer);
            
        }

        /// <summary>
        /// Called when application is shutting down (not when a logger goes out of scope)
        /// </summary>
        public virtual void OnClose()
        {
            Stop();
            //-------------------------------------------------------
            // put any partially filled buffer directly into persistent store
            //-------------------------------------------------------
            try
            {
                submitMessagesToAction();
            }
            catch (Exception ex)
            {
                // Log.InternalLog(ex, "Error when closing AsyncBuffer<{0}>",typeof(T).FullName);
            }
        }

        /// <summary>
        /// Empty internal buffer and submit records to buffer action
        /// </summary>
        public void Flush()
        {
            try
            {
                if (_queueLock.TryEnterUpgradeableReadLock(250) == false) return; //another process is already processing event queue so we don't need to do it
            }
            finally
            {
                if (_queueLock.IsUpgradeableReadLockHeld)
                {
                    _queueLock.ExitUpgradeableReadLock();
                }
            }

            Stop();
            submitMessagesToAction();
            Start();
        }
        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite); //suspend timer
        }
        public void Start()
        {
            _timer.Change(0, _bufferSweepMs);
        }

        /// <summary>
        /// Look over queue of object pages and create worker threads for each page.  Threads are responsible for
        /// persisting their page to persistent store
        /// </summary>
        /// <param name="context">not used.  Needed for compatibility with OnTimer event hander</param>
        private void sweepQueue(object context)
        {
            Stop();

            try
            {
                DateTime currentDate = DateTime.Now;
                if (_eventQueue.Count >= MaxPageSize || currentDate.Subtract(_lastMessageReceived).TotalMilliseconds > _cacheTimeOutMs)
                {
                    _lastMessageReceived = currentDate; //reset cache timer
                    submitMessagesToAction();
                }
            }
            catch (ThreadAbortException)
            {
                //swallow thread abort exception thrown when system is shutting down
            }
            catch (Exception)
            {
                //never throw an error from buffer
            }
            finally
            {
                Start();
            }

        }

        private void submitMessagesToAction()
        {
            try
            {
                if (_queueLock.TryEnterUpgradeableReadLock(250) == false) return; //can't get lock after 0.25 second so ignore and try again on next sweep
                if (_eventQueue.Count > 0)
                {
                    var objectsFromQueue = new List<T>();

                    try
                    {
                        if (_queueLock.TryEnterWriteLock(1000) == false) return; //can't get lock after 1 second so ignore

                        while (_eventQueue.Count > 0)  //empty buffer of objects as quickly as possible
                        {
                            T queuedItem;
                            if (_eventQueue.TryDequeue(out queuedItem))
                            {
                                objectsFromQueue.Add(queuedItem);
                            }
                        }
                    }
                    finally
                    {
                        _queueLock.ExitWriteLock();
                    }

                    if (objectsFromQueue.Count > 0)
                    {
                        Task.Factory.StartNew(() =>
                            {
                                _bufferAction(objectsFromQueue);
                            });
                    }
                }
            }
            catch (ThreadAbortException)
            {
                //swallow thread abort exception thrown when system is shutting down
            }
            catch (Exception ex)
            {
                //Log.InternalLog(ex, "Error queuing pages to thread pool");
                return;
            }
            finally
            {
                if (_queueLock.IsUpgradeableReadLockHeld)
                {
                    _queueLock.ExitUpgradeableReadLock();
                }
            }
        }

        //    protected abstract void OnFlushPage(IEnumerable<T> pageOfObjects);

        #region IDisposable Members

        /// <summary>
        /// Persist any remaining log entries in queue
        /// </summary>
        /// <param name="isDisposing">True is release both managed and unmanaged resources.  False for unmanaged only</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing == true)
            {
                OnClose(); //flush pending records
                //release managed resources 
            }

            // Free your own state (unmanaged objects).
            // Set large fields to null.

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose 
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
        }
        /// <summary>
        /// Class destructor.  Do not persist any queued log records
        /// </summary>
        ~AsyncBuffer()
        {
            Dispose(false);
        }

        #endregion
    }
}