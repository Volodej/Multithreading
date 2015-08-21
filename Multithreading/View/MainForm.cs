using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FilesTree.Model.Data;
using FilesTree.Presenter.Events;

namespace FilesTree.View
{
    internal sealed partial class FilesTreeForm : Form, IFilesTreeView
    {
        private readonly SynchronizationContext _context;
        private readonly List<TreeNode> _nodesHierarchy = new List<TreeNode>();
        private bool _isTreeBuilding;
        private TreeNode _lastAddedNode;

        public FilesTreeForm()
        {
            InitializeComponent();

            _context = SynchronizationContext.Current;
        }

        /// <summary>
        ///     Switch between "ready to build tree" mode and "wait tree building" mode.
        /// </summary>
        private bool IsTreeBuilding
        {
            get { return _isTreeBuilding; }
            set
            {
                _context.Send(s =>
                {
                    _isTreeBuilding = value;
                    _progressBar.Visible = value;
                    _selectButton.Text = value ? "Cancel" : "Select Folder";
                }, null);
            }
        }

        #region Interface Implementation

        public event EventHandler<SelectedFolderEventArgs> TreeBuildingStarted;
        public event EventHandler<FileSystemObjectEventArgs> TreeNodeSelected;
        public event EventHandler TreeBuildingCanceled;
        public event EventHandler<ErrorEventArgs> ViewExceptionOccurred;


        public void ShowError(string errorMessage, string exceptionMessage, string threadName)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append("Error occured. ");
            strBuilder.Append(errorMessage);
            if (exceptionMessage != null)
            {
                strBuilder.Append("\nException message: ");
                strBuilder.Append(exceptionMessage);
            }
            strBuilder.Append("\nError thread: ");
            strBuilder.Append(threadName);

            _context.Send(s => MessageBox.Show(strBuilder.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error), null);
        }

        public void ShowTreeBuildingProgress()
        {
            IsTreeBuilding = true;
            _filesTreeView.Nodes.Clear();
            _propertiesListBox.Items.Clear();
        }

        /// <summary>
        ///     Clear all information about previous tree from form.
        /// </summary>
        public void ResetFilesTree()
        {
            IsTreeBuilding = false;
        }

        public void AddTreeNode(FileSystemObjectData assosiatedData)
        {
            try
            {
                TreeNode parentNode = GetParentNode(assosiatedData);
                TreeNodeCollection nodeCollection = parentNode != null ? parentNode.Nodes : _filesTreeView.Nodes;
                int iconIndex = assosiatedData is FileData ? 0 : 1;
                var newTreeNode = new TreeNode(assosiatedData.Name, iconIndex, iconIndex) {Tag = assosiatedData};
                _lastAddedNode = newTreeNode;
                _context.Send(s => nodeCollection.Add(newTreeNode), null);
            }
            catch (Exception ex)
            {
                OnInterfaceExceptionOccurred("Error while adding new tree node.", ex);
            }
        }

        public void ShowTreeBuildingEnded()
        {
            _context.Send(s =>
            {
                IsTreeBuilding = false;
                MessageBox.Show("Files Tree building ended.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }, null);
        }

        public void ShowElementProperties(FileSystemObjectData data)
        {
            if (data == null)
            {
                _propertiesListBox.Items.Clear();
                return;
            }

            _propertiesListBox.BeginUpdate();
            _propertiesListBox.Items.Clear();
            _propertiesListBox.Items.Add("Owner: " + data.Owner);
            _propertiesListBox.Items.Add("Size: " + BytesCountToString(data.BytesSize));
            _propertiesListBox.Items.Add("Creation Time: " + data.CreationTime.ToString("g"));
            _propertiesListBox.Items.Add("LastWrite Time: " + data.LastWriteTime.ToString("g"));
            _propertiesListBox.Items.Add("LastAccess Time: " + data.LastAccessTime.ToString("g"));
            _propertiesListBox.EndUpdate();
        }

        #endregion

        private void selectButton_Click(object sender, EventArgs e)
        {
            if (IsTreeBuilding)
            {
                OnTreeBuildingCanceled();
            }
            else
            {
                SelectPathes();
            }
        }

        private void _filesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            EventHandler<FileSystemObjectEventArgs> handler = TreeNodeSelected;
            if (handler != null) handler(this, new FileSystemObjectEventArgs((FileSystemObjectData) e.Node.Tag));
        }

        private void SelectPathes()
        {
            if (targetFolderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;

            if (saveOutputFileDialog.ShowDialog() != DialogResult.OK)
                return;

            OnTreeBuildingStarted(targetFolderBrowserDialog.SelectedPath, saveOutputFileDialog.FileName);
        }

        /// <summary>
        ///     Find parent tree node for current adding data in _nodesHierarchy.
        /// </summary>
        /// <param name="assosiatedData">Assosiated data for new tree node.</param>
        /// <returns>Returns tree node for which have to be added new node with assossiatedData.</returns>
        private TreeNode GetParentNode(FileSystemObjectData assosiatedData)
        {
            if (_lastAddedNode == null)
                return null;

            if (assosiatedData.Parent == null)
            {
                _nodesHierarchy.Clear();
                return null;
            }

            if (_lastAddedNode.Tag == assosiatedData.Parent)
            {
                _nodesHierarchy.Add(_lastAddedNode);
                return _lastAddedNode;
            }

            for (int i = _nodesHierarchy.Count - 1; i >= 0; i--)
            {
                if (_nodesHierarchy[i].Tag == assosiatedData.Parent)
                    return _nodesHierarchy[i];
                _nodesHierarchy.RemoveAt(i);
            }

            throw new InvalidOperationException("Can't find parent node.");
        }

        /// <summary>
        ///     Format number of bytes count to readable string representation.
        /// </summary>
        /// <param name="byteCount">Number of bytes.</param>
        /// <returns>Returns string representation of size.</returns>
        private string BytesCountToString(long byteCount)
        {
            string[] suf = {"B", "KB", "MB", "GB", "TB", "PB", "EB"};
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes/Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount)*num) + suf[place];
        }

        #region Events Invocators

        private void OnTreeBuildingStarted(string targetFolderPath, string outputFilePath)
        {
            EventHandler<SelectedFolderEventArgs> handler = TreeBuildingStarted;
            if (handler != null) handler(this, new SelectedFolderEventArgs(targetFolderPath, outputFilePath));
        }

        private void OnTreeBuildingCanceled()
        {
            EventHandler handler = TreeBuildingCanceled;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void OnInterfaceExceptionOccurred(string errorMessage, Exception exception)
        {
            EventHandler<ErrorEventArgs> handler = ViewExceptionOccurred;
            if (handler != null) handler(this, new ErrorEventArgs(errorMessage, exception));
        }

        #endregion
    }
}