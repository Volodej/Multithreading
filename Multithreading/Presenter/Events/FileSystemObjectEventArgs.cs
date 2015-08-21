using System;
using FilesTree.Model.Data;

namespace FilesTree.Presenter.Events
{
    internal sealed class FileSystemObjectEventArgs : EventArgs
    {
        private readonly FileSystemObjectData _fileSystemObjectData;

        public FileSystemObjectEventArgs(FileSystemObjectData fileSystemObjectData)
        {
            _fileSystemObjectData = fileSystemObjectData;
        }

        public FileSystemObjectData FileSystemObjectData
        {
            get { return _fileSystemObjectData; }
        }
    }
}