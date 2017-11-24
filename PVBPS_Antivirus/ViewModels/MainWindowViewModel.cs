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
using AntiVirusLib.FileInfo;
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
        private readonly FileTypeChecker _fileTypeChecker;

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
            _fileTypeChecker = new FileTypeChecker();
            _scanner = new MalwareScanner(configGateway.YaraPath, configGateway.IndexRule, configGateway.CustomRule, configGateway.DbPath, configGateway.QuarantinePath, configGateway.ApiKey, configGateway.StringsPath);
        }

        public void CopyToClipboard(string s)
        {
            Clipboard.SetText(s);
        }

        private async void OpenFolderCommandExecute(object o)
        {
            var openFileDialog = new FolderBrowserDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FilePath = openFileDialog.SelectedPath;

                IEnumerable<string> files = Directory.EnumerateFiles(FilePath);
                List<FileModel> models = files
                    .Select(t => new FileModel() {FilePath = t, Name = Path.GetFileNameWithoutExtension(t)}).ToList();

                List<FileModel> valid = models.Where(t => _fileTypeChecker.IsValid(t.FilePath)).ToList();
                Models.Clear();
                foreach (FileModel model in valid)
                {
                    Models.Add(new SampleViewModel() {FileModel = model, Pos = Models.Count});
                }

                List<string> invalidFiles = models.Where(t => !_fileTypeChecker.IsValid(t.FilePath))
                    .Select(t => Path.GetFileNameWithoutExtension(t.FilePath)).ToList();

                if (invalidFiles.Any())
                {
                    string msg = $"Files:\n{string.Join("\n", invalidFiles)}\nare not valid exe or dll";
                    await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("Error", msg);
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

        private async void OpenFileCommandExecute(object o)
        {
            var fileDialog = new OpenFileDialog();
            var result = fileDialog.ShowDialog();

            if (!result.HasValue || !result.Value) return;

            FilePath = fileDialog.FileName;

            bool isValid = _fileTypeChecker.IsValid(FilePath);

            if (!isValid)
            {
                await ((MetroWindow) Application.Current.MainWindow).ShowMessageAsync("Error",
                    "Given file is not valid exe or dll, please try another one");
            }

            fileDialog.Reset();
            Models.Clear();
            FileModel model = new FileModel() {FilePath = FilePath, Name = Path.GetFileNameWithoutExtension(FilePath)};
            Models.Add(new SampleViewModel() { FileModel = model, Pos = Models.Count });
        }

        private void PreprocessSamples()
        {
            foreach (SampleViewModel model in Models.ToList())
            {
                if (!File.Exists(model.FileModel.FilePath))
                {
                    Models.Remove(model);
                }
            }
        }

        private async Task PerformScan(bool fast = true)
        {
            ProgressDialogController controller = await ((MetroWindow)Application.Current.MainWindow).ShowProgressAsync("Please wait", "Scan in progress...");
            controller.SetIndeterminate();

            PreprocessSamples();

            foreach (var model in Models)
            {
                FileModel fileModel = model.FileModel;

                _fileTypeChecker.IsValid(fileModel.FilePath);

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

            var infected = Models.Where(t => !t.FileModel.IsClean).Select(t => t.FileModel.Name).ToList();

            if (!infected.Any())
            {
                return;
            }

            string quarantineMessage =
                $"These files are infected:\n{string.Join("\n", infected)}\nDo you want to move them to quarantine ?";

            MetroDialogSettings settings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No"
            };
            MessageDialogResult dialogResult = await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("Scan complete", quarantineMessage, MessageDialogStyle.AffirmativeAndNegative, settings);

            if (dialogResult == MessageDialogResult.Affirmative)
            {
                var infectedFiles = Models.Where(t => !t.FileModel.IsClean).Select(t => t.FileModel.FilePath).ToList();
                var notMoved = _scanner.MoveFilesToQurantine(infectedFiles);

                if (notMoved.Any())
                {
                    await ((MetroWindow) Application.Current.MainWindow).ShowMessageAsync("Scan complete",
                        $"{Models.Count - notMoved.Count} out of {Models.Count} moved");
                }
            }
        }
    }
}