using System;
using System.Linq;
using System.Windows;
using System.Speech.Recognition;
using System.Threading;
using System.Speech.Synthesis;

namespace SpeechToTextDemo
{
    public partial class MainWindow : Window
    {
        private enum State
        {
            Idle = 0,
            Accepting = 1,
            Off = 2,
        }

        private State RecogState = State.Off;
        private SpeechRecognitionEngine srecog;
        private SpeechSynthesizer synth = null;
        private int Hypothesized = 0;
        private int Recognized = 0;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeRecognizerSynthesizer();

            if (SelectInputDevice())
            {
                LoadDictationGrammar();
                btnStart.IsEnabled = true;
                ReadAloud("Speech Engine Ready for Input");
            }
        }

        private void InitializeRecognizerSynthesizer()
        {
            var selectedRecognizer = (from o in SpeechRecognitionEngine.InstalledRecognizers()
                                      where o.Culture.Equals(Thread.CurrentThread.CurrentCulture)
                                      select o).FirstOrDefault();
            srecog = new SpeechRecognitionEngine(selectedRecognizer);
            srecog.AudioStateChanged += new EventHandler<AudioStateChangedEventArgs>(recognizer_AudioStateChanged);
            srecog.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(recognizer_SpeechHypothesized);
            srecog.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

            synth = new SpeechSynthesizer();
        }

        private bool SelectInputDevice()
        {
            bool proceedLoading = true;
            if (IsOscompatible())
            {
                try
                {
                    srecog.SetInputToDefaultAudioDevice();
                }
                catch
                {
                    proceedLoading = false;
                }
            }
            else
                ThreadPool.QueueUserWorkItem(InitSpeechRecogniser);
            return proceedLoading;
        }

        private bool IsOscompatible()
        {
            OperatingSystem osInfo = Environment.OSVersion;
            if (osInfo.Version > new Version("6.0"))
                return true;
            else
                return false;
        }

        private void InitSpeechRecogniser(object o)
        {
            srecog.SetInputToDefaultAudioDevice();
        }

        private void LoadDictationGrammar()
        {
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(new Choices("End Dictate"));
            Grammar commandGrammar = new Grammar(grammarBuilder);
            commandGrammar.Name = "main command grammar";
            srecog.LoadGrammar(commandGrammar);

            DictationGrammar dictationGrammar = new DictationGrammar();
            dictationGrammar.Name = "dictation";
            srecog.LoadGrammar(dictationGrammar);
        }

        //Hiển thị label trạng thái
        private void recognizer_AudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            switch (e.AudioState)
            {
                case AudioState.Speech:
                    lStatus.Content = "Listening";
                    break;
                case AudioState.Silence:
                    lStatus.Content = "Idle";
                    break;
                case AudioState.Stopped:
                    lStatus.Content = "Stopped";
                    break;
            }
        }

        //Hiển thị đếm số từ có thể nhận dạng (gần giống)
        private void recognizer_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Hypothesized++;
            tHypothesized.Text = "Hypothesized: " + Hypothesized.ToString();
        }
        /*
         * recognizer_SpeechRecognized: Hàm này là trình xử lý sự kiện cho sự kiện SpeechRecognized. 
         * Sự kiện này được kích hoạt khi SpeechRecognitionEngine nhận ra một cụm từ phù hợp với ngữ pháp5. 
         * Trong hàm này, biến Recognized được tăng lên và giá trị của nó được hiển thị trong tRecognized.Text.
         * Nếu RecogState bằng State.Off, hàm sẽ dừng lại. Nếu không, nó sẽ lấy độ chính xác và cụm từ đã nhận 
         * ra từ kết quả nhận dạng. Nếu cụm từ là “End Dictate”, nó sẽ dừng nhận dạng và đọc to “Dictation Ended”.
         * Nếu không, nó sẽ thêm cụm từ đã nhận ra vào TextBox1.
         */
        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Recognized++;
            tRecognized.Text = "Recognized: " + Recognized.ToString();

            if (RecogState == State.Off)
                return;
            float accuracy = (float)e.Result.Confidence;
            string phrase = e.Result.Text;
            {
                if (phrase == "End Dictate")
                {
                    RecogState = State.Off;
                    srecog.RecognizeAsyncStop();
                    ReadAloud("Dictation Ended");
                    return;
                }
                TextBox1.AppendText(" " + e.Result.Text);
            }
        }

        public void ReadAloud(string speakText)
        {
            try
            {
                srecog.RecognizeAsyncCancel();
                synth.SpeakAsync(speakText);
            }
            catch { }
        }


        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            switch (RecogState)
            {
                case State.Off:
                    RecogState = State.Accepting;
                    btnStart.Content = "Stop";
                    srecog.RecognizeAsync(RecognizeMode.Multiple);
                    break;
                case State.Accepting:
                    RecogState = State.Off;
                    btnStart.Content = "Start";
                    srecog.RecognizeAsyncStop();
                    break;
            }
        }
    }
}