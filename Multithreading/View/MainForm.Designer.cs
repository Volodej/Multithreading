namespace FilesTree.View
{
    partial class FilesTreeForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilesTreeForm));
            this._selectButton = new System.Windows.Forms.Button();
            this._filesTreeView = new System.Windows.Forms.TreeView();
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this.targetFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveOutputFileDialog = new System.Windows.Forms.SaveFileDialog();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._propertiesListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // _selectButton
            // 
            this._selectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._selectButton.Location = new System.Drawing.Point(384, 327);
            this._selectButton.Name = "_selectButton";
            this._selectButton.Size = new System.Drawing.Size(88, 23);
            this._selectButton.TabIndex = 0;
            this._selectButton.Text = "Select Folder";
            this._selectButton.UseVisualStyleBackColor = true;
            this._selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // _filesTreeView
            // 
            this._filesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._filesTreeView.ImageIndex = 0;
            this._filesTreeView.ImageList = this._imageList;
            this._filesTreeView.Location = new System.Drawing.Point(12, 12);
            this._filesTreeView.Name = "_filesTreeView";
            this._filesTreeView.SelectedImageIndex = 0;
            this._filesTreeView.Size = new System.Drawing.Size(238, 309);
            this._filesTreeView.TabIndex = 1;
            this._filesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._filesTreeView_AfterSelect);
            // 
            // _imageList
            // 
            this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
            this._imageList.TransparentColor = System.Drawing.Color.Transparent;
            this._imageList.Images.SetKeyName(0, "document_16xLG.png");
            this._imageList.Images.SetKeyName(1, "folder_Closed_16xLG.png");
            // 
            // targetFolderBrowserDialog
            // 
            this.targetFolderBrowserDialog.Description = "Select target folder";
            this.targetFolderBrowserDialog.ShowNewFolderButton = false;
            // 
            // saveOutputFileDialog
            // 
            this.saveOutputFileDialog.Filter = "XML file|*.xml";
            this.saveOutputFileDialog.Title = "Save files tree";
            // 
            // _progressBar
            // 
            this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._progressBar.Location = new System.Drawing.Point(12, 329);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(366, 19);
            this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this._progressBar.TabIndex = 2;
            // 
            // _propertiesListBox
            // 
            this._propertiesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._propertiesListBox.FormattingEnabled = true;
            this._propertiesListBox.IntegralHeight = false;
            this._propertiesListBox.Location = new System.Drawing.Point(256, 12);
            this._propertiesListBox.Name = "_propertiesListBox";
            this._propertiesListBox.Size = new System.Drawing.Size(214, 309);
            this._propertiesListBox.TabIndex = 3;
            // 
            // FilesTreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 362);
            this.Controls.Add(this._propertiesListBox);
            this.Controls.Add(this._progressBar);
            this.Controls.Add(this._filesTreeView);
            this.Controls.Add(this._selectButton);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "FilesTreeForm";
            this.Text = "Files Tree";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _selectButton;
        private System.Windows.Forms.TreeView _filesTreeView;
        private System.Windows.Forms.FolderBrowserDialog targetFolderBrowserDialog;
        private System.Windows.Forms.SaveFileDialog saveOutputFileDialog;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.ListBox _propertiesListBox;
        private System.Windows.Forms.ImageList _imageList;
    }
}

