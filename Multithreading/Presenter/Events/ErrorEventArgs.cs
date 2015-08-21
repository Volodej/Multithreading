using System;

namespace FilesTree.Presenter.Events
{
    internal sealed class ErrorEventArgs : EventArgs
    {
        private readonly string _errorMessage;
        private readonly Exception _exception;

        public ErrorEventArgs(string errorMessage, Exception exception)
        {
            _errorMessage = errorMessage;
            _exception = exception;
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }
}