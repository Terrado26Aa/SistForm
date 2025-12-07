using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Forms.Models
{
    public class ToolbarItem : INotifyPropertyChanged
    {
        private string _text;
        private string _iconImageSource;
        private ICommand _command;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public string IconImageSource
        {
            get => _iconImageSource;
            set => SetProperty(ref _iconImageSource, value);
        }

        public ICommand Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }

        private bool _isContextItem = false;
        public bool IsContextItem
        {
            get => _isContextItem;
            set => SetProperty(ref _isContextItem, value);
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
