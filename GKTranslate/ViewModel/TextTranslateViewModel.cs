using System.Linq;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using GKTranslate.Service;
using System.Windows;
using GKTranslate.Models;
using System;

namespace GKTranslate.ViewModel
{
    internal class TextTranslateViewModel : BindableBase
    {
        public ObservableCollection<CBItem> Language_cb { get; set; }
        private TextTranslateServcie Sv = new TextTranslateServcie();
        public TextTranslateViewModel() 
        {
            Init_Command();
            Init_Models();
        }

        private void Init_Command()
        {
            TranslateCommand = new MyICommand(OnTranslate, CanTranslate);
            CopyCommand = new MyICommand(OnCopy, CanCopy);
            ReloadCommand = new MyICommand(OnReload, () => true);
            ChangeCommand = new MyICommand(OnChange, CanChange);
        }

        private void Init_Models()
        {
            Language_cb = new ObservableCollection<CBItem>
            {
                new CBItem { Name = "English", Value = "en" },
                new CBItem { Name = "Vietnamese", Value = "vi" },
                new CBItem { Name = "French", Value = "fr" },
                new CBItem { Name = "German", Value = "de" },
                new CBItem { Name = "Spanish", Value = "es" },
                new CBItem { Name = "Italian", Value = "it" },
                new CBItem { Name = "Dutch", Value = "nl" },
                new CBItem { Name = "Portuguese", Value = "pt" },
                new CBItem { Name = "Russian", Value = "ru" },
                new CBItem { Name = "Japanese", Value = "ja" },
                new CBItem { Name = "Korean", Value = "ko" },
                new CBItem { Name = "Chinese Simplified", Value = "zh-CN" },
                new CBItem { Name = "Chinese Traditional", Value = "zh-TW" },
                new CBItem { Name = "Arabic", Value = "ar" },
                new CBItem { Name = "Turkish", Value = "tr" },
                new CBItem { Name = "Greek", Value = "el" },
                new CBItem { Name = "Hebrew", Value = "he" },
                new CBItem { Name = "Latin", Value = "la" },
            };
            SelectedFrom_cb = Language_cb.ElementAt(1);
            SelectedTo_cb = Language_cb.ElementAt(0);
        }

        #region properties
        private string _inputTxb;
        public string InputTxb 
        {
            get => _inputTxb;
            set
            {
                if(_inputTxb != value)
                {
                    _inputTxb = value;
                    OnPropertyChanged(nameof(InputTxb));
                    TranslateCommand.RaiseCanExecuteChanged();
                    ChangeCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private string _outputTxb;
        public string OutputTxb
        {
            get => _outputTxb;
            set
            {
                if(_outputTxb != value)
                {
                    _outputTxb = value;
                    OnPropertyChanged(nameof(OutputTxb));
                    CopyCommand.RaiseCanExecuteChanged();
                    ChangeCommand.RaiseCanExecuteChanged();
                }
                
            }
        }
        private CBItem _seletedFrom_cb;
        public CBItem SelectedFrom_cb
        {
            get => _seletedFrom_cb;
            set
            {
                if (_seletedFrom_cb != value)
                {
                    _seletedFrom_cb = value;
                    OnPropertyChanged(nameof(SelectedFrom_cb));
                    if (CanTranslate()) OutputTxb = Sv.TextTranslated(InputTxb, SelectedFrom_cb.Value, SelectedTo_cb.Value);
                }
            }
        }
        private CBItem _seletedTo_cb;
        public CBItem SelectedTo_cb
        {
            get => _seletedTo_cb;
            set
            {
                if(_seletedTo_cb != value)
                {
                    _seletedTo_cb = value;
                    OnPropertyChanged(nameof(SelectedTo_cb));
                    if (CanTranslate()) OutputTxb = Sv.TextTranslated(InputTxb, SelectedFrom_cb.Value, SelectedTo_cb.Value);
                }
            }
        }
        
        #endregion

        #region command
        public MyICommand TranslateCommand { get; set; }
        private void OnTranslate()
        {
            OutputTxb = Sv.TextTranslated(InputTxb,SelectedFrom_cb.Value,SelectedTo_cb.Value);
        }
        private bool CanTranslate()
        {
            return InputTxb != null;
        }

        public MyICommand CopyCommand {get; set;}
        private void OnCopy()
        {
            Clipboard.SetText(OutputTxb);
        }
        private bool CanCopy()
        {
            return OutputTxb != null;
        }

        public MyICommand ReloadCommand { get; set;}
        private void OnReload()
        {
            InputTxb = null;
            OutputTxb = null;
        }

        public MyICommand ChangeCommand { get; set;}
        private void OnChange()
        {
            InputTxb = OutputTxb;

            CBItem tempSelected = SelectedFrom_cb;
            SelectedFrom_cb = SelectedTo_cb;
            SelectedTo_cb = tempSelected;
        }
        private bool CanChange()
        {
            return (InputTxb != null && OutputTxb !=null);
        }
        #endregion
    }
}
