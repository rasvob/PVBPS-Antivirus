using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;
using AntiVirusLib.Database;
using AntiVirusLib.Models;
using AntiVirusLib.Scanner;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using PVBPS_Antivirus.Config;
using Application = System.Windows.Application;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace PVBPS_Antivirus.ViewModels
{
    public class MainWindowViewModel: BaseViewModel
    {
        private readonly MalwareScanner _scanner;

        public ObservableCollection<SampleViewModel> Models { get; set; }

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
            Models = new ObservableCollection<SampleViewModel>();

            ConfigGateway configGateway = new ConfigGateway();
            _scanner = new MalwareScanner(configGateway.YaraPath, configGateway.IndexRule, configGateway.CustomRule, configGateway.DbPath, configGateway.QuarantinePath, configGateway.ApiKey);
        }

        private void OpenFolderCommandExecute(object o)
        {
            var openFileDialog = new FolderBrowserDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FilePath = openFileDialog.SelectedPath;

                IEnumerable<string> files = Directory.EnumerateFiles(FilePath);
                IEnumerable<FileModel> models = files.Select(t => new FileModel() {FilePath = t, Name = Path.GetFileNameWithoutExtension(t)});
                
                Models.Clear();
                foreach (FileModel model in models)
                {
                    Models.Add(new SampleViewModel() {FileModel = model, Pos = Models.Count});
                }
            }
        }

        private async void DeepScanCommandExecute(object o)
        {
            await PerformScan(false);
        }

        private bool FastScanCommandCanExecute(object o)
        {
            return Models != null && Models.Any();
        }

        private async void FastScanCommandExecute(object o)
        {
            await PerformScan();
        }

        private void OpenFileCommandExecute(object o)
        {
            var fileDialog = new OpenFileDialog();
            var result = fileDialog.ShowDialog();

            if (!result.HasValue || !result.Value) return;

            FilePath = fileDialog.FileName;
            fileDialog.Reset();
            Models.Clear();
            FileModel model = new FileModel() {FilePath = FilePath, Name = Path.GetFileNameWithoutExtension(FilePath)};
            Models.Add(new SampleViewModel() { FileModel = model, Pos = Models.Count });
        }

        private async Task PerformScan(bool fast = true)
        {
            ProgressDialogController controller = await ((MetroWindow)Application.Current.MainWindow).ShowProgressAsync("Please wait", "Scan in progress...");
            controller.SetIndeterminate();
            foreach (var model in Models)
            {
                FileModel fileModel = model.FileModel;

                if (fast)
                {
                    await _scanner.FastScan(fileModel);
                }
                else
                {
                    await _scanner.DeepScan(fileModel);
                }

                if (!fileModel.IsClean)
                {
                    _scanner.SaveToDb(fileModel);
                }

                model.FileModel = null;
                model.FileModel = fileModel;
            }

            await controller.CloseAsync();
        }
    }
}