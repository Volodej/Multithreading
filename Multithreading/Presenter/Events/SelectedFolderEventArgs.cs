using System;

namespace FilesTree.Presenter.Events
{
    internal sealed class SelectedFolderEventArgs : EventArgs
    {
        private readonly string _outputFilePath;
        private readonly string _targetFolderPath;

        public SelectedFolderEventArgs(string targetFolderPath, string outputFilePath)
        {
            _targetFolderPath = targetFolderPath;
            _outputFilePath = outputFilePath;
        }

        public string TargetFolderPath
        {
            get { return _targetFolderPath; }
        }

        public string OutputFilePath
        {
            get { return _outputFilePath; }
        }
    }
}