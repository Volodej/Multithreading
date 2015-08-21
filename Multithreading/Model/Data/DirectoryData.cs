using System.Threading;

namespace FilesTree.Model.Data
{
    internal sealed class DirectoryData : FileSystemObjectData
    {
        private long _bytesSize;

        public DirectoryData(FileSystemObjectProperties properties, DirectoryData parent)
            : base(properties, parent)
        {
        }

        public override long BytesSize
        {
            get { return Interlocked.Read(ref _bytesSize); }
        }

        /// <summary>
        ///     Add to current directory child file system object (other directory or file). This method needed to increase
        ///     directory's BytesSize.
        /// </summary>
        /// <param name="objectData">File system object to add.</param>
        public void AddFileSystemObject(FileSystemObjectData objectData)
        {
            var fileData = objectData as FileData;
            if (fileData != null)
                AddBytesSize(fileData.BytesSize);
        }

        /// <summary>
        ///     Add BytesSize of all parent directories.
        /// </summary>
        /// <param name="size"></param>
        private void AddBytesSize(long size)
        {
            Interlocked.Add(ref _bytesSize, size);

            if (Parent != null)
                Parent.AddBytesSize(size);
        }
    }
}