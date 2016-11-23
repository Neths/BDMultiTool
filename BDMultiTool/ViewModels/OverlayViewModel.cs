using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BDMultiTool.ViewModels
{
    public class OverlayViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<MenuItem> _mainMenuItems;
        public IEnumerable<MenuItem> MainMenuItems
        {
            get { return _mainMenuItems; }
            set
            {
                _mainMenuItems = value;
                OnPropertyChanged();
            }
        }
    }
}
