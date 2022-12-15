using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StenographAudio_WPF_.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Threading;
using Microsoft.Win32;

namespace StenographAudio_WPF_.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private readonly MainWindowModel _model = new MainWindowModel();

        public DelegateCommand SoundFileSearchCommand { get; }
        public DelegateCommand FileForHidingSearchCommand { get; }
        public DelegateCommand DestForEncrFileCommand { get; }

        public DelegateCommand EncrFileSearchCommand { get; }
        public DelegateCommand DestForDecrFileCommand { get; }

        public DelegateCommand EncryptStartCommand { get; }
        public DelegateCommand DecryptStartCommand { get; }
        
        public MainWindowViewModel()
        {
            _model.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            EncryptBitsCBSelectedIndex = 0;
            DecryptBitsCBSelectedIndex = 0;
            _model.CryptBtnsIsEnabled = true;

            SoundFileSearchCommand = new DelegateCommand(() =>      //кнопка обзора пути к "Исходному звуковому файлу"
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    Filter = "wave files (*.wav)|*.wav|All files (*.*)|*.*",
                    Title = "Выберите исходный звуковой файл"
                };
                InputSoundFilePath = ShowOpenDialog(dialog);
                RaisePropertyChanged(nameof(InputSoundFilePath));
            });
            FileForHidingSearchCommand = new DelegateCommand(() =>  //кнопка обзора пути к "Скрываемому файлу"
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    Filter = "ZIP files (*.zip)|*.zip|All files (*.*)|*.*",
                    Title = "Выберите скрываемый файл"
                };
                InputFileForHidingPath = ShowOpenDialog(dialog);
                RaisePropertyChanged(nameof(InputFileForHidingPath));
            });
            DestForEncrFileCommand = new DelegateCommand(() =>      //кнопка обзора пути к сохраняемому "Выходному (закодированному) аудио файлу"
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Filter = "wave files (*.wav)|*.wav|All files (*.*)|*.*",
                    Title = "Сохранить закодированный аудио файл",
                };
                OutputEncrFilePath = ShowSaveDialog(saveFileDialog);
                RaisePropertyChanged(nameof(OutputEncrFilePath));
            });

            EncrFileSearchCommand = new DelegateCommand(() =>       //кнопка обзора пути к "Исходному (закодированному) звуковому файлу"
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    Filter = "wave files (*.wav)|*.wav|All files (*.*)|*.*",
                    Title = "Выберите закодированный звуковой файл"
                };
                InputEncrFilePath = ShowOpenDialog(dialog);
                RaisePropertyChanged(nameof(InputEncrFilePath));
            });
            DestForDecrFileCommand = new DelegateCommand(() =>      //кнопка обзора пути к сохраняемому "Выходному (раскодированному) файлу"
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Filter = "ZIP files (*.zip)|*.zip|All files (*.*)|*.*",
                    Title = "Сохранить раскодированный файл",
                };
                OutputDecrFilePath = ShowSaveDialog(saveFileDialog);
                RaisePropertyChanged(nameof(OutputDecrFilePath));
            });

            EncryptStartCommand = new DelegateCommand(() => _model.EncryptStart());
            DecryptStartCommand = new DelegateCommand(() => _model.DecryptStart());
        }


        private string ShowOpenDialog(OpenFileDialog dialog)
        {
            if (dialog.ShowDialog() == true)
                return dialog.FileName;
            else
                return "ошибка чтения файла";
        }

        private string ShowSaveDialog(SaveFileDialog dialog)
        {
            if (dialog.ShowDialog() == true)
                return dialog.FileName;
            else
                return "ошибка задания пути к файлу";
        }

        public string InputSoundFilePath { get => _model.InputSoundFilePath; private set => _model.InputSoundFilePath = value; }
        public string InputFileForHidingPath { get => _model.InputFileForHidingPath; private set => _model.InputFileForHidingPath = value; }
        public string OutputEncrFilePath { get => _model.OutputEncrFilePath; private set => _model.OutputEncrFilePath = value; }
        public int EncryptBitsCBSelectedIndex { get => _model.BitsForCryptCount - 1; set => _model.BitsForCryptCount = value + 1; }
        public string InputEncrFilePath { get => _model.InputEncrFilePath; private set => _model.InputEncrFilePath = value; }
        public string OutputDecrFilePath { get => _model.OutputDecrFilePath; private set => _model.OutputDecrFilePath = value; }
        public int DecryptBitsCBSelectedIndex { get => _model.BitsForDecryptCount - 1; set => _model.BitsForDecryptCount = value + 1; }

        public double ProgrBarEncrVal { get => _model.ProgressEncrVal; }
        public double ProgrBarDecrVal { get => _model.ProgressDecrVal; }
        public bool CryptBtnsIsEnabled { get => _model.CryptBtnsIsEnabled; }
    }
}
