using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using AntiVirusLib.Models;
using Microsoft.Win32;
using Application = System.Windows.Application;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace PVBPS_Antivirus.ViewModels
{
    public class MainWindowViewModel: BaseViewModel
    {
        public ObservableCollection<FileModel> Models { get; set; }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (value == _filePath) return;
                _filePath = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenFileCommand { get; }
        public ICommand OpenFolderCommand { get; }
        public ICommand FastScanCommand { get; }
        public ICommand DeepScanCommand { get; }

        public MainWindowViewModel()
        {
            OpenFileCommand = new SimpleCommand(OpenFileCommandExecute);
            OpenFolderCommand = new SimpleCommand(OpenFolderCommandExecute);
            FastScanCommand = new SimpleCommand(FastScanCommandExecute, FastScanCommandCanExecute);
            DeepScanCommand = new SimpleCommand(DeepScanCommandExecute, FastScanCommandCanExecute);
            Models = new ObservableCollection<FileModel>();
        }

        private void OpenFolderCommandExecute(object o)
        {
            var openFileDialog = new FolderBrowserDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string name = openFileDialog.SelectedPath;
            }
        }

        private void DeepScanCommandExecute(object o)
        {
            throw new NotImplementedException();
        }

        private bool FastScanCommandCanExecute(object o)
        {
            throw new NotImplementedException();
        }

        private void FastScanCommandExecute(object o)
        {
            throw new NotImplementedException();
        }

        private void OpenFileCommandExecute(object o)
        {
            var fileDialog = new OpenFileDialog();
            var result = fileDialog.ShowDialog();

            if (!result.HasValue || !result.Value) return;

            string file = fileDialog.FileName;
            fileDialog.Reset();
            Models.Add(new FileModel() {FilePath = file});
        }
    }
}