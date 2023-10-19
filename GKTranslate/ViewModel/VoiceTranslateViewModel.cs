using GKTranslate.Models;
using GKTranslate.Service;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Speech.Synthesis;
using System;
using System.Speech.Recognition;
using static System.Net.Mime.MediaTypeNames;

namespace GKTranslate.ViewModel
{
    internal class VoiceTranslateViewModel : BindableBase
    {
        public VoiceTranslateViewModel()
        {       
            Init_Command();
            Init_Models();
        }

        private void Init_Command()
        {
            StartCommand = new MyICommand(OnStart, () => true);
            CopyCommand = new MyICommand(OnCopy, CanCopy);
            ReloadCommand = new MyICommand(OnReload, () => true);
        }

        private void Init_Models()
        {
            StateButton = "Start";
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

        public ObservableCollection<CBItem> Language_cb { get; set; }
        private TextTranslateServcie Sv = new TextTranslateServcie();
        private SpeechRecognitionEngine srecog = new SpeechRecognitionEngine();
        private SpeechSynthesizer synth = new SpeechSynthesizer();

        private string _inputTxb;
        public string InputTxb
        {
            get => _inputTxb;
            set
            {
                if (_inputTxb != value)
                {
                    _inputTxb = value;
                    OnPropertyChanged(nameof(InputTxb));
                    if (_inputTxb != null) OutputTxb = Sv.TextTranslated(InputTxb, SelectedFrom_cb.Value,SelectedTo_cb.Value);
                }
            }
        }
        private string _outputTxb;
        public string OutputTxb
        {
            get => _outputTxb;
            set
            {
                if (_outputTxb != value)
                {
                    _outputTxb = value;
                    OnPropertyChanged(nameof(OutputTxb));
                    CopyCommand.RaiseCanExecuteChanged();
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
                }
            }
        }
        private CBItem _seletedTo_cb;
        public CBItem SelectedTo_cb
        {
            get => _seletedTo_cb;
            set
            {
                if (_seletedTo_cb != value)
                {
                    _seletedTo_cb = value;
                    OnPropertyChanged(nameof(SelectedTo_cb));
                    //if (CanTranslate()) OutputTxb = Sv.TextTranslated(InputTxb, SelectedFrom_cb.Value, SelectedTo_cb.Value);
                }
            }
        }
        private string _stateButton;
        public string StateButton
        {
            get => _stateButton;
            set
            {
                if (_stateButton != value)
                {
                    _stateButton = value;
                    OnPropertyChanged(nameof(StateButton));
                }
            }
        }
        #endregion

        #region command
        public MyICommand StartCommand { get; set; }
        private void OnStart()
        {
            if (StateButton == "Start") // Speech to text input -> Text Translate -> output
            {
                MessageBox.Show("CheckHotKey");
                StateButton = "Stop";
                DictationGrammar dictationGrammar = new DictationGrammar();
                dictationGrammar.Name = "dictation";
                try
                {
                    srecog.RequestRecognizerUpdate();
                    srecog.LoadGrammar(dictationGrammar);
                    srecog.SpeechRecognized += Srecog_SpeechRecognized;
                    srecog.SetInputToDefaultAudioDevice();
                    srecog.RecognizeAsync(RecognizeMode.Multiple);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                StateButton = "Start";
                srecog.RecognizeAsyncStop();
                if (OutputTxb != null)
                {
                    synth.Speak(OutputTxb);
                }
            }
        }
        private void Srecog_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            InputTxb += e.Result.Text.ToString(); //+ Environment.NewLine;
        }

        public MyICommand CopyCommand { get; set; }
        private void OnCopy()
        {
            Clipboard.SetText(OutputTxb);
        }
        private bool CanCopy()
        {
            return OutputTxb != null;
        }

        public MyICommand ReloadCommand { get; set; }
        private void OnReload()
        {
            InputTxb = null;
            OutputTxb = null;
        }
        #endregion
    }
}
