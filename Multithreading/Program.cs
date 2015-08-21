using System;
using System.Threading;
using System.Windows.Forms;
using FilesTree.Model;
using FilesTree.Presenter;
using FilesTree.View;

namespace FilesTree
{
    internal static class Program
    {
        /// <summary>
        ///     Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Thread.CurrentThread.Name = "MainThread";

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var treeView = new FilesTreeForm();
            var treeModel = new FilesTreeModel();
            var treePresenter = new FilesTreePresenter(treeView, treeModel);

            Application.Run(treeView);
        }
    }
}