using System;
using System.Threading;
using FilesTree.Model;
using FilesTree.Presenter.Events;
using FilesTree.View;

namespace FilesTree.Presenter
{
    internal sealed class FilesTreePresenter
    {
        private readonly FilesTreeModel _model;
        private readonly IFilesTreeView _view;

        public FilesTreePresenter(IFilesTreeView view, FilesTreeModel model)
        {
            _view = view;
            _model = model;

            _view.TreeBuildingStarted += TreeBuildingStartedHandler;
            _view.TreeBuildingCanceled += TreeBuildingCanceledHandler;
            _view.ViewExceptionOccurred += ErrorOccurredHandler;
            view.TreeNodeSelected += TreeNodeSelectedHandler;

            _view.ResetFilesTree();

            _model.ErrorOccurred += ErrorOccurredHandler;
            _model.TreeBuildingFinished += TreeBuildingFinishedHandler;
            _model.TreeElementAdded += TreeElementAddedHandler;
        }

        private void TreeBuildingStartedHandler(object sender, SelectedFolderEventArgs e)
        {
            _view.ShowTreeBuildingProgress();
            _model.StartTreeBuilding(e.TargetFolderPath, e.OutputFilePath);
        }

        private void TreeBuildingCanceledHandler(object sender, EventArgs e)
        {
            _model.AbortTreeBuilding();
            _view.ResetFilesTree();
        }

        private void ErrorOccurredHandler(object sender, ErrorEventArgs e)
        {
            _model.AbortTreeBuilding();
            _view.ShowError(e.ErrorMessage, e.Exception != null ? e.Exception.Message : null, Thread.CurrentThread.Name);
            _view.ResetFilesTree();
        }

        private void TreeNodeSelectedHandler(object sender, FileSystemObjectEventArgs e)
        {
            _view.ShowElementProperties(e.FileSystemObjectData);
        }

        private void TreeBuildingFinishedHandler(object sender, EventArgs e)
        {
            _view.ShowTreeBuildingEnded();
        }

        private void TreeElementAddedHandler(object sender, FileSystemObjectEventArgs e)
        {
            _view.AddTreeNode(e.FileSystemObjectData);
        }
    }
}