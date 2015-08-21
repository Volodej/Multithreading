using System;
using System.Threading;
using FilesTree.Model.Tasks;
using FilesTree.Presenter.Events;

namespace FilesTree.Model
{
    internal sealed class FilesTreeModel
    {
        public event EventHandler<ErrorEventArgs> ErrorOccurred;
        public event EventHandler TreeBuildingFinished;
        public event EventHandler<FileSystemObjectEventArgs> TreeElementAdded;

        private FilesInfoCollector _infoCollector;
        private FilesInfoNotifier _infoNotifier;
        private XmlInfoWriter _xmlWriter;
        private int _tasksFinished;

        public void StartTreeBuilding(string targetFolderPath, string outputFilePath)
        {
            try
            {
                _tasksFinished = 0;

                _infoCollector = new FilesInfoCollector(targetFolderPath);
                _infoNotifier = new FilesInfoNotifier();
                _xmlWriter = new XmlInfoWriter(outputFilePath);

                _infoCollector.GotNewFileSystemData += GotNewFileSystemDataHandler;
                _infoCollector.TaskFinished += TaskFinishedHandler;
                _infoCollector.ErrorOccurred += ErrorOccurredHandler;
                _infoNotifier.NewFileInfoAdded += NewFileInfoAddedHandler;
                _infoNotifier.TaskFinished += TaskFinishedHandler;
                _infoNotifier.ErrorOccurred += ErrorOccurredHandler;
                _xmlWriter.TaskFinished += TaskFinishedHandler;
                _xmlWriter.ErrorOccurred += ErrorOccurredHandler;

                var infoCollectorThread = new Thread(_infoCollector.Process) {IsBackground = true, Name = "InfoCollectorThread"};
                var infoNotifierThread = new Thread(_infoNotifier.ProcessTask) {IsBackground = true, Name = "InfoNotifierThread"};
                var xmlWriterThread = new Thread(_xmlWriter.ProcessTask) {IsBackground = true, Name = "XmlWriterThread"};

                infoCollectorThread.Start();
                infoNotifierThread.Start();
                xmlWriterThread.Start();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(new ErrorEventArgs("Error occured while starting tree building.", ex));
            }
        }

        private void ErrorOccurredHandler(object sender, ErrorEventArgs e)
        {
            OnErrorOccurred(e);
        }

        /// <summary>
        /// Handle ending of each task.
        /// </summary>
        private void TaskFinishedHandler(object sender, EventArgs e)
        {
            if (sender == _infoCollector)
            {
                _infoNotifier.SetQueueFillingFinished();
                _xmlWriter.SetQueueFillingFinished();
            }
            Interlocked.Increment(ref _tasksFinished);
            // Check if all tasks finished
            if (_tasksFinished == 3)
            {
                OnTreeBuildingFinished();
                _infoCollector = null;
                _infoNotifier  = null;
                _xmlWriter = null;
            }
        }

        private void NewFileInfoAddedHandler(object sender, FileSystemObjectEventArgs e)
        {
            EventHandler<FileSystemObjectEventArgs> handler = TreeElementAdded;
            if (handler != null) handler(this, e);
        }

        private void GotNewFileSystemDataHandler(object sender, FileSystemObjectEventArgs e)
        {
            _infoNotifier.EnqueueTaskData(e.FileSystemObjectData);
            _xmlWriter.EnqueueTaskData(e.FileSystemObjectData);
        }

        public void AbortTreeBuilding()
        {
            if (_infoCollector != null)
                _infoCollector.AbortTask();
            if (_infoNotifier != null)
                _infoNotifier.AbortTask();
            if (_xmlWriter != null)
                _xmlWriter.AbortTask();
        }

        private void OnTreeBuildingFinished()
        {
            EventHandler handler = TreeBuildingFinished;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void OnErrorOccurred(ErrorEventArgs e)
        {
            EventHandler<ErrorEventArgs> handler = ErrorOccurred;
            if (handler != null) handler(this, e);
        }
    }
}
