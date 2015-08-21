namespace FilesTree.Model.Data
{
    internal sealed class FileData : FileSystemObjectData
    {
        private readonly long _bytesSize;

        public FileData(FileSystemObjectProperties properties, DirectoryData parent, long fileSize)
            : base(properties, parent)
        {
            _bytesSize = fileSize;
        }

        public override long BytesSize
        {
            get { return _bytesSize; }
        }
    }
}