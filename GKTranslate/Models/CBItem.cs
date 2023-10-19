using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKTranslate.Models
{
    internal class CBItem : BaseNotifyPropertyChanged
    {
        public CBItem() { }
        public CBItem(string name, string value) 
        {
            Name = name;
            Value = value;
        }

        private string _name;
        public string Name 
        {
            get => _name;
            set
            {
                if(_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }
        private string _value;
        public string Value
        {
            get => _value;
            set 
            {
                if(_value != value) { }
                _value = value;
                RaisePropertyChanged($"{nameof(Value)}");
            }
        }
    }
}
