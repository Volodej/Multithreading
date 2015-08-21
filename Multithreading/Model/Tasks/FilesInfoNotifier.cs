using System;
using FilesTree.Model.Data;
using FilesTree.Presenter.Events;

namespace FilesTree.Model.Tasks
{
    /// <summary>
    ///     Class for building files tree in GUI.  
    /// </summary>
    internal sealed class FilesInfoNotifier : QueueableTask<FileSystemObjectData>
    {
        public event EventHandler<FileSystemObjectEventArgs> NewFileInfoAdded;

        protected override void ProcessDequeuedData(FileSystemObjectData data)
        {
            EventHandler<FileSystemObjectEventArgs> handler = NewFileInfoAdded;
            if (handler != null) handler(this, new FileSystemObjectEventArgs(data));
        }
    }
}