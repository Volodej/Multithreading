using System;
using System.Collections.Generic;
using System.Threading;
using FilesTree.Presenter.Events;

namespace FilesTree.Model.Tasks
{
    /// <summary>
    ///     Abstract thread-safe class for tasks, that have to be complemented while running.
    /// </summary>
    /// <typeparam name="T">Task data to queue.</typeparam>
    internal abstract class QueueableTask<T>
    {
        private readonly Queue<T> _dataQueue = new Queue<T>();
        private readonly Object _lock = new Object();

        /// <summary>
        ///     Field represents if thread, that gathering files and directories information, has finished to add new data to
        ///     queue.
        /// </summary>
        private volatile bool _queueFillingFinished;

        /// <summary>
        ///     Field represents if current task was aborted.
        /// </summary>
        private volatile bool _taskAborted;

        public event EventHandler TaskFinished;
        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        /// <summary>
        ///     Add data to task's queue to process.
        /// </summary>
        /// <param name="data">Data to add to queue.</param>
        public void EnqueueTaskData(T data)
        {
            Monitor.Enter(_lock);
            _dataQueue.Enqueue(data);
            Monitor.Pulse(_lock);
            Monitor.Exit(_lock);
        }

        /// <summary>
        ///     Start extraction and processing data from task's queue.
        /// </summary>
        public void ProcessTask()
        {
            try
            {
                while (!_taskAborted)
                {
                    Monitor.Enter(_lock);
                    while (_dataQueue.Count == 0)
                    {
                        if (_queueFillingFinished)
                        {
                            Monitor.Exit(_lock);
                            FinishTask();
                            return;
                        }
                        Monitor.Wait(_lock);
                        if (_taskAborted)
                        {
                            Monitor.Exit(_lock);
                            return;
                        }
                    }
                    T data = _dataQueue.Dequeue();
                    Monitor.Exit(_lock);
                    ProcessDequeuedData(data);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred("Unknown error.", ex);
            }
        }

        public void SetQueueFillingFinished()
        {
            Monitor.Enter(_lock);
            _queueFillingFinished = true;
            Monitor.Pulse(_lock);
            Monitor.Exit(_lock);
        }

        public void AbortTask()
        {
            if (_taskAborted)
                return;

            Monitor.Enter(_lock);
            _taskAborted = true;
            Monitor.Pulse(_lock);
            Monitor.Exit(_lock);

            DisposeThis();
        }

        /// <summary>
        /// Process dequeued data in derived class.
        /// </summary>
        /// <param name="data">Data to process.</param>
        protected abstract void ProcessDequeuedData(T data);

        protected virtual void FinishTask()
        {
            DisposeThis();

            EventHandler handler = TaskFinished;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected void OnErrorOccurred(string errorMessage, Exception exception)
        {
            if (_taskAborted)
                return;
            _taskAborted = true;
            EventHandler<ErrorEventArgs> handler = ErrorOccurred;
            if (handler != null) handler(this, new ErrorEventArgs(errorMessage, exception));
        }

        private void DisposeThis()
        {
            var disposable = this as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}