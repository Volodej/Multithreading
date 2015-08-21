using System;
using System.IO;
using System.Security.AccessControl;

namespace FilesTree.Model.Data
{
    /// <summary>
    /// Abstract class that contains information about file system object.
    /// </summary>
    internal abstract class FileSystemObjectData
    {
        protected FileSystemObjectData(FileSystemObjectProperties properties, DirectoryData parent)
        {
            FileSystemRights = properties.FileSystemRights;
            Owner = properties.Owner;
            Attributes = properties.Attributes;
            LastAccessTime = properties.LastAccessTime;
            LastWriteTime = properties.LastWriteTime;
            CreationTime = properties.CreationTime;
            Name = properties.Name;
            Parent = parent;
        }

        public string Name { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime LastWriteTime { get; private set; }
        public DateTime LastAccessTime { get; private set; }
        public FileAttributes Attributes { get; private set; }
        public string Owner { get; private set; }
        public FileSystemRights FileSystemRights { get; private set; }
        public DirectoryData Parent { get; private set; }
        public abstract long BytesSize { get; }
    }
}