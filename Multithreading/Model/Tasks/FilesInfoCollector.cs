using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;
using FilesTree.Model.Data;
using FilesTree.Presenter.Events;
using ErrorEventArgs = FilesTree.Presenter.Events.ErrorEventArgs;

namespace FilesTree.Model.Tasks
{
    /// <summary>
    ///     Class for gathering information about selected directory and about all nested directories and files.
    /// </summary>
    internal sealed class FilesInfoCollector
    {
        private readonly IList<IdentityReference> _currentUserGroups;
        private readonly SecurityIdentifier _currentUserIdentifier;
        private readonly string _directoryPath;

        private bool _isAborted;

        public FilesInfoCollector(string directoryPath)
        {
            _directoryPath = directoryPath;
            WindowsIdentity user = WindowsIdentity.GetCurrent();

            // Save user and user's groups identifiers for security check
            _currentUserIdentifier = user.User;
            _currentUserGroups = user.Groups.ToList();
        }

        public event EventHandler<FileSystemObjectEventArgs> GotNewFileSystemData;
        public event EventHandler TaskFinished;
        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        /// <summary>
        ///     Start gathering information about selected folder.
        /// </summary>
        public void Process()
        {
            try
            {
                var directoryInfo = new DirectoryInfo(_directoryPath);
                GetDirectoryData(directoryInfo, null);
                OnTaskFinished();
            }
            catch (Exception ex)
            {
                OnErrorOccurred("Unknown error.", ex);
            }
        }

        public void AbortTask()
        {
            _isAborted = true;
        }

        private void GetDirectoryData(DirectoryInfo directoryInfo, DirectoryData parent)
        {
            // Get directory access control
            DirectorySecurity accessControl;
            try
            {
                accessControl = directoryInfo.GetAccessControl(AccessControlSections.Access);
            }
            catch (UnauthorizedAccessException ex)
            {
                OnErrorOccurred("Can't get access to directory " + directoryInfo.Name, ex);
                throw;
            }

            // Get current directory properties
            FileSystemObjectProperties properties = GetFileSystemObjectProperties(directoryInfo, accessControl);
            var directoryData = new DirectoryData(properties, parent);
            if (parent != null)
                parent.AddFileSystemObject(directoryData);
            OnGotNewFileSystemData(directoryData);

            // Get data for all nested files and directories
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                if (_isAborted)
                    return;
                GetDirectoryData(directory, directoryData);
            }
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (_isAborted)
                    return;
                GetFileData(file, directoryData);
            }
        }

        private void GetFileData(FileInfo fileInfo, DirectoryData parent)
        {
            // Get file access control
            FileSecurity accessControl;
            try
            {
                accessControl = fileInfo.GetAccessControl(AccessControlSections.Access);
            }
            catch (UnauthorizedAccessException ex)
            {
                OnErrorOccurred("Can't get access to file " + fileInfo.Name, ex);
                throw;
            }

            FileSystemObjectProperties properties = GetFileSystemObjectProperties(fileInfo, accessControl);
            var fileData = new FileData(properties, parent, fileInfo.Length);
            parent.AddFileSystemObject(fileData);
            OnGotNewFileSystemData(fileData);
        }

        private FileSystemObjectProperties GetFileSystemObjectProperties(FileSystemInfo info, CommonObjectSecurity objectSecurity)
        {
            IdentityReference owner = objectSecurity.GetOwner(typeof (NTAccount));
            string objectOwner = owner != null ? owner.Value : "Unknown";

            return new FileSystemObjectProperties
            {
                Name = info.Name,
                CreationTime = info.CreationTime,
                LastWriteTime = info.LastWriteTime,
                LastAccessTime = info.LastAccessTime,
                Attributes = info.Attributes,
                FileSystemRights = GetFileSystemRights(objectSecurity),
                Owner = objectOwner
            };
        }

        private FileSystemRights GetFileSystemRights(CommonObjectSecurity objectSecurity)
        {
            AuthorizationRuleCollection accessRules = objectSecurity.GetAccessRules(true, true, typeof (SecurityIdentifier));
            FileSystemRights fileSystemAllowRights = 0;
            FileSystemRights fileSystemDenyRights = 0;
            foreach (FileSystemAccessRule accessRule in accessRules)
            {
                IdentityReference identityReference = accessRule.IdentityReference;
                if (identityReference != _currentUserIdentifier && _currentUserGroups.All(reference => reference != identityReference))
                    continue;
                if (accessRule.AccessControlType == AccessControlType.Deny)
                {
                    fileSystemDenyRights = fileSystemDenyRights | accessRule.FileSystemRights;
                }
                else
                {
                    fileSystemAllowRights = fileSystemAllowRights | accessRule.FileSystemRights;
                }
            }

            return fileSystemAllowRights & (~fileSystemDenyRights);
        }

        private void OnGotNewFileSystemData(FileSystemObjectData objectData)
        {
            if (_isAborted)
                return;

            EventHandler<FileSystemObjectEventArgs> handler = GotNewFileSystemData;
            if (handler != null) handler(this, new FileSystemObjectEventArgs(objectData));
        }

        private void OnTaskFinished()
        {
            EventHandler handler = TaskFinished;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void OnErrorOccurred(string errorMessage, Exception exception)
        {
            if (_isAborted)
                return;
            _isAborted = true;
            EventHandler<ErrorEventArgs> handler = ErrorOccurred;
            if (handler != null) handler(this, new ErrorEventArgs(errorMessage, exception));
        }
    }
}