using System;
using System.IO;
using System.Security.AccessControl;

namespace FilesTree.Model.Data
{
    /// <summary>
    /// Accessory class
    /// </summary>
    internal sealed class FileSystemObjectProperties
    {
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public FileAttributes Attributes { get; set; }
        public string Owner { get; set; }
        public FileSystemRights FileSystemRights { get; set; }
    }
}