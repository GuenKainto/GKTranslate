using GKTranslate.ViewModel;
using GKTranslate.MyCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GKTranslate.ViewModel
{
    internal class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            NavCommand = new NavigateICommand<string>(OnNav);
        }
        private BindableBase _CurrentViewModel;
        public BindableBase CurrentViewModel 
        {
            get => _CurrentViewModel;
            set { SetProperty(ref _CurrentViewModel, value); }
        }

        private TextTranslateViewModel textTranslateViewModel = new TextTranslateViewModel();
        private VoiceTranslateViewModel voiceTranslateViewModel = new VoiceTranslateViewModel();
        public NavigateICommand<string> NavCommand { get; private set; }

        private void OnNav(string destination)
        {

            switch (destination)
            {
                case "text":
                    CurrentViewModel = textTranslateViewModel;
                    break;
                case "voice":
                default:
                    CurrentViewModel = voiceTranslateViewModel;
                    break;
            }
        }
    }
}
