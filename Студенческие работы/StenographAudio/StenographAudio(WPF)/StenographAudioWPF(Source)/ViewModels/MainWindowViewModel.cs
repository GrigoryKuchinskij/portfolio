﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StenographAudio_WPF_.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Threading;

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

            SoundFileSearchCommand = new DelegateCommand(() =>
            {
                if (_model.SoundFileSearchDialog() == -1)
                    _model.InputSoundFilePath = "ошибка чтения файла";
            });
            FileForHidingSearchCommand = new DelegateCommand(() =>
            {
                if (_model.FileForHidingSearchDialog() == -1)
                    _model.InputFileForHidingPath = "ошибка чтения файла";
            });
            DestForEncrFileCommand = new DelegateCommand(() =>
            {
                if (_model.DestForEncrFileDialog() == -1)
                    _model.OutputEncrFilePath = "ошибка задания пути к файлу";
            });

            EncrFileSearchCommand = new DelegateCommand(() =>
            {
                if (_model.EncrFileSearchDialog() == -1)
                    _model.InputEncrFilePath = "ошибка чтения файла";
            });
            DestForDecrFileCommand = new DelegateCommand(() =>
            {
                if (_model.DestForDecrFileDialog() == -1)
                    _model.OutputDecrFilePath = "ошибка задания пути к файлу";
            });

            EncryptStartCommand = new DelegateCommand(() => _model.EncryptStart());
            DecryptStartCommand = new DelegateCommand(() => _model.DecryptStart());
        }

        public string InputSoundFilePath { get => _model.InputSoundFilePath; }
        public string InputFileForHidingPath { get => _model.InputFileForHidingPath; }
        public string OutputEncrFilePath { get => _model.OutputEncrFilePath; }
        public int EncryptBitsCBSelectedIndex { get => _model.BitsForCryptCount - 1; set => _model.BitsForCryptCount = value + 1; }
        public string InputEncrFilePath { get => _model.InputEncrFilePath; }
        public string OutputDecrFilePath { get => _model.OutputDecrFilePath; }
        public int DecryptBitsCBSelectedIndex { get => _model.BitsForDecryptCount - 1; set => _model.BitsForDecryptCount = value + 1; }

        public double ProgrBarEncrVal { get => _model.ProgressEncrVal; }
        public double ProgrBarDecrVal { get => _model.ProgressDecrVal; }
        public bool CryptBtnsIsEnabled { get => _model.CryptBtnsIsEnabled; }
    }
}
