using AntiVirusLib.Models;

namespace PVBPS_Antivirus.ViewModels
{
    public class SampleViewModel: BaseViewModel
    {
        private FileModel _fileModel;
        private int _pos;

        public FileModel FileModel
        {
            get { return _fileModel; }
            set
            {
                if (Equals(value, _fileModel)) return;
                _fileModel = value;
                OnPropertyChanged();
            }
        }

        public int Pos
        {
            get { return _pos; }
            set
            {
                if (value == _pos) return;
                _pos = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title
        {
            get { return $"Sample #{Pos}"; }
        }
    }
}