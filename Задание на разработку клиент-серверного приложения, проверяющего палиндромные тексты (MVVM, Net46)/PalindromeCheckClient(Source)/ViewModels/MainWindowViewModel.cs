using PalindromeCheckClient.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Threading;

namespace PalindromeCheckClient.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly MainWindowModel _Model = new MainWindowModel();
        public MainWindowViewModel()
        {
            _Model.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            OpenCommand = new DelegateCommand(() =>
            {
                _Model.ShowOpenDialogForFolderPath();
                
                if (_Model.FillDataGridWithFolderPath() == -1)
                {
                    _Model.FolderPath = "ошибка чтения файлов";
                    Thread.Sleep(500);
                    _Model.FolderPath = "";
                }
            });
            CheckCommand = new DelegateCommand(() =>
            {
                if (URI.Trim() != "" && (URI.Trim().IndexOf("http://") != -1 || URI.Trim().IndexOf("https://") != -1))
                {
                    _Model.CheckPalindrome();
                }
                else
                {
                    _Model.URI = "неверный адрес";
                    Thread.Sleep(500);
                    _Model.URI = "http ://127.0.0.1:8080/"; 
                }
            });
        }

        public string URI { get => _Model.URI; set => _Model.URI = value; }
        public bool WaitingForCommand { get => _Model.WaitingForCommand; }
        public string FolderPath { get => _Model.FolderPath; set => _Model.FolderPath = value; }

        public DelegateCommand OpenCommand { get; }
        public DelegateCommand CheckCommand { get; }
        public ReadOnlyObservableCollection<FileDataItem> FilesToCheckDGItems => _Model.PublicFileDataCollectionForDG;
        public ReadOnlyObservableCollection<PalindromeStatusItem> PalindromeStatusDGItems => _Model.PalindromeStatusPublicCollection;
    }    
}
