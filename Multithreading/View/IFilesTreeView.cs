using System;
using FilesTree.Model.Data;
using FilesTree.Presenter.Events;

namespace FilesTree.View
{
    internal interface IFilesTreeView
    {
        event EventHandler<SelectedFolderEventArgs> TreeBuildingStarted;
        event EventHandler<FileSystemObjectEventArgs> TreeNodeSelected;
        event EventHandler TreeBuildingCanceled;
        event EventHandler<ErrorEventArgs> ViewExceptionOccurred;

        void ShowError(string errorMessage, string exceptionMessage, string threadName);
        void ShowTreeBuildingProgress();
        void ResetFilesTree();
        void AddTreeNode(FileSystemObjectData assosiatedData);
        void ShowTreeBuildingEnded();
        void ShowElementProperties(FileSystemObjectData data);
    }
}