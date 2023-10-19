using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using WpfAppTest.Commands;

namespace WpfAppTest.ViewModels
{
    public class HomeViewModel : Models.BaseNotifyPropertyChanged
    {

        #region models
        private Models.User _selectedUser;

        public Models.User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    RaisePropertyChanged(nameof(SelectedUser));
                    TranslateCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private ObservableCollection<Models.User> _listUser;

        public ObservableCollection<Models.User> ListUser
        {
            get { return _listUser; }
            set
            {
                _listUser = value;
            }
        }


        #endregion


        #region commands
        public VfxCommand TranslateCommand { get; set; }
        #endregion


        public HomeViewModel()
        {
            Init_Command();
            Init_Model();
        }

        private void Init_Command()
        {
            TranslateCommand = new VfxCommand(OnTranslate, CanTranslate);
        }

        private void OnTranslate(object obj)
        {
            
        }

        private bool CanTranslate()
        {            
            return SelectedUser != null;
        }

        private void Init_Model()
        {
            ListUser = new ObservableCollection<Models.User>
            {
                new Models.User() {Name = "Tho", Age = 20},
                new Models.User() {Name = "Thao", Age = 23},
                new Models.User() {Name = "Phuong", Age = 22},
            };
            SelectedUser = ListUser.FirstOrDefault();
        }
    }
}
